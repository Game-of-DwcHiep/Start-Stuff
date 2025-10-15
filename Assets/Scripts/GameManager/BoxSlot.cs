using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BoxSlot : MonoBehaviour
{
    [Header("Material Settings")]
    public Material defaultMat;
    public Material highlightMat;

    [Header("References")]
    public MeshRenderer meshRenderer;      
    public MeshRenderer meshColliderLine;  
    public PlayerController player;        

    [Header("Slot State")]
    public bool hasBox = false;            
    private int insideCount = 0;
    public bool isPlayerInside => insideCount > 0;  

    public BoxHighlighter boxHighlighter;
    public BoxCollider boxCollider;

    // ✅ Thêm bot reference
    private BotController bot;  

    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        boxHighlighter = null;
        UpdateMaterial();
        boxCollider.enabled = true;
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (isPlayerInside || hasBox)
        {
            boxCollider.enabled = false;
            //AstarPath.active.Scan();
        }
        else
            boxCollider.enabled = true;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player
            var p = other.GetComponent<PlayerController>();
            if (p != null)
            {
                player = p;
                player.SetNearbySlot(this);
            }

            // Bot
            var b = other.GetComponent<BotController>();
            if (b != null)
            {
                bot = b;
                bot.SetNearbySlot(this);
            }

            insideCount++; // ✅ tăng số lượng khi có ai đó bước vào
            UpdateMaterial();
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var p = other.GetComponent<PlayerController>();
            if (p != null)
            {
                p.ClearNearbySlot(this);
                if (player == p) player = null;
            }

            var b = other.GetComponent<BotController>();
            if (b != null)
            {
                b.ClearNearbySlot(this);
                if (bot == b) bot = null;
            }

            insideCount = Mathf.Max(0, insideCount - 1); // ✅ giảm số lượng
            UpdateMaterial();
        }
    }


    // 📦 Đặt hoặc gỡ box
    public void SetBox(bool value)
    {
        hasBox = value;
        UpdateMaterial();
        boxCollider.enabled = !value;
    }

    void UpdateMaterial()
    {
        bool isLocked = hasBox || isPlayerInside;

        var mat = meshColliderLine.materials;
        if (mat.Length > 1)
        {
            mat[1] = isLocked ? highlightMat : defaultMat;
            meshColliderLine.materials = mat;
        }

        meshRenderer.material = isLocked ? highlightMat : defaultMat;
    }

    public Vector3 GetSlotPosition()
    {
        Vector3 pos = transform.position;
        pos.y = 19f;
        return pos;
    }
}

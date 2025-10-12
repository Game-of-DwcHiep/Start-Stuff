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
    public bool isPlayerInside = false;    

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
        if(isPlayerInside)
            boxCollider.enabled = false;
    }
    void OnTriggerEnter(Collider other)
    {
        // 🎮 Player vào vùng
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerController>();
            isPlayerInside = true;
            if (player != null)
                player.SetNearbySlot(this);
            bot = other.GetComponent<BotController>();
            if (bot != null)
                bot.SetNearbySlot(this);
            UpdateMaterial();
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 🎮 Player rời vùng
        if (other.CompareTag("Player"))
        {
            player?.ClearNearbySlot(this);
            player = null;
            isPlayerInside = false;

            bot?.ClearNearbySlot(this);
            bot = null;

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

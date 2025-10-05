using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BoxSlot : MonoBehaviour
{
    [Header("Material Settings")]
    public Material defaultMat;
    public Material highlightMat;

    [Header("References")]
    public MeshRenderer meshRenderer;      // hiển thị box slot chính
    public MeshRenderer meshColliderLine;  // line outline (nếu có)
    public PlayerController player;        // player hiện đang ở gần slot

    [Header("Slot State")]
    public bool hasBox = false;            // true = đã có box đặt lên
    public bool isPlayerInside = false;    // true = player đang đứng trong vùng slot

    public BoxHighlighter boxHighlighter;

    public BoxCollider boxCollider;

    void Start()
    {
        GetComponent<Collider>().isTrigger = true; // vùng trigger
        boxHighlighter = null;
        UpdateMaterial();
        boxCollider.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerController>();
            isPlayerInside = true;
            player.SetNearbySlot(this); // 📌 báo player biết slot này đang gần
            UpdateMaterial();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.ClearNearbySlot(this);
            player = null;
            isPlayerInside = false;
            UpdateMaterial();
        }
    }

    // 📦 Gọi khi box được đặt lên slot
    public void SetBox(bool value)
    {
        hasBox = value;
        UpdateMaterial();
        boxCollider.enabled = !value; // tắt collider khi đã có box
    }

    // 📄 Hàm cập nhật hiển thị
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
        pos.y = 19f; // luôn cố định trục Y = 12.7f
        return pos;
    }
}

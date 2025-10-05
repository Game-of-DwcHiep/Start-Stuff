using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FinalLock : MonoBehaviour
{
    [Header("Material Settings")]
    public Material normalMat;
    public Material highlightMat;

    [Header("Visual")]
    public MeshRenderer meshRenderer; // phần hiển thị ổ khoá
    private PlayerController player;

    void Start()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true; // chỉ phát hiện player, không cản đường
        SetMaterial(normalMat);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerController>();
            player.wingame = true; // báo player là đã chạm khoá cuối
            SetMaterial(highlightMat);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            SetMaterial(normalMat);
        }
    }

    

    void SetMaterial(Material mat)
    { 
        if (meshRenderer)
            meshRenderer.material = mat;
    }
}

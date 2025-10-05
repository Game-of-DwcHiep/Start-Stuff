using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [Header("Materials")]
    public Material defaultMaterial;
    public Material highlightMaterial;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material = defaultMaterial;
    }

    // Gọi từ vùng Trigger con
    public void SetHighlight(bool isNear)
    {
        rend.material = isNear ? highlightMaterial : defaultMaterial;
    }
}

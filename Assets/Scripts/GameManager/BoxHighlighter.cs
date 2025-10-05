using UnityEngine;

public class BoxHighlighter : MonoBehaviour
{
    public Material defaultMaterial;
    public Material highlightMaterial;

    public Renderer rend;

    [HideInInspector] public bool isPickedUp = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material = defaultMaterial;
    }

    public void SetHighlight(bool isNear)
    {
        if (isPickedUp) return;
        rend.material = isNear ? highlightMaterial : defaultMaterial;
    }

    public void SetPickedUp(bool picked)
    {
        isPickedUp = picked;
        rend.material = picked ? highlightMaterial : defaultMaterial;
    }
}

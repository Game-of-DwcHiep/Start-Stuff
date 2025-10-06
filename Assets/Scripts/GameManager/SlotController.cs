using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlotController : MonoSingleton<SlotController>
{
    public BoxCollider boxCollider;
    public int key = 0;
    public Material defaultMaterial;
    public Material highlightMaterial;

    public Renderer rend;

    private void Start()
    {
        boxCollider.enabled = true;
    }
    public void UnLock()
    {
        key ++;
        if (key >= 2)
        {
            boxCollider.enabled = false;
            rend.material = highlightMaterial;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSlot2 : MonoBehaviour
{
    [Header("Material Settings")]
    public Material defaultMat;
    public Material highlightMat;

    public MeshRenderer meshRenderer;       // hiển thị khối slot
    public MeshRenderer meshColliderLine;   // phần line (nếu có)
    public bool hasBox = false;             // đang có box chưa?

    private void Start()
    {
        // collider chính để chặn player
        var col = GetComponent<BoxCollider>();
        col.isTrigger = false;
        UpdateMaterial(defaultMat);
    }

    public void PlaceBox(GameObject box)
    {
        hasBox = true;
        UpdateMaterial(highlightMat);

        // Đặt box đúng vị trí slot
        Vector3 pos = transform.position;
        //pos.y = 12.7f; // cố định chiều cao
        box.transform.position = pos;
        box.transform.rotation = transform.rotation;
        box.transform.SetParent(transform);
        SlotController.Instance.UnLock();
    }
/*
    public void RemoveBox()
    {
        hasBox = false;
        UpdateMaterial(defaultMat);
    }

    public Vector3 GetSlotPosition()
    {
        Vector3 pos = transform.position;
        pos.y = 12.7f;
        return pos;
    }
*/
    public void UpdateMaterial(Material mat)
    {
        if (meshRenderer)
        {
            var mats = meshRenderer.materials;
            mats[3] = mat;
            meshRenderer.materials = mats;
        }
        if (meshColliderLine)
        {
            var mats = meshColliderLine.materials;
            if (mats.Length > 1)
            {
                mats[3] = mat;
                meshColliderLine.materials = mats;
            }
        }
    }
}

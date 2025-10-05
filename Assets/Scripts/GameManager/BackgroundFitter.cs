using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFitter : MonoBehaviour
{
    public Camera _camera; // Camera chính
    public GameObject BG; // GameObject chứa SpriteRenderer của background

    void Start()
    {
        FitBackgroundToCamera();
    }

    private void FitBackgroundToCamera()
    {
        if (_camera == null)
        {
            Debug.LogError("Không tìm thấy Camera! Hãy gán Camera vào Inspector.");
            return;
        }

        if (BG == null)
        {
            Debug.LogError("Không tìm thấy BG! Hãy gán GameObject BG vào Inspector.");
            return;
        }

        // Lấy SpriteRenderer của BG
        SpriteRenderer spriteRenderer = BG.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Không tìm thấy SpriteRenderer trên BG!");
            return;
        }

        // Kích thước Sprite
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        // Kích thước Camera (chiều cao và chiều rộng dựa trên Orthographic Size)
        float cameraHeight = 2f * _camera.orthographicSize;
        float cameraWidth = cameraHeight * _camera.aspect;

        // Tính tỷ lệ scale để BG vừa với Camera
        Vector3 scale = BG.transform.localScale;
        scale.x = cameraWidth / spriteSize.x;
        scale.y = cameraHeight / spriteSize.y;

        // Gán lại scale cho BG
        BG.transform.localScale = scale;
    }
}

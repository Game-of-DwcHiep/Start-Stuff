using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DwcHyep
{
    public class GameScreen : Screen
    {
        public void BackGoHome()
        {
            if(GameManager.Instance.checkCreateMap)
            {
                return;
            }
            ScreenManager.Instance.Back();
            UIManager.Instance.OpenMainMenu();
        }

        // Hàm xử lý khi nhấn giữ nút lên
        public void OnButtonHoldUp()
        {
            if (WormController.Instance != null && !GameManager.Instance.checkCreateMap && GameManager.Instance.isMoving)
            {
                WormController.Instance.OnButtonDown(Vector2.up);
            }
        }

        // Hàm xử lý khi nhấn giữ nút xuống
        public void OnButtonHoldDown()
        {
            if (WormController.Instance != null && !GameManager.Instance.checkCreateMap && GameManager.Instance.isMoving)
            {
                WormController.Instance.OnButtonDown(Vector2.down);
            }
        }

        // Hàm xử lý khi nhấn giữ nút trái
        public void OnButtonHoldLeft()
        {
            if (WormController.Instance != null && !GameManager.Instance.checkCreateMap && GameManager.Instance.isMoving)
            {
                WormController.Instance.OnButtonDown(Vector2.left);
            }
        }

        // Hàm xử lý khi nhấn giữ nút phải
        public void OnButtonHoldRight()
        {
            if (WormController.Instance != null && !GameManager.Instance.checkCreateMap && GameManager.Instance.isMoving)
            {
                WormController.Instance.OnButtonDown(Vector2.right);
            }
        }

        // Hàm xử lý khi thả nút
        public void OnButtonRelease( )
        {
            if (WormController.Instance != null && !GameManager.Instance.checkCreateMap && GameManager.Instance.isMoving)
            {
                WormController.Instance.OnButtonUp();
            }
        }
    }
}

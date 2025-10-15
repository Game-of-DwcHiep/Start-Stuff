using UnityEngine;
using UnityEngine.UI;
public class SelectLevel : MonoBehaviour
{
    [Header("Danh sách các button level (gắn trong Inspector theo thứ tự)")]
    public Button[] levelButtons;

    private void OnEnable()
    {
        UpdateLevelButtons();
    }

    private void UpdateLevelButtons()
    {
        // 🔹 Lấy level hiện tại từ GameManager
        int currentLevel = GameManager.Instance.LoadLevel();

        // 🔹 Lặp qua toàn bộ button và bật/tắt theo level
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1; // vì mảng bắt đầu từ 0, level bắt đầu từ 1
            bool isUnlocked = levelIndex <= currentLevel;

            levelButtons[i].interactable = isUnlocked;

            // Tuỳ chọn: đổi màu button cho đẹp
            ColorBlock colors = levelButtons[i].colors;
            colors.normalColor = isUnlocked ? Color.white : Color.gray;
            levelButtons[i].colors = colors;
        }
    }
}

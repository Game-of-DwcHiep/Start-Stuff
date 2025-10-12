using UnityEngine;
using UnityEngine.UI;

public class UIActionManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform libraryPanel;   // Panel chứa các action ban đầu
    public Transform dropZonePanel;  // Panel chứa các action đã chọn
    public Button runButton;
    public Button resetButton;

    public Button clearButton;

    [Header("Game References")]
    public LevelController2 levelController;

    public Text actionNumberText;
    public int actionNumber = 0;

    public GameObject panel;

    public GameObject panelMoving;

    private void Start()
    {
        runButton.gameObject.SetActive(true);
        resetButton.gameObject.SetActive(false);
        actionNumberText.text = actionNumber.ToString() + "/3 " + "lines";
        panel.SetActive(true);
        panelMoving.SetActive(false);
        runButton.onClick.AddListener(OnRunClicked);
        clearButton.onClick.AddListener(ClearActions);
        resetButton.onClick.AddListener(OnResetClicked);
        
    }

    public void MoveToDropZone(UIActionItem item)
    {
        actionNumber++;
        actionNumberText.text = actionNumber.ToString() + "/3 " + "lines";
        Transform newParent = (item.transform.parent == dropZonePanel) ? libraryPanel : dropZonePanel;
        item.transform.SetParent(newParent);
        item.transform.localScale = Vector3.one;
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void OnRunClicked()
    {
        // Gọi chạy chuỗi hành động
        panel.SetActive(false);
        levelController.StartSequence();
        panelMoving.SetActive(true);
        runButton.interactable = false;
        clearButton.interactable = false;
        resetButton.gameObject.SetActive(true);
    }
    public void OnResetClicked()
    {
        // Reset lại level
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    public void ClearActions()
    {
        // Đưa tất cả item trong DropZone về lại Library
        var items = dropZonePanel.GetComponentsInChildren<UIActionItem>();
        foreach (var item in items)
        {
            item.transform.SetParent(libraryPanel);
            item.transform.localScale = Vector3.one;
            item.choose = false;
        }

        // Xóa các bước đã lưu trong LevelController
        levelController.steps.Clear();

        actionNumber = 0;
        actionNumberText.text = actionNumber.ToString() + "/3 " + "lines";
    }
}

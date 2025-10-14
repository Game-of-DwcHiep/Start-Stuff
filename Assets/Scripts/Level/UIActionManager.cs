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


    //
    private ActionType currentActionType;
    private bool isChoosingTarget = false;
    private UIActionItem currentActionItem;

    public GameObject panelAction;

    public GameObject panelMoving;

    public GameObject panelTarget;

    public GameObject[] abc;


    private void Start()
    {
        runButton.gameObject.SetActive(true);
        resetButton.gameObject.SetActive(false);
        actionNumberText.text = actionNumber.ToString() + "/3 " + "lines";
        panelAction.SetActive(true);
        panelTarget.SetActive(false);
        panelMoving.SetActive(false);
        runButton.onClick.AddListener(OnRunClicked);
        clearButton.onClick.AddListener(ClearActions);
        resetButton.onClick.AddListener(OnResetClicked);
        foreach(var a in abc)
        {
            a.SetActive(true);
        }    
    }

    public void OnActionTypeSelected(UIActionItem item)
    {
        currentActionItem = item;
        currentActionType = item.actionType;
        isChoosingTarget = true;
        panelAction.SetActive(false);
        panelTarget.SetActive(true);
        foreach (var a in abc)
        {
            a.SetActive(true);
        }
        Debug.Log($"Chọn action: {currentActionType}. Hãy chọn vị trí target.");
    }

    // Bước 2: người chơi chọn target
    public void OnTargetSelected(Transform target)
    {
        if (!isChoosingTarget)
        {
            Debug.LogWarning("Chưa chọn loại hành động!");
            return;
        }

        // Tạo step và thêm vào danh sách hành động
        levelController.steps.Add(new BotActionStep
        {
            actionType = currentActionType,
            targetPosition = target.transform
        });

        panelAction.SetActive(true);
        panelTarget.SetActive(false);

        foreach (var a in abc)
        {
            a.SetActive(false);
        }
        Debug.Log($"Hoàn tất: {currentActionType} → {target.name}");

        // Reset trạng thái
        isChoosingTarget = false;
        currentActionItem.choose = false;
        currentActionItem = null;
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
        panelAction.SetActive(false);
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
            item.GetComponent<Button>().interactable = true;
            item.transform.SetParent(libraryPanel);
            item.transform.localScale = Vector3.one;
            item.choose = false;
        }
        var t = panelTarget.transform;
        foreach(Transform b in t)
        {
            b.gameObject.SetActive(true);
        }
        panelAction.SetActive(true);
        panelTarget.SetActive(false);
        // Xóa các bước đã lưu trong LevelController
        levelController.steps.Clear();

        actionNumber = 0;
        actionNumberText.text = actionNumber.ToString() + "/3 " + "lines";
    }
}

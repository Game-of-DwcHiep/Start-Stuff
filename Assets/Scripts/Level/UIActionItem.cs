using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIActionItem : MonoBehaviour
{
    public ActionType actionType;            // Loại hành động (Move, Attack, Pickup, ...)
    public UIActionManager manager;          // Tham chiếu đến manager

    private Button button;
    public bool choose = false;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (choose) return;
        choose = true;

        // Gọi manager để báo rằng người chơi đã chọn loại action này
        manager.OnActionTypeSelected(this);
        manager.MoveToDropZone(this);
        button.interactable = false;
    }
}

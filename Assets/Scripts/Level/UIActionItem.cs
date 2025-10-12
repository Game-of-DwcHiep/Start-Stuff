using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIActionItem : MonoBehaviour
{
    public ActionType actionType;
    public Transform targetPosition; // 🔹 Đã được gắn sẵn trong Inspector
    public UIActionManager manager;

    private Button button;
    public bool choose = false;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if(choose) return;
        manager.levelController.steps.Add(new BotActionStep { actionType = this.actionType, targetPosition = this.targetPosition });
        manager.MoveToDropZone(this);
        choose = true;
        StartCoroutine(FlashMaterial());
    }
    private IEnumerator FlashMaterial()
    {
        if (targetPosition == null || GameManager.Instance.highlightMat == null)
            yield break;

        MeshRenderer mr = targetPosition.GetComponent<MeshRenderer>();
        if (mr == null)
            yield break;


        mr.material = GameManager.Instance.highlightMat;

        yield return new WaitForSeconds(1f); // thời gian hiển thị (1 giây)

        // Trả lại material ban đầu
        mr.material = GameManager.Instance.defaultMat;
    }
}

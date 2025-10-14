using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIActionTarget : MonoBehaviour
{
    private Button button;
    public UIActionManager manager;

    public Transform target;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {        
        StartCoroutine(FlashMaterial());
    }

    private IEnumerator FlashMaterial()
    {
        if (target == null || GameManager.Instance.highlightMat == null)
            yield break;

        MeshRenderer mr = target.GetComponent<MeshRenderer>();
        if (mr == null)
            yield break;


        mr.material = GameManager.Instance.highlightMat;

        yield return new WaitForSeconds(0.5f); // thời gian hiển thị (1 giây)

        // Trả lại material ban đầu
        mr.material = GameManager.Instance.defaultMat;
        manager.OnTargetSelected(target);
        //manager.MoveToDropZone(UIActionManager.cur)
        gameObject.SetActive(false);
    }
}

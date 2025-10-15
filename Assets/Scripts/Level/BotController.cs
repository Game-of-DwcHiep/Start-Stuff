
using Pathfinding;
using System.Collections;
using UnityEngine;

public class BotController : MonoBehaviour
{
    public Transform holdPoint;

    private IAstarAI ai;
    private Rigidbody rb;

    public BoxHighlighter carriedBox;
    public BoxSlot nearbySlot;
    public BoxHighlighter nearbyBox;
    public bool isStop = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ai = GetComponent<IAstarAI>();
        rb.freezeRotation = true;
    }

    // ✅ Kiểm tra có thể di chuyển đến vị trí hay không
    public bool CanReach(Vector3 targetPosition)
    {
        var startNode = AstarPath.active.GetNearest(transform.position).node;
        var endNode = AstarPath.active.GetNearest(targetPosition).node;
        if (startNode == null || endNode == null) return false;
        return Pathfinding.PathUtilities.IsPathPossible(startNode, endNode);
    }

    // ✅ Di chuyển đến vị trí bằng A* (tự động)
    public IEnumerator MoveTo(Vector3 targetPosition)
    {
        if (!CanReach(targetPosition))
        {
            Debug.LogWarning("⚠️ Không có đường đến vị trí này!");
            yield break;
        }

        ai.destination = targetPosition;
        ai.SearchPath();
        isStop = false;

        var aipath = ai as AIPath;

        while (ai.pathPending || aipath.remainingDistance > aipath.endReachedDistance)
        {
            yield return null;
        }   

        isStop = true;
    }
    public IEnumerator PickUp(Vector3 targetPosition)
    {
        // Tạm disable collider của box target trước khi đi
        if (nearbyBox != null)
        {
            foreach (var c in nearbyBox.GetComponentsInChildren<Collider>())
                c.enabled = false;
        }

        // Di chuyển đến box (dùng pathfinding)
        ai.destination = targetPosition;
        ai.SearchPath();
        isStop = false;

        var aipath = ai as AIPath;

        while (nearbyBox == null && (ai.pathPending || aipath.remainingDistance > aipath.endReachedDistance))
        {
            yield return null;
        }
        isStop = true;
        // Khi đến nơi -> nhặt box
        if (nearbyBox != null)
        {
            carriedBox = nearbyBox;
            nearbyBox.SetPickedUp(true);

            // Gắn box lên tay bot
            nearbyBox.transform.SetParent(holdPoint);
            nearbyBox.transform.localPosition = Vector3.zero;
            nearbyBox.transform.localRotation = Quaternion.identity;
            nearbyBox = null;
        }
    }

    // ✅ Hành động thả box
    public IEnumerator DropAt(Vector3 targetPosition)
    {
        if (carriedBox == null)
        {
            Debug.LogWarning("❌ Không có box nào để thả!");
            yield break;
        }

        // 🔹 Di chuyển đến vị trí target
        ai.destination = targetPosition;
        ai.SearchPath();
        isStop = false;

        var aipath = ai as AIPath;

        // 🔸 Chỉ tiếp tục di chuyển khi chưa vào vùng slot
        while (nearbySlot == null && (ai.pathPending || aipath.remainingDistance > aipath.endReachedDistance))
        {
            yield return null;
        }

        // 🔸 Khi phát hiện slot, dừng lại
        isStop = true;
        ai.destination = transform.position; // dừng di chuyển hoàn toàn

        if (nearbySlot == null)
        {
            Debug.LogWarning("❌ Không tìm thấy slot để thả box!");
            yield break;
        }

        // ==========================
        //  THẢ BOX VÀO SLOT
        // ==========================
        carriedBox.SetPickedUp(false);

        foreach (var c in carriedBox.GetComponentsInChildren<Collider>())
            c.enabled = true;

        // 🔹 Gắn box vào slot
        carriedBox.transform.SetParent(nearbySlot.transform);
        carriedBox.transform.localPosition = Vector3.zero + Vector3.forward * 0.5f; // nâng lên một chút cho đẹp
        carriedBox.transform.localRotation = Quaternion.identity;

        // 🔹 Cập nhật trạng thái slot
        nearbySlot.SetBox(true);
        nearbySlot.boxHighlighter = carriedBox;

        // 🔹 Reset lại biến tạm
        carriedBox = null;
        nearbySlot = null;

        Debug.Log("✅ Đã thả box thành công vào slot!");
    }



    // =============================
    //  Gắn slot / box gần nhất từ trigger
    // =============================
    public void SetNearbySlot(BoxSlot slot)
    {
        nearbySlot = slot;
    }

    public void SetNearbyBox(BoxHighlighter box)
    {
        nearbyBox = box;
    }

    public void ClearNearbySlot(BoxSlot slot)
    {
        if (nearbySlot == slot)
            nearbySlot = null;
    }

    public void ClearNearbyBox(BoxHighlighter box)
    {
        if (nearbyBox == box)
            nearbyBox = null;
    }
}

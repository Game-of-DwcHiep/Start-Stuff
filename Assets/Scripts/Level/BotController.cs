// using Pathfinding;
// using System.Collections;
// using UnityEngine;

// //[RequireComponent(typeof(Rigidbody))]
// //[RequireComponent(typeof(BoxCollider))]
// public class BotController : MonoBehaviour
// {
//     [Header("Movement Settings")]
//     public float speed = 3f;

//     [Header("Pickup Settings")]
//     public Transform holdPoint;             
//     public Transform dropPoint;             

//     private Rigidbody rb;

//     private BoxHighlighter carriedBox;      
//     public BoxSlot nearbySlot;              
//     public BoxHighlighter nearbyBox;

//     public Transform transformCurrent;
//     public Transform target;
//     IAstarAI ai;
//     public bool isStop = true;

//     void Start()
//     {
//         rb = GetComponent<Rigidbody>();
//         rb.freezeRotation = true;
//         isStop = true;
//     }
//     void OnEnable()
//     {
//         ai = GetComponent<IAstarAI>();
//         if (ai != null) ai.onSearchPath += Update;
//     }

//     void OnDisable()
//     {
//         if (ai != null) ai.onSearchPath -= Update;
//         isStop = true;
//     }

//     /// <summary>Updates the AI's destination every frame</summary>
//     void Update()
//     {
//         if (target != null && ai != null && !isStop) ai.destination = target.position;
//     }

//     public bool CanReach(Vector3 targetPosition)
//     {
//         // Lấy nearest node của bot và target
//         var startNode = AstarPath.active.GetNearest(transform.position).node;
//         var endNode = AstarPath.active.GetNearest(targetPosition).node;

//         // Kiểm tra nếu một trong hai node null → không thể đi
//         if (startNode == null || endNode == null)
//             return false;

//         // Kiểm tra có kết nối hay không
//         return PathUtilities.IsPathPossible(startNode, endNode);
//     }

//     public IEnumerator PickUp(Vector3 targetPosition)
//     {
//         // 🔹 Kiểm tra xem có đường không
//         if (!CanReach(targetPosition))
//         {
//             Debug.LogWarning("❌ Bot không thể di chuyển đến vị trí pickup!");
//             yield break;
//         }

//         // 🔹 Di chuyển đến vị trí box
//         while (nearbyBox == null)
//         {
//             Vector3 direction = (targetPosition - transform.position).normalized;
//             transform.rotation = Quaternion.LookRotation(-direction);
//             rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
//             yield return null;
//         }

//         // 🔹 Khi tới gần box
//         if (nearbyBox != null)
//         {
//             carriedBox = nearbyBox;
//             nearbyBox.SetPickedUp(true);

//             foreach (var c in nearbyBox.GetComponentsInChildren<Collider>())
//                 c.enabled = false;

//             nearbyBox.transform.SetParent(holdPoint);
//             nearbyBox.transform.localPosition = Vector3.zero;
//             nearbyBox.transform.localRotation = Quaternion.identity;

//             nearbyBox = null;
//         }
//     }

//     public IEnumerator DropAt(Vector3 targetPosition)
//     {
//         if (carriedBox == null)
//             yield break;

//         if (!CanReach(targetPosition))
//         {
//             Debug.LogWarning("❌ Bot không thể di chuyển đến vị trí drop!");
//             yield break;
//         }

//         while (nearbySlot == null)
//         {
//             Vector3 direction = (targetPosition - transform.position).normalized;
//             transform.rotation = Quaternion.LookRotation(-direction);
//             rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
//             yield return null;
//         }

//         if (nearbySlot != null)
//         {
//             carriedBox.SetPickedUp(false);
//             foreach (var c in carriedBox.GetComponentsInChildren<Collider>())
//                 c.enabled = true;

//             carriedBox.transform.SetParent(null);
//             carriedBox.transform.position = nearbySlot.GetSlotPosition();

//             nearbySlot.SetBox(true);
//             nearbySlot.boxHighlighter = carriedBox;

//             carriedBox = null;
//             nearbySlot = null;
//         }
//     }


//     // public IEnumerator PickUp( Vector3 targetPosition)
//     // {

//     //     while (nearbyBox == null)
//     //     {
//     //         Vector3 direction = (targetPosition - transform.position).normalized;
//     //         transform.rotation = Quaternion.LookRotation(-direction);
//     //         rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
//     //         yield return null;
//     //     }
//     //     // 2️⃣ Khi đã nhận được nearbyBox → nhặt box
//     //     if (nearbyBox != null)
//     //     {
//     //         carriedBox = nearbyBox;
//     //         nearbyBox.SetPickedUp(true);

//     //         // 🔹 Tắt collider
//     //         foreach (var c in nearbyBox.GetComponentsInChildren<Collider>())
//     //             c.enabled = false;

//     //         // 🔹 Gắn box lên tay bot
//     //         nearbyBox.transform.SetParent(holdPoint);
//     //         nearbyBox.transform.localPosition = Vector3.zero;
//     //         nearbyBox.transform.localRotation = Quaternion.identity;

//     //         // 🔹 Reset nearbyBox (để tránh trigger lại nhặt)
//     //         nearbyBox = null;
//     //     }
//     // }

//     // public IEnumerator DropAt(Vector3 targetPosition)
//     // {
//     //     if (carriedBox == null)
//     //         yield break;

//     //     while (nearbySlot == null)
//     //     {
//     //         Vector3 direction = (targetPosition - transform.position).normalized;
//     //         transform.rotation = Quaternion.LookRotation(-direction);
//     //         rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
//     //         yield return null;
//     //     }

//     //     // 2️⃣ Khi đã đến slot → thả box xuống
//     //     if (nearbySlot != null)
//     //     {
//     //         carriedBox.SetPickedUp(false);

//     //         foreach (var c in carriedBox.GetComponentsInChildren<Collider>())
//     //             c.enabled = true;

//     //         carriedBox.transform.SetParent(null);
//     //         carriedBox.transform.position = nearbySlot.GetSlotPosition();

//     //         nearbySlot.SetBox(true);
//     //         nearbySlot.boxHighlighter = carriedBox;

//     //         carriedBox = null;
//     //         nearbySlot = null;
//     //     }
//     // }

//     // =============================
//     //  Gắn slot / box gần nhất từ trigger
//     // =============================
//     public void SetNearbySlot(BoxSlot slot)
//     {
//         nearbySlot = slot;
//     }

//     public void SetNearbyBox(BoxHighlighter box)
//     {
//         nearbyBox = box;
//     }

//     public void ClearNearbySlot(BoxSlot slot)
//     {
//         if (nearbySlot == slot)
//             nearbySlot = null;
//     }

//     public void ClearNearbyBox(BoxHighlighter box)
//     {
//         if (nearbyBox == box)
//             nearbyBox = null;
//     }
// }

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

    // ✅ Hành động nhặt box
    // public IEnumerator PickUp(Vector3 targetPosition)
    // {
    //     // 1️⃣ Di chuyển tới vị trí box
    //     yield return MoveTo(targetPosition);

    //     if (nearbyBox == null)
    //     {
    //         Debug.LogWarning("❌ Không có box nào để nhặt!");
    //         yield break;
    //     }

    //     // 2️⃣ Nhặt box
    //     carriedBox = nearbyBox;
    //     nearbyBox.SetPickedUp(true);

    //     foreach (var c in nearbyBox.GetComponentsInChildren<Collider>())
    //         c.enabled = false;

    //     nearbyBox.transform.SetParent(holdPoint);
    //     nearbyBox.transform.position = Vector3.zero;
    //     nearbyBox.transform.rotation = Quaternion.identity;

    //     nearbyBox = null;

    //     Debug.Log("✅ Đã nhặt box thành công!");
    // }

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
            Debug.LogError("aaa");
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

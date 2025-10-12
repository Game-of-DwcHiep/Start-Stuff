using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class BotController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;

    [Header("Pickup Settings")]
    public Transform holdPoint;             
    public Transform dropPoint;             

    private Rigidbody rb;

    private BoxHighlighter carriedBox;      
    public BoxSlot nearbySlot;              
    public BoxHighlighter nearbyBox;       

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // =============================
    //  Di chuyển tự động
    // =============================
    // public IEnumerator MoveTo(Vector3 target)
    // {
    //     Vector3 direction = (target - transform.position).normalized;
    //     transform.rotation = Quaternion.LookRotation(-direction);
    //     while (Vector3.Distance(transform.position, target) > 0.1f)
    //     {         
    //         direction = (target - transform.position).normalized;
    //         rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
    //         yield return null;
    //     }
    //     if(nearbySlot != null)
    //         nearbySlot.SetBox(false);
    // }

    // public IEnumerator MoveTo(Vector3 target)
    // {
    //     // 1️⃣ Di chuyển theo trục X trước
    //     Vector3 start = transform.position;

    //     // di chuyển trên trục X
    //     while (Mathf.Abs(transform.position.x - target.x) > 0.05f)
    //     {
    //         float directionX = Mathf.Sign(target.x - transform.position.x);
    //         Vector3 moveDir = new Vector3(directionX, 0, 0);
    //         transform.rotation = Quaternion.LookRotation(-moveDir);
    //         rb.MovePosition(rb.position + moveDir * speed * Time.deltaTime);
    //         yield return null;
    //     }

    //     // 2️⃣ Sau đó di chuyển theo trục Z
    //     while (Mathf.Abs(transform.position.z - target.z) > 0.05f)
    //     {
    //         float directionZ = Mathf.Sign(target.z - transform.position.z);
    //         Vector3 moveDir = new Vector3(0, 0, directionZ);
    //         transform.rotation = Quaternion.LookRotation(-moveDir);
    //         rb.MovePosition(rb.position + moveDir * speed * Time.deltaTime);
    //         yield return null;
    //     }

    //     // Giữ nguyên Y (tránh thay đổi độ cao)
    //     transform.position = new Vector3(target.x, start.y, target.z);

    //     // ✅ Dừng lại hoàn toàn khi đến nơi
    //     rb.linearVelocity = Vector3.zero;

    //     if (nearbySlot != null)
    //         nearbySlot.SetBox(false);
    // }

    public IEnumerator MoveTo(Vector3 target)
    {
        Vector3 start = transform.position;
        Vector3 current = start;

        // Giới hạn vòng lặp (phòng tránh lỗi vô hạn)
        int safetyCounter = 0;

        while (Vector3.Distance(current, target) > 0.1f && safetyCounter < 500)
        {
            safetyCounter++;

            // Tính hướng di chuyển chính (theo trục lớn hơn)
            Vector3 diff = target - current;
            Vector3 moveDir = Vector3.zero;

            // Chọn trục nào còn xa hơn
            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
                moveDir = new Vector3(Mathf.Sign(diff.x), 0, 0);
            else
                moveDir = new Vector3(0, 0, Mathf.Sign(diff.z));

            // Kiểm tra xem hướng đó có bị chặn không
            if (Physics.Raycast(current + Vector3.up * 0.5f, moveDir, out RaycastHit hit, 1f))
            {
                // Nếu bị chặn, thử đổi trục
                Vector3 altDir = (moveDir.x != 0) ? new Vector3(0, 0, Mathf.Sign(diff.z)) : new Vector3(Mathf.Sign(diff.x), 0, 0);

                // Nếu hướng phụ không bị chặn → đi hướng đó
                if (!Physics.Raycast(current + Vector3.up * 0.5f, altDir, 1f))
                {
                    moveDir = altDir;
                }
                else
                {
                    // Nếu cả hai hướng đều bị chặn → dừng
                    Debug.LogWarning("🚧 Bot bị kẹt tại " + current);
                    yield break;
                }
            }

            // Xoay bot theo hướng di chuyển
            transform.rotation = Quaternion.LookRotation(-moveDir);

            // Di chuyển dần dần tới bước tiếp theo
            Vector3 nextPos = current + moveDir;
            while (Vector3.Distance(transform.position, nextPos) > 0.05f)
            {
                rb.MovePosition(Vector3.MoveTowards(rb.position, nextPos, speed * Time.deltaTime));
                yield return null;
            }

            current = nextPos; // cập nhật vị trí mới
        }

        rb.linearVelocity = Vector3.zero;
        transform.position = new Vector3(target.x, start.y, target.z);
        if (nearbySlot != null)
            nearbySlot.SetBox(false);
    }


    // =============================
    //  Hành động: Nhặt box
    // =============================
    public IEnumerator PickUp( Vector3 targetPosition)
    {

        while (nearbyBox == null)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(-direction);
            rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
            yield return null;
        }
        // 2️⃣ Khi đã nhận được nearbyBox → nhặt box
        if (nearbyBox != null)
        {
            carriedBox = nearbyBox;
            nearbyBox.SetPickedUp(true);

            // 🔹 Tắt collider
            foreach (var c in nearbyBox.GetComponentsInChildren<Collider>())
                c.enabled = false;

            // 🔹 Gắn box lên tay bot
            nearbyBox.transform.SetParent(holdPoint);
            nearbyBox.transform.localPosition = Vector3.zero;
            nearbyBox.transform.localRotation = Quaternion.identity;

            // 🔹 Reset nearbyBox (để tránh trigger lại nhặt)
            nearbyBox = null;
        }
    }

    public IEnumerator DropAt(Vector3 targetPosition)
    {
        if (carriedBox == null)
            yield break;

        while (nearbySlot == null)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(-direction);
            rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
            yield return null;
        }

        // 2️⃣ Khi đã đến slot → thả box xuống
        if (nearbySlot != null)
        {
            carriedBox.SetPickedUp(false);

            foreach (var c in carriedBox.GetComponentsInChildren<Collider>())
                c.enabled = true;

            carriedBox.transform.SetParent(null);
            carriedBox.transform.position = nearbySlot.GetSlotPosition();

            nearbySlot.SetBox(true);
            nearbySlot.boxHighlighter = carriedBox;

            carriedBox = null;
            nearbySlot = null;
        }
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

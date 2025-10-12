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
    public IEnumerator MoveTo(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(-direction);
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {         
            direction = (target - transform.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
            yield return null;
        }
        if(nearbySlot != null)
            nearbySlot.SetBox(false);
    }

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

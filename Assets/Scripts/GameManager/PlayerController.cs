using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerController : MonoSingleton<PlayerController>
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Pickup Settings")]
    public Transform holdPoint;             // Vị trí để cầm box
    public Transform dropPoint;             // Vị trí để thả box
    public KeyCode pickupKey = KeyCode.Space; // Phím nhặt / thả

    private Rigidbody rb;
    private Vector3 moveDir;
    private Vector3 contactNormal;
    private bool isColliding = false;

    private BoxHighlighter nearbyBox;   // Box gần player
    private BoxHighlighter carriedBox;  // Box đang được cầm

    public bool carriedBoxStatus = false;
    public BoxSlot nearbySlot;

    public BoxSlot2 nearbySlot2;

    public bool wingame = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // tránh bị nghiêng
    }

    void Update()
    {
        HandleMovementInput();
        HandlePickupInput();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    // =============================
    // 🎮 Xử lý di chuyển
    // =============================
    void HandleMovementInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(h, 0, v).normalized;

        if (moveDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(-moveDir);
        }
    }

    void MovePlayer()
    {
        if (moveDir == Vector3.zero) return;

        Vector3 finalDir = moveDir;

         if (isColliding)
         {
             // Cho phép trượt dọc tường
             finalDir = Vector3.ProjectOnPlane(moveDir, contactNormal).normalized;
         }
        rb.MovePosition(rb.position + finalDir * moveSpeed * Time.fixedDeltaTime);
    }

    void OnCollisionStay(Collision collision)
    {
        isColliding = true;
        contactNormal = collision.contacts[0].normal;
    }

    void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }

    // =============================
    // 📦 Xử lý nhặt và thả box
    // =============================
    void HandlePickupInput()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            if(wingame) 
            {
                WinGame();
                return;
            }

            if (carriedBox == null && nearbyBox != null)
            {
                PickupBox(nearbyBox);
            }
            else if (carriedBox != null)
            {
                DropBox();
            }
        }
    }
    void WinGame()
    {
        Debug.Log("🎉 YOU WIN!"); // bạn có thể thay bằng UI hoặc animation sau này

        // ⚙️ Nếu bạn có scene quản lý, có thể gọi hàm
        // GameManager.Instance.WinGame(); 
        // hoặc load scene chiến thắng:
        // SceneManager.LoadScene("WinScene");
    }

    void PickupBox(BoxHighlighter box)
    {
        isColliding = false;
        carriedBox = box;
        box.SetPickedUp(true);

        // 🔹 Tắt tất cả collider trong box (cả con)
        foreach (var c in box.GetComponentsInChildren<Collider>())
            c.enabled = false;

        // 🔹 Gắn box lên tay player
        box.transform.SetParent(holdPoint);
        box.transform.localPosition = Vector3.zero;
        box.transform.localRotation = Quaternion.identity;
        if (nearbySlot != null)
        {
            nearbySlot.SetBox(false);
            nearbySlot.boxHighlighter = null;
        }   

        carriedBoxStatus = true;
    }

    // void DropBox()
    // {
    //     isColliding = false;
    //     if (carriedBox == null)
    //         return;

    //     carriedBox.SetPickedUp(false);

    //     // Bật lại collider
    //     foreach (var c in carriedBox.GetComponentsInChildren<Collider>())
    //         c.enabled = true;

    //     // Nếu player đang gần slot → đặt box vào đó
    //     if (nearbySlot != null)
    //     {
    //         carriedBox.transform.SetParent(null);
    //         carriedBox.transform.position = nearbySlot.GetSlotPosition();
    //         nearbySlot.SetBox(true); // thông báo slot đã có box
    //         nearbySlot.boxHighlighter = carriedBox; // liên kết slot với boxÍ
    //     }
    //     else
    //     {
    //         // Thả xuống bình thường
    //         carriedBox.transform.SetParent(null);
    //         carriedBox.transform.position = dropPoint.position;
    //     }

    //     carriedBox = null;
    //     carriedBoxStatus = false;
    // }

    void DropBox()
    {
        if (carriedBox == null)
            return;

        carriedBox.SetPickedUp(false);

        // bật lại collider
        foreach (var c in carriedBox.GetComponentsInChildren<Collider>())
            c.enabled = true;
        if (nearbySlot != null)
        {
            carriedBox.transform.SetParent(null);
            carriedBox.transform.position = nearbySlot.GetSlotPosition();
            nearbySlot.SetBox(true); // thông báo slot đã có box
            nearbySlot.boxHighlighter = carriedBox; // liên kết slot với boxÍ
        }
        // 🔹 Nếu đang gần slot và slot trống → đặt box vào đó
        else if (nearbySlot2 != null && !nearbySlot2.hasBox)
        {
            nearbySlot2.PlaceBox(carriedBox.gameObject);
        }
        else
        {
            // 🔹 Thả xuống bình thường
            carriedBox.transform.SetParent(null);
            carriedBox.transform.position = dropPoint.position;
        }

        carriedBox = null;
        carriedBoxStatus = false;
    }

    // 📍 Liên kết với BoxTriggerZone (giữ nguyên)
    public void SetNearbyBox(BoxHighlighter box)
    {
        nearbyBox = box;
    }

    public void ClearNearbyBox(BoxHighlighter box)
    {
        if (nearbyBox == box)
            nearbyBox = null;
    }
    // =============================
    // 📍 Liên kết với BoxSlot
    // =============================
    public void SetNearbySlot(BoxSlot slot)
    {
        nearbySlot = slot;
        
    }

    public void ClearNearbySlot(BoxSlot slot)
    {
        if (nearbySlot == slot)
            nearbySlot = null;
    }

    // =============================
    // 📍 Liên kết với BoxSlot
    // =============================
    public void SetNearbySlot2(BoxSlot2 slot)
    {
        nearbySlot2 = slot;
        
    }

    public void ClearNearbySlot2(BoxSlot2 slot)
    {
        if (nearbySlot2 == slot)
            nearbySlot2 = null;
    }



}

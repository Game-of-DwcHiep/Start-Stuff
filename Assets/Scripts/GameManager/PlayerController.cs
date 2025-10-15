using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public LevelController2 levelController;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Pickup Settings")]
    public Transform holdPoint;
    public Transform dropPoint;
    public KeyCode pickupKey = KeyCode.Space;

    private Rigidbody rb;
    private Vector3 moveDir;

    private BoxHighlighter nearbyBox;
    private BoxHighlighter carriedBox;

    public bool carriedBoxStatus = false;
    public BoxSlot nearbySlot;
    public BoxSlot2 nearbySlot2;
    public bool wingame = false;

    public int level;

    // ✅ Dùng cho di chuyển bằng UI (mobile)
    private Vector2 uiMoveInput = Vector2.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
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
        // Nếu có input từ UI (mobile) thì dùng nó
        if (uiMoveInput != Vector2.zero)
        {
            moveDir = new Vector3(uiMoveInput.x, 0, uiMoveInput.y).normalized;
        }
        else
        {
            // Nếu không, dùng bàn phím (cho PC)
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            moveDir = new Vector3(h, 0, v).normalized;
        }

        if (moveDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(-moveDir);
    }

    void MovePlayer()
    {

        if (moveDir == Vector3.zero)
        {
            rb.linearVelocity = Vector3.zero; // (Unity 6+ dùng linearVelocity, Unity cũ dùng velocity)
            return;
        }

        Vector3 move = moveDir * moveSpeed;
        move.y = rb.linearVelocity.y; // giữ lực trọng lực nếu có

        rb.linearVelocity = move; // Rigidbody sẽ tự xử lý va chạm
    }

    // 📱 Các hàm cho UI Button gọi
    public void OnMoveButtonDown(string direction)
    {
        switch (direction)
        {
            case "Up": uiMoveInput = Vector2.up; break;
            case "Down": uiMoveInput = Vector2.down; break;
            case "Left": uiMoveInput = Vector2.left; break;
            case "Right": uiMoveInput = Vector2.right; break;
        }
    }

    public void OnMoveButtonUp()
    {
        uiMoveInput = Vector2.zero;
    }

    // =============================
    // 📦 Xử lý nhặt/thả
    // =============================
    void HandlePickupInput()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            TryPickupOrDrop();
        }
    }

    // 📱 Cho UI Button Space gọi (nếu muốn)
    public void OnPickupButton()
    {
        TryPickupOrDrop();
    }

    private void TryPickupOrDrop()
    {
        if (wingame)
        {
            WinGame();
            return;
        }

        if (carriedBox == null && nearbyBox != null)
            PickupBox(nearbyBox);
        else if (carriedBox != null)
            DropBox();
    }

    void WinGame()
    {
        levelController.panelWin.SetActive(false);
        GameManager.Instance.WinGame(level);
        SoundManager.Instance.PlayWinMusic();
    }

    
    void PickupBox(BoxHighlighter box)
    {
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
            carriedBox.transform.SetParent(nearbySlot.transform);
            carriedBox.transform.localPosition = Vector3.zero + Vector3.forward * 0.5f; // nâng lên một chút cho đẹp
            carriedBox.transform.localRotation = Quaternion.identity;
            //carriedBox.transform.position = nearbySlot.GetSlotPosition();
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

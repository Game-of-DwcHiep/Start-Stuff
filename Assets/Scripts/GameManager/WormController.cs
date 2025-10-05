using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WormController : MonoSingleton<WormController>
{
    public float moveDistance = 1f; // Độ dài di chuyển mỗi lần (1 đơn vị)
    public float moveSpeed = 0.2f; // Tốc độ di chuyển giữa 2 điểm

    public Transform targetAll;
    public Transform head;
    public Transform body;
    public Transform tail;

    public Vector2 targetPositionHead;
    public Vector2 targetPositionBody;
    public Vector2 targetPositionTail;

    [Header("spriteRenderer cac vi tri tren player")]
    public SpriteRenderer spriteRendererHead;
    public SpriteRenderer spriteRendererBody;
    public SpriteRenderer spriteRendererTail;

    [Header("sprite cac vi tri tren than khi di chuyen")]

    [SerializeField]
    private Sprite _spHead, _spHeadEat;

    [SerializeField]
    private Sprite _spBody, _spBody2, _spBody3;

    private Vector2 lastDirection = Vector2.right; // Hướng di chuyển trước 1 bước

    private Vector2 previousDirection = Vector2.right;// hướng di chuyển trước 2 bước

    private Vector2 aroundPlayer = Vector2.right;// hướng hiện tại của player trái hay phải
    //private Vector2 aroundPlayer = Vector2.right;// hướng hiện tại của player 

    private float timeElapsed = 0f;

    // Check va chạm mặt đất
    public LayerMask groundLayer; // tất cả các layer player có thể di chuyển trên và không bị rơi

    public LayerMask notEatLayer;// tất cả các layer player bị chặn và không ăn được

    public float groundCheckDistance = 0.5f; // Khoảng cách kiểm tra mặt đất
    public float fallSpeed = 5f; // Tốc độ rơi

    public bool isGrounded; // Trạng thái đứng trên mặt đất

    [Header("sử dụng phím tắt để chơi game trên editer")]
    [SerializeField]
    private bool isGetKeyDown = true;

    [Header("cho phép sâu quay đầu")]
    [SerializeField]
    private bool isTurnMove = true;

    [Header("bàn tay thả sâu")]
    public GameObject handTarget;
    public GameObject handBat;//tay bat
    public GameObject handTha;//tay tha

    [SerializeField]
    private bool isMoving = false; // Cờ trạng thái để kiểm tra di chuyển

    // Bổ sung cho input từ button
    private bool isButtonPressed = false;
    private Vector2 buttonDirection = Vector2.zero;

    void Update()
    {
        HandleInput();
        HandleButtonInput();
        UpdateGroundedState();
    }
    private void HandleInput()
    {
        if (isGetKeyDown)
        {
            if (Input.GetKey(KeyCode.UpArrow))
                AttemptMove(Vector2.up, Vector2.down);
            else if (Input.GetKey(KeyCode.DownArrow))
                AttemptMove(Vector2.down, Vector2.up);
            else if (Input.GetKey(KeyCode.LeftArrow))
                AttemptMove(Vector2.left, Vector2.right);
            else if (Input.GetKey(KeyCode.RightArrow))
                AttemptMove(Vector2.right, Vector2.left);
        }
    }

    private void HandleButtonInput()
    {
        if (isButtonPressed)
        {
            AttemptMove(buttonDirection, -buttonDirection);
        }
    }

    public void OnButtonDown(Vector2 direction)
    {
        isButtonPressed = true;
        buttonDirection = direction;
    }

    public void OnButtonUp()
    {
        isButtonPressed = false;
    }
    public void AttemptMove(Vector2 direction, Vector2 oppositeDirection)
    {
        if (!isGrounded || !GameManager.Instance.isMoving || isMoving)
        {
            return;
        }    
        Vector2 headRayOrigin = head.position;
        Vector2 tailRayOrigin = tail.position;

        // Kiểm tra va chạm theo hướng
        RaycastHit2D hit = Physics2D.Raycast(headRayOrigin, direction, moveDistance, notEatLayer);
        RaycastHit2D hitTail = Physics2D.Raycast(tailRayOrigin, Vector2.down, moveDistance, notEatLayer);

        if (hit.collider != null)
        {
            Debug.LogError($"Không thể di chuyển {direction}, phía trước có vật cản: {hit.collider.name}");
            return;
        }
        if (lastDirection != oppositeDirection )
        {
            SetTargetPositions(direction);
            SetTargetRotation(direction);
            StartCoroutine(MoveCoroutine());
            isMoving = true; // Đặt trạng thái đang di chuyển
            previousDirection = lastDirection;// cập nhật hướng di chuyển trước 2 bước
            lastDirection = direction; // Cập nhật hướng di chuyển cuối cùng
        }

        if (isTurnMove && direction != lastDirection && lastDirection == previousDirection )
        {
            if (hitTail.collider != null && previousDirection == Vector2.up && !isMoving)
            {
                Debug.LogError("Không thể quay đầu vì đuôi đang chạm đất!");
                return;
            }
            TurnAround(direction);
        }    
    }
    private void TurnAround(Vector2 direction)// quay đầu player
    {
        // Lưu vị trí hiện tại của đầu, thân và đuôi
        Vector2 currentHeadPosition = head.position;
        Vector2 currentBodyPosition = body.position;
        Vector2 currentTailPosition = tail.position;

        // Đổi vị trí đầu và đuôi
        targetPositionHead = currentTailPosition;
        targetPositionTail = currentHeadPosition;
        targetPositionBody = currentBodyPosition;

        // Cập nhật vị trí targetAll để định hướng lại sâu
        targetAll.position = currentTailPosition;

        lastDirection = direction;
        // Cập nhật các vị trí trên scene
        head.position = targetPositionHead;
        body.position = targetPositionBody;
        tail.position = targetPositionTail;
        //Debug.LogError("Turned Around: Head -> Tail, Tail -> Head");
        //return;
    }    
    private IEnumerator MoveCoroutine()
    {
        timeElapsed = 0f; // Đặt lại thời gian khi bắt đầu di chuyển

        Vector2 startHeadPosition = targetAll.position;
        Vector2 startBodyPosition = body.position;
        Vector2 startTailPosition = tail.position;

        while (timeElapsed < moveSpeed)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / moveSpeed;
            float _tail = timeElapsed / moveSpeed * 4;// smooth rieng cho đuôi

            // Áp dụng easing để mượt hơn
            float smoothT = Mathf.SmoothStep(0, 1, t);
            float smoothTail = Mathf.SmoothStep(0, 1, _tail);

            // Cập nhật vị trí với easing
            targetAll.position = Vector2.Lerp(startHeadPosition, targetPositionHead, smoothT);
            body.position = Vector2.Lerp(startBodyPosition, targetPositionBody, smoothT);
            tail.position = Vector2.Lerp(startTailPosition, targetPositionTail, smoothTail);


            yield return null; // Chờ khung hình tiếp theo
        }
        // Đảm bảo các vị trí được đặt chính xác sau khi hoàn tất di chuyển
        targetAll.transform.position = RoundVector2(targetPositionHead);
        body.transform.position = RoundVector2(targetPositionBody);
        tail.transform.position = RoundVector2(targetPositionTail);

        isMoving = false; // Hoàn tất di chuyển, cho phép di chuyển mới
        //UpdateGroundedState();
        OnMovementComplete();
    }
    private void SetTargetRotation(Vector2 direction)
    {
        if (direction == Vector2.right) // Di chuyển sang phải
        {
            aroundPlayer = Vector2.right;
            head.rotation = Quaternion.Euler(0, 0, 0);
            if (lastDirection == Vector2.up)
            {
                ChangeSprite(spriteRendererBody, _spBody2);
                body.rotation = Quaternion.Euler(0, 0, 0);
                tail.rotation = Quaternion.Euler(0, 0, 90);

            }
            else if (lastDirection == Vector2.down)
            {
                ChangeSprite(spriteRendererBody, _spBody3);
                body.rotation = Quaternion.Euler(0, 0, -90);
                tail.rotation = Quaternion.Euler(0, 0, -90);
            }
            else if (lastDirection == Vector2.right)
            {
                ChangeSprite(spriteRendererBody, _spBody);
                body.rotation = Quaternion.Euler(0, 0, 0);
                tail.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else if (direction == Vector2.left) // Di chuyển sang trái
        {
            aroundPlayer = Vector2.left;
            head.rotation = Quaternion.Euler(0, 180, 0);
            if (lastDirection == Vector2.up)
            {
                ChangeSprite(spriteRendererBody, _spBody2);
                body.rotation = Quaternion.Euler(0, 0, -90);
                tail.rotation = Quaternion.Euler(0, 180, 90);
            }
            else if (lastDirection == Vector2.down)
            {
                ChangeSprite(spriteRendererBody, _spBody3);
                body.rotation = Quaternion.Euler(0, 180, -90);
                tail.rotation = Quaternion.Euler(0, 180, -90);
            }
            else if (lastDirection == Vector2.left)
            {
                ChangeSprite(spriteRendererBody, _spBody);
                body.rotation = Quaternion.Euler(0, 180, 0);
                tail.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        else if (direction == Vector2.up)// di chuyển lên
        {
            if (lastDirection == Vector2.right)
            {
                aroundPlayer = Vector2.right;
                ChangeSprite(spriteRendererBody, _spBody3);
                head.rotation = Quaternion.Euler(0, 0, 90);
                body.rotation = Quaternion.Euler(0, 0, 0);
                tail.rotation = Quaternion.Euler(0, 0, 0);
                previousDirection = lastDirection;
            }
            else if (lastDirection == Vector2.left)
            {
                aroundPlayer = Vector2.left;
                ChangeSprite(spriteRendererBody, _spBody3);
                head.rotation = Quaternion.Euler(0, 180, 90);
                body.rotation = Quaternion.Euler(0, 0, -90);
                tail.rotation = Quaternion.Euler(0, 180, 0);
                previousDirection = lastDirection;
            }
            else if (lastDirection == Vector2.up)
            {
                ChangeSprite(spriteRendererBody, _spBody);
                if (previousDirection == Vector2.right)
                {
                    body.rotation = Quaternion.Euler(0, 0, 90);
                    tail.rotation = Quaternion.Euler(0, 0, 90);
                    return;
                }
                else if (previousDirection == Vector2.left)
                {
                    body.rotation = Quaternion.Euler(0, 180, 90);
                    tail.rotation = Quaternion.Euler(0, 180, 90);
                    return;
                }
                else if(previousDirection == Vector2.down)
                {
                    if (aroundPlayer == Vector2.right)
                    {
                        head.rotation = Quaternion.Euler(0, 0, 90);
                        body.rotation = Quaternion.Euler(0, 0, 90);
                        tail.rotation = Quaternion.Euler(0, 0, 90);
                    }
                    else
                    {
                        head.rotation = Quaternion.Euler(0, 180, 90);
                        body.rotation = Quaternion.Euler(0, 180, 90);
                        tail.rotation = Quaternion.Euler(0, 180, 90);
                    }
                }
                previousDirection = lastDirection;
            }
        }
        else if (direction == Vector2.down)// di chuyển xuống
        {
            if (lastDirection == Vector2.right)
            {
                aroundPlayer = Vector2.left;
                ChangeSprite(spriteRendererBody, _spBody2);
                head.rotation = Quaternion.Euler(0, 0, -90);
                body.rotation = Quaternion.Euler(0, 0, -90);
                tail.rotation = Quaternion.Euler(0, 0, 0);
                previousDirection = lastDirection;
            }
            else if (lastDirection == Vector2.left)
            {
                aroundPlayer = Vector2.right;
                ChangeSprite(spriteRendererBody, _spBody2);
                head.rotation = Quaternion.Euler(0, 180, -90);
                body.rotation = Quaternion.Euler(0, 0, 0);
                tail.rotation = Quaternion.Euler(0, 180, 0);
                previousDirection = lastDirection;
            }
            else if (lastDirection == Vector2.down )
            {
                ChangeSprite(spriteRendererBody, _spBody);
                if (previousDirection == Vector2.right)
                {
                    body.rotation = Quaternion.Euler(0, 0, -90);
                    tail.rotation = Quaternion.Euler(0, 0, -90);
                    return;
                }
                else if (previousDirection == Vector2.left)
                {
                    body.rotation = Quaternion.Euler(0, 180, -90);
                    tail.rotation = Quaternion.Euler(0, 180, -90);
                    return;
                }
                else if (previousDirection == Vector2.up)
                {
                    if (aroundPlayer == Vector2.right)
                    {
                        head.rotation = Quaternion.Euler(0, 180, -90);
                        body.rotation = Quaternion.Euler(0, 180, -90);
                        tail.rotation = Quaternion.Euler(0, 180, -90);
                    }
                    else
                    {
                        head.rotation = Quaternion.Euler(0, 0, -90);
                        body.rotation = Quaternion.Euler(0, 0, -90);
                        tail.rotation = Quaternion.Euler(0, 0, -90);
                    }                       
                }   
                previousDirection = lastDirection;
            }
        }
    }
    private void ChangeSprite(SpriteRenderer spriteRenderer, Sprite newSprite)
    {
        if (spriteRenderer.sprite != newSprite)
        {
            spriteRenderer.sprite = newSprite;
        }
    }  
    private void SetTargetPositions(Vector2 direction)
    {
        SoundManager.Instance.Play("moving");
        targetPositionHead = (Vector2)targetAll.position + direction * moveDistance;
        targetPositionBody = targetAll.position;
        targetPositionTail = body.position;
        timeElapsed = 0f; // Đặt lại thời gian khi bắt đầu di chuyển
        CheckDropBox(direction);
    }
    private void CheckDropBox(Vector2 direction)
    {
        // Kiểm tra nếu đầu con sâu chạm vào ô gỗ
        Vector2 targetGridPosition = (Vector2)targetAll.position + direction * moveDistance;
        int row = Mathf.RoundToInt(-targetGridPosition.y); // Chuyển từ tọa độ sang hàng
        int col = Mathf.RoundToInt(targetGridPosition.x); // Chuyển từ tọa độ sang cột
        var l = GameManager.Instance;
        if (row >= 0 && row < l.grid.GetLength(0) && col >= 0 && col < l.grid.GetLength(1))
        {
            if (l.grid[row, col] == 1 || l.grid[row, col] == 2) // Nếu ô gỗ tồn tại
            {
                Destroy(l.cellObjects[row, col]);
                l.cellObjects[row, col] = null;
                l.grid[row, col] = 0;
                l.CheckLayerBot(row + 1, col);
                l.CheckLayerRight(row, col + 1);
                SoundManager.Instance.Play("eat");
                StartCoroutine(ResetHeadSprite(spriteRendererHead));              
            }  
            else if(l.grid[row, col] == 3)
            {
                Debug.LogError("k an được box này");
            }    
        }
        if (!l.IsAnyBoxStillExists())
        {
            l.isMoving = false;
            l.LoseGame();
        }   
    }
    private IEnumerator ResetHeadSprite(SpriteRenderer headSpriteRenderer)
    {
        headSpriteRenderer.sprite = _spHeadEat;
        yield return new WaitForSeconds(0.2f); // Chờ 0.2 giây hoặc tùy chỉnh thời gian
        headSpriteRenderer.sprite = _spHead;
    }
    private void OnMovementComplete()// đã di chuyển thành công
    {
        GameManager.Instance.HandleDisconnectedPiecesAdvanced();
        GameManager.Instance.AreGridsMatching();
    }  
    public bool IsWormAtPosition(Vector2 position)
    {
        // Kiểm tra nếu đầu, thân, hoặc đuôi của con sâu đang ở vị trí này
        return (Vector2)head.position == position || (Vector2)body.position == position || (Vector2)tail.position == position;
    }
    private Vector2 RoundVector2(Vector2 vector)
    {
        return new Vector2(Mathf.Round(vector.x),Mathf.Round(vector.y));
    }
    // kiem tra cham dat
    private bool IsGroundedAt(Vector2 position, Vector2 direction)
    {
        return Physics2D.Raycast(position, direction, groundCheckDistance, groundLayer);
    }

    private bool IsHeadGrounded()
    {
        if (head == null)
        {
            Debug.LogError("Head chưa được gán Transform!");
            return false;
        }
        return IsGroundedAt(head.position, Vector2.down);
    }

    private bool IsBodyGrounded()
    {
        if (body == null)
        {
            Debug.LogError("Body chưa được gán Transform!");
            return false;
        }
        return IsGroundedAt(body.position, Vector2.down);
    }

    private bool IsTailGrounded()
    {
        if (tail == null)
        {
            Debug.LogError("Tail chưa được gán Transform!");
            return false;
        }
        return IsGroundedAt(tail.position, Vector2.down);
    }

    private bool IsHeadSideGrounded()
    {
        return IsGroundedAt(head.position, Vector2.left) || IsGroundedAt(head.position, Vector2.right);
    }

    private bool IsBodySideGrounded()
    {
        return IsGroundedAt(body.position, Vector2.left) || IsGroundedAt(body.position, Vector2.right);
    }

    private void DebugRay(Vector2 origin, Vector2 direction, bool isHit)
    {
        Debug.DrawLine(origin, origin + direction * groundCheckDistance, isHit ? Color.green : Color.red);
    }

    private void UpdateGroundedState()
    {
        // Kiểm tra trạng thái chạm đất
        bool headGrounded = IsHeadGrounded();
        bool bodyGrounded = IsBodyGrounded();
        bool tailGrounded = IsTailGrounded();

        // Kiểm tra trạng thái bên trái và bên phải
        bool headSideGrounded = IsHeadSideGrounded();
        bool bodySideGrounded = IsBodySideGrounded();

        // Tính toán trạng thái tổng thể
        isGrounded = headGrounded || bodyGrounded || tailGrounded || (headSideGrounded && lastDirection == Vector2.up) || bodySideGrounded;
        //Debug.LogError(isGrounded);
        // Debug hiển thị raycast
        DebugRay(head.position, Vector2.down, headGrounded);
        DebugRay(body.position, Vector2.down, bodyGrounded);
        DebugRay(tail.position, Vector2.down, tailGrounded);
        DebugRay(head.position, Vector2.left, IsGroundedAt(head.position, Vector2.left));
        DebugRay(head.position, Vector2.right, IsGroundedAt(head.position, Vector2.right));
        DebugRay(body.position, Vector2.left, IsGroundedAt(body.position, Vector2.left));
        DebugRay(body.position, Vector2.right, IsGroundedAt(body.position, Vector2.right));

        // Nếu không có mặt đất, xử lý rơi
        if (!isGrounded)
        {
            HandleFall();
            return;
        }
        targetAll.position = RoundVector2(targetAll.position);
    }

    private void HandleFall()
    {
        targetAll.position += (Vector3)(Vector2.down * fallSpeed * Time.deltaTime);
        if (targetAll.position.y <= -10) // Kiểm tra nếu vị trí y <= -10
        {
            //SoundManager.Instance.Play("drop");
            GameManager.Instance.LoseGame();
            Destroy(targetAll.gameObject); // Hủy đối tượng
            return;
        }
        StartCoroutine(CheckDropBox());
    }
    private IEnumerator CheckDropBox()
    {
        yield return new WaitForSeconds(0.3f);
        GameManager.Instance.HandleDisconnectedPiecesAdvanced();
    }    
}

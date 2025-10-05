using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PopupComplete : Popup
{
    public Text level;
    public int levelComplete;

    public Text txtCoin;
    public int coin = 10;

    public Text txtCoinBonus;

    public Button btnReward;

    public RectTransform targetObject; // Đối tượng cần xoay
    public float rotationSpeed = 200f; // Tốc độ xoay (độ mỗi giây)

    private float currentAngle = 0f;   // Góc hiện tại (0 ~ -180)
    private float targetAngle = -180f; // Góc mục tiêu (-180 hoặc 0)
    private bool rotatingToNegative = true; // Trạng thái xoay: true = xoay đến -180, false = xoay về 0
    private bool isRotating = true; // Trạng thái cho phép xoay

    [Space(10f)]
    public GameObject objectPrefab;
    public Transform target;
    public Transform parent = null;
    public Transform parent2;
    public int objectCount;
    public float spawnRadius;

    public float scaleDuration = 1f;
    public float minMoveDuration = 0.5f;
    public float maxMoveDuration = 2f;

    private List<Vector3> spawnPositions = new List<Vector3>();
    private void OnEnable()
    {
        levelComplete = UIManager.Instance.levelCurrent;
        level.text = "LEVEL " + levelComplete.ToString();
        coin = 10;
        txtCoin.text = "+ " + coin.ToString();
        btnReward.gameObject.SetActive(true);
        isRotating = true;
        WinLevel();
    }

    private void OnDestroy()
    {
        btnReward.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (targetObject != null && isRotating)
        {
            RotateBackAndForthLogic();
        }
    }

    private void WinLevel()
    {
        GameManager.Instance.GiveCoins(coin);
        GameManager.Instance.Save();
        SpawnObjects();
        //ChangeTextCoins();
        //UIManager.Instance.UpDateTextCoin();
    }    

    private void RotateBackAndForthLogic()
    {
        // Xác định hướng xoay dựa trên trạng thái
        float step = rotationSpeed * Time.deltaTime * (rotatingToNegative ? -1 : 1);

        // Cập nhật góc hiện tại
        currentAngle += step;

        // Giới hạn góc trong khoảng [0, -180]
        currentAngle = Mathf.Clamp(currentAngle, -180f, 0f);

        // Áp dụng góc xoay cho đối tượng
        targetObject.localRotation = Quaternion.Euler(0, 0, currentAngle);

        // Khi đạt góc mục tiêu, đổi trạng thái xoay
        if (Mathf.Approximately(currentAngle, targetAngle))
        {
            rotatingToNegative = !rotatingToNegative; // Đổi hướng xoay
            targetAngle = rotatingToNegative ? -180f : 0f; // Cập nhật góc mục tiêu
        }
        UpdateCoinBasedOnAngle();
    }
    private void UpdateCoinBasedOnAngle()
    {
        // Kiểm tra khoảng góc và thay đổi coin
        if (currentAngle <= 0 && currentAngle > -33)
        {
            // X2 coin nếu góc từ 0 đến -33
            coin = 10 * 2; // hoặc bất kỳ giá trị khởi tạo nào bạn muốn
            txtCoinBonus.text = "+ " + coin.ToString();
        }
        else if (currentAngle <= -33 && currentAngle > -94)
        {
            // X3 coin nếu góc từ -33 đến -94
            coin = 10 * 3; // hoặc giá trị khởi tạo nào khác
            txtCoinBonus.text = "+ " + coin.ToString();
        }
        else if (currentAngle <= -94 && currentAngle > -146)
        {
            // X3 coin nếu góc từ -33 đến -94
            coin = 10 * 4; // hoặc giá trị khởi tạo nào khác
            txtCoinBonus.text = "+ " + coin.ToString();
        }
        else 
        {
            // X3 coin nếu góc từ -33 đến -94
            coin = 10 * 5; // hoặc giá trị khởi tạo nào khác
            txtCoinBonus.text = "+ " + coin.ToString();
        }
    }

    public void ButtonRewardADS()
    {      
        Action SuccessEvent = () =>
        {
            isRotating = false;
            btnReward.gameObject.SetActive(false);
            GameManager.Instance.GiveCoins(coin);
            GameManager.Instance.Save();
            UIManager.Instance.UpDateTextCoin();
        };
        Action Failed = () =>
        {
            Debug.LogError("faild");
        };
    }   
    public void NextLevel()
    {
        UIManager.Instance.levelCurrent ++;
        Hide(false);
        UIManager.Instance.OnNextLevel();
    }

    /*public void ChangeTextCoins()
    {
        DOVirtual.Float(coin, GameManager.Instance.Coins, 2f, v => UIManager.Instance.textCoin.text = v.ToString("0")).SetDelay(0.5f);
    }  */  
    public void SpawnObjects()
    {
        spawnPositions.Clear();
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 spawnPosition = GetRandomPositionInCircle(spawnRadius);
            spawnPositions.Add(spawnPosition);

            GameObject obj = Instantiate(objectPrefab, spawnPosition, Quaternion.identity, parent);
            obj.transform.localScale = Vector3.zero;
            ScaleAndMoveObject(obj);
        }
    }
    private Vector3 GetRandomPositionInCircle(float radius)
    {
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);
        float distance = UnityEngine.Random.Range(0f, radius);
        float x = Mathf.Cos(angle) * distance;
        float y = Mathf.Sin(angle) * distance;

        return parent.TransformPoint(new Vector3(x, y, 0));
    }

    private void ScaleAndMoveObject(GameObject obj)
    {
        float moveDuration = UnityEngine.Random.Range(minMoveDuration, maxMoveDuration);
        obj.transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBounce).OnComplete(() => {
            obj.transform.SetParent(parent2);
            obj.transform.DOMove(target.position, moveDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => Destroy(obj));
                UIManager.Instance.UpDateTextCoin();
        });
    }
}

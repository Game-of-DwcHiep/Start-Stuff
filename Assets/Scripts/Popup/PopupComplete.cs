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
    }

    private void OnDestroy()
    {
        btnReward.gameObject.SetActive(false);
    }
   
    public void NextLevel()
    {
        UIManager.Instance.levelCurrent ++;
        Hide(false);
        UIManager.Instance.OnNextLevel();
    }
}

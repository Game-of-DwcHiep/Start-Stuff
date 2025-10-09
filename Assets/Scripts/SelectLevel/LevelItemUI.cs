using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelItemUI : MonoBehaviour
{
    public int level;
    public Image target;
    public Sprite levelDone;
    public Sprite levelCurrent;
    public Sprite levelLock;

    public Text numberLevel;

   /* public void UpdateUIItem(int level)
    {
        this.level = level;
        if(level  < GameManager.Instance.LevelNumber)
        {
            target.sprite = levelDone;
            numberLevel.text = level.ToString();
        }  
        else if(level == GameManager.Instance.LevelNumber)
        {
            target.sprite = levelCurrent;
            numberLevel.text = level .ToString();
        }  
        else
        {
            target.sprite = levelLock;
            numberLevel.gameObject.SetActive(false);
        }    
    }  
    public void OnPlayGameSelectLevel()
    {
        if(level <= GameManager.Instance.LevelNumber)
        {
            UIManager.Instance.SelectLevel(this.level);
        }
        else
        {
            Debug.LogError("Level hien tai la " + GameManager.Instance.LevelNumber);
        }
    }*/
}

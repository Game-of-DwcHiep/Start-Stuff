using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupLose : Popup
{
    public Text level;

    public int levelLose;

    private void OnEnable()
    {
        levelLose = UIManager.Instance.levelCurrent ;
        level.text = "LEVEL " + levelLose.ToString();
    }

    public void RePlay()
    {
        Hide(false);
        UIManager.Instance.RePlayGame();
    }
}

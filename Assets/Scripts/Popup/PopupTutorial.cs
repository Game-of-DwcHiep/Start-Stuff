using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupTutorial : Popup
{
    private int number = 1;
    public GameObject help1;
    public GameObject help2;

    //public Text text;

    private void OnEnable()
    {
        InitPopup();
    }

    private void InitPopup()
    {
        number = 1;
        //text.text = "Use it to move the worm!";
        help1.SetActive(true);
        help2.SetActive(false);

    }

    public void NextHelp()
    {
        if(number == 2)
        {
            Hide(true);
        }    
        number++;
        help1.SetActive(false);
        help2.SetActive(true);
    }    
}

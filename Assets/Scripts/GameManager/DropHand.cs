using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropHand : MonoBehaviour
{
    public GameObject targetHand;

    public GameObject handBat;

    public GameObject handTha;

    public void ShowHand()
    {
        targetHand.SetActive(true);
    }    

    public void ThaTay()
    {
        handBat.SetActive(false);
        handTha.SetActive(true);
    }  
    public void CloseHand()
    {
        targetHand.SetActive(false);
    }    
}

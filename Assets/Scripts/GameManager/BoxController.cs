using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    private int row;
    private int col;

    public GameObject layerTop;

    public GameObject layerLeft;
    public void Initialize(int row, int col)
    {
        this.row = row;
        this.col = col;
        SetLayerCreateBox();
    }

    private void SetLayerCreateBox()
    {
        if(row == 0 )        
            layerTop.SetActive(true);      
        if(col == 0 )
            layerLeft.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScreen : DwcHyep.Screen
{
    public LevelItemUI itemUI;

    public Transform groupItem;

    private void OnEnable()
    {         
        UpdateUIScreen();
    }
    public void UpdateUIScreen()
    {
        
        foreach (Transform child in groupItem)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i <= GameManager.Instance.listDataLevel.Length; i++)
        {
            GameObject cell = Instantiate(itemUI.gameObject, groupItem);
            cell.name = $"Level_{i + 1}";
            LevelItemUI itemSkinShop = cell.GetComponent<LevelItemUI>();
            itemSkinShop.UpdateUIItem(i + 1);
        }
    }
}

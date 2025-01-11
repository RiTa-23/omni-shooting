using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    //プレイヤーが現在所持しているアイテムをリスト管理

    // Start is called before the first frame update
    public Dictionary<Item.ItemName,int> ItemList;
    void Start()
    {
        ItemList = new Dictionary<Item.ItemName, int>();
        //ItemList.Add(Item.ItemName.speedUp, 0);
        //ItemList.Add(Item.ItemName.maxSpeedUp, 0);
        ItemList.Add(Item.ItemName.maxEnergyUp, 0);
        ItemList.Add(Item.ItemName.energySpeedUp, 0);
        ItemList.Add(Item.ItemName.rapidFireUp, 0);
    }

    // Update is called once per frame
    public void AddItem(Item.ItemName itemName)
    {
        ItemList[itemName]++;
    }

    public void ResetItemList()
    {
        //ItemList[Item.ItemName.speedUp] = 0;
        //ItemList[Item.ItemName.maxSpeedUp] = 0;
        ItemList[Item.ItemName.maxEnergyUp] = 0;
        ItemList[Item.ItemName.energySpeedUp] = 0;
        ItemList[Item.ItemName.rapidFireUp] = 0;
    }
}

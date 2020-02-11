using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{

    private List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();                
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);

        switch (item.itemType)
        {
            case ItemType.Crown:
                GameManager.GetInstance().SetWorldState("CrownObtained", true);
                break;
        }
    }

    public bool HasItem(ItemType itemType)
    {
        foreach(Item item in itemList)
        {
            if (item.itemType == itemType)
                return true;
        }
        return false;
    }

    public int NumItems()
    {
        return itemList.Count;
    }

}

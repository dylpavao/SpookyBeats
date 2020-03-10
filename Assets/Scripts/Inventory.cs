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

    public List<Item> ItemList()
    {
        return itemList;
    }

    public string[] GetPage(int pg, int numItemPerPage)
    {
        string[] page = new string[numItemPerPage];
        for(int i = 0; i < page.Length; i++)
        {
            if (i < itemList.Count)
                page[i] = itemList[i].itemType.ToString();
            else
                page[i] = "";
        }
        return page;
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

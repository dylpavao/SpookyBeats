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

    public void RemoveItem(ItemType itemToRemove)
    {               
        itemList.Remove(GetItem(itemToRemove));
    }

    public void AddItem(Item item)
    {               
        switch (item.itemType)
        {
            case ItemType.Crown:
                GameManager.GetInstance().SetWorldState("CrownObtained", true);
                break;
            case ItemType.Apple:
                GameManager.GetInstance().SetWorldState("AppleObtained", true);
                break;
            case ItemType.RedKey:
                GameManager.GetInstance().SetWorldState("RedKeyObtained", true);
                break;
            case ItemType.BlueKey:
                GameManager.GetInstance().SetWorldState("BlueKeyObtained", true);                
                break;
            case ItemType.YellowKey:
                GameManager.GetInstance().SetWorldState("YellowKeyObtained", true);                
                break;
            case ItemType.Garlic:
                GameManager.GetInstance().SetWorldState("GarlicObtained", true);
                break;
        }

        itemList.Add(item);
        ArrayList message = new ArrayList { "Doug obtained a " + item.itemName + "!", "Doug put the " + item.itemName + " in his inventory." };
        
        if (HasItem(ItemType.RedKey) && HasItem(ItemType.BlueKey) && HasItem(ItemType.YellowKey)) //Create Mega Key
        {            
            RemoveItem(ItemType.RedKey);
            RemoveItem(ItemType.BlueKey);
            RemoveItem(ItemType.YellowKey);
            message.Add("The Keys begin to glow...");
            message.Add("It looks as if they are becoming one.");
            message.Add("Doug obtained the Mega Key!");
            message.Add("Doug put the Mega Key in his inventory.");
            itemList.Add(new Item { itemType = ItemType.MegaKey, amount = 1, itemName = "Mega Key" });
        }
           
        Dialogue itemDlog = new Dialogue { sentences = (string[])message.ToArray(typeof(string)) };
        GameObject.Find("UI_Assistant").GetComponent<UI_Assistant>().StartDialogue(itemDlog, UI_Assistant.DialogueType.Item);                       
    }

    public List<Item> ItemList()
    {
        return itemList;
    }

    public string[] GetPage(int pg, int numItemPerPage)
    {
        string[] page = new string[numItemPerPage];
        for(int i = 0; i < numItemPerPage; i++)
        {
            if (i < itemList.Count)
            {
                page[i] = itemList[i].itemName;                
            }                
            else
            {
                page[i] = "";                
            }                
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

    public Item GetItem(ItemType itemType)
    {
        Item it = null;
        foreach (Item item in itemList)
        {
            if (item.itemType == itemType)
                return item;
        }
        return it;
    }

    public int NumItems()
    {
        return itemList.Count;
    }    

}

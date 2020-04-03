using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{

    private List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();
        //AddItem(new Item { itemType = ItemType.RedKey, amount = 1, itemName = "Red Key" });
        //AddItem(new Item { itemType = ItemType.BlueKey, amount = 1, itemName = "Blue Key" });
    }

    public void RemoveItem(ItemType itemToRemove)
    {        
        //string[] test = new string[] { "The " + GetItem(itemToRemove).itemName + " was removed from Doug's inventory."};
        //Dialogue d = new Dialogue { sentences = test };
        //GameObject.Find("UI_Assistant").GetComponent<UI_Assistant>().StartDialogue(d, UI_Assistant.DialogueType.Default);
        itemList.Remove(GetItem(itemToRemove));
    }

    public void AddItem(Item item)
    {       
        itemList.Add(item);

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

        // spawn dialogue
        string[] test = new string[] { "Doug obtained a "+item.itemName+"!", "Doug put the "+item.itemName+" in his inventory."};
        Dialogue d = new Dialogue { sentences = test };
        GameObject.Find("UI_Assistant").GetComponent<UI_Assistant>().StartDialogue(d, UI_Assistant.DialogueType.Item);        
        

        //CheckKeys();
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

    public void CheckKeys()
    {        
        if(HasItem(ItemType.RedKey) && HasItem(ItemType.BlueKey) && HasItem(ItemType.YellowKey))
        {
            Debug.Log("Mega Key");
            RemoveItem(ItemType.RedKey);
            RemoveItem(ItemType.BlueKey);
            RemoveItem(ItemType.YellowKey);
            AddItem(new Item { itemType = ItemType.MegaKey, amount = 1, itemName = "Mega Key"});
            //string[] test = new string[] { "The Mega Key was created!", "The Mega Key was added to Doug's inventory." };
            //Dialogue d = new Dialogue { sentences = test };
            //GameObject.Find("UI_Assistant").GetComponent<UI_Assistant>().StartDialogue(d);
        }
    }

}

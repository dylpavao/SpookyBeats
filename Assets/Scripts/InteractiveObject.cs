using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
        
    [SerializeField] private bool isItem;
    [SerializeField] private ItemType neededItem = ItemType.Null;
    [SerializeField] private ItemType givesItem = ItemType.Null;
    [SerializeField] private Dialogue dialogue1;
    [SerializeField] private Dialogue dialogue2;
    [SerializeField] private Dialogue dialogue3;
    private Item item = null;
    private bool test = false;
    private bool hasRequiredItem;        



    private void Start()
    {        
        hasRequiredItem = false;
        if(givesItem != ItemType.Null)
        {
            string name = givesItem.ToString();
            switch (givesItem)
            {
                case ItemType.YellowKey:
                    name = "Yellow Key";
                    break;
                case ItemType.BlueKey:
                    name = "Blue Key";
                    break;
                case ItemType.RedKey:
                    name = "Red Key";
                    break;
            }            

            item = new Item { itemType = givesItem, amount = 1, itemName = name};
        }
            
    }

    public void TriggerDialogue()
    {        
        if (test)
        {
            Dialogue d = new Dialogue { sentences = dialogue2.GetSentences()};
            d.Append(dialogue3.GetSentences());
            FindObjectOfType<UI_Assistant>().StartDialogue(d);
            test = false;
        }
        else if(hasRequiredItem)
            FindObjectOfType<UI_Assistant>().StartDialogue(dialogue2);
        else
            FindObjectOfType<UI_Assistant>().StartDialogue(dialogue1);
    }

    public void Unlock()
    {        
        hasRequiredItem = true;

        if (gameObject.name == "QueenKazoo")
        {
            GameManager.GetInstance().SetWorldState("GateOpen", true);
        }
    }

    public void Fuck(bool ya)
    {
        if (ya && test)
            test = false;
        else 
            test = ya;
    }

    public Item GetItem()
    {
        return item;
    }

    public bool GivesItem()
    {
        return givesItem != ItemType.Null;
    }

    public ItemType NeededItem()
    {
        return neededItem;
    }

    public ItemType GivenItem()
    {
        return givesItem;
    }
    
    public bool NeedsItem()
    {      
        return neededItem != ItemType.Null;
    }

    public bool HasRequiredItem()
    {
        return hasRequiredItem;
    }

    public bool IsItem()
    {
        return isItem;
    }

}

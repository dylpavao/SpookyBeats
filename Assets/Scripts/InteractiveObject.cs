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
    private bool firstTrigger = true;
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
        Dialogue dialogueToTrigger;
        if (hasRequiredItem)
        {
            if (firstTrigger)
            {
                dialogueToTrigger = new Dialogue { sentences = dialogue2.GetSentences() };
                dialogueToTrigger.Append(dialogue3.GetSentences());
                firstTrigger = false;
            }
            else
            {
                dialogueToTrigger = dialogue2;
            }                      
        }
        else
        {
            dialogueToTrigger = dialogue1;
        }
        FindObjectOfType<UI_Assistant>().StartDialogue(dialogueToTrigger);
    }

    public void Unlock()
    {        
        hasRequiredItem = true;

        if (gameObject.name == "QueenKazoo")
        {
            GameManager.GetInstance().SetWorldState("GateOpen", true);
        }
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

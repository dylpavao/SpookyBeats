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
    private bool hasRequiredItem;    
    private Item item = null;    


    private void Start()
    {        
        hasRequiredItem = false;
        if(givesItem != ItemType.Null)
            item = new Item { itemType = givesItem, amount = 1 };
    }

    public void TriggerDialogue()
    {
        if(hasRequiredItem)
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
    
    public bool NeedsItem()
    {
        return neededItem != ItemType.Null;
    }

    public bool IsItem()
    {
        return isItem;
    }

}

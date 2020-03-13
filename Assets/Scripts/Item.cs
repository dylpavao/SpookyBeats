using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Null, Crown, Kazoo, Apple, YellowKey, BlueKey, RedKey, MegaKey,
}

public class Item
{    
    public ItemType itemType;    
    public int amount;
    public string itemName;    

}

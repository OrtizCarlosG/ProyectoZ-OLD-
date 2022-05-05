using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int _itemID;
    public string _itemName;
    public enum _itemType
    {
        weapon,
        scope,
        muzzle,
        grip
    }

    public _itemType itemType;

    private void Start()
    {
        ItemManager.addItem(this);
    }


}

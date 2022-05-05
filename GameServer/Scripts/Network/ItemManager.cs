using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static List <Item> itemList = new List<Item>();

    public static ItemManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void addItem(Item item)
    {
        itemList.Add(item);
        ServerSend.ItemSpawned(item.transform.position);
    }

    public static void removeItem(Item item)
    {
        Destroy(item.transform.gameObject);
        itemList.Remove(item);
    }
}

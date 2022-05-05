using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Database : MonoBehaviour
{
    public WeaponsDB _itemsDB;
    public static Database instance;

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


    public static Weapon getItemByName(string name)
    {
        return instance._itemsDB._weapons.FirstOrDefault(i => i._weaponName == name);
    }

    public static Weapon getWeaponByID(int id)
    {
        return instance._itemsDB._weapons[id];
    }

    public static List<Weapon> getWeaponsList()
    {
        return instance._itemsDB._weapons;
    }
    
}

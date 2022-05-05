using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapons", menuName = "Proyecto Z/Weapons/Weapons Database")]
public class WeaponsDB : ScriptableObject
{
    public List<Weapon> _weapons;
}

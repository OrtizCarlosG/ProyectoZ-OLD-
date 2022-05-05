using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Proyecto Z/Weapons/Weapon")]
public class Weapon: ScriptableObject
{
    public string _weaponName;
    public int _weaponID;
    public enum weaponType
    {
        FullAuto,
        SemiAuto,
        Burst,
        All
    }

    public enum weaponsType
    {
        AssaultRifle,
        MDR,
        SMG,
        Shotgun,
        Sniper
    }

    public weaponType _FireMode;
    public weaponsType _weaponType;

    public float _fireRate;

    public float _reloadTime;

    public bool _canUseSight;
    public bool _canUseGrip;
    public bool _canUseMuzzle;

    public int _sightID;
    public int _gripID;
    public int _muzzleID;

    public int _loadedAmmo;
    public int _totalAmmo;
    public int _maxLoadAmmo = 30;
    public float _weaponDamage = 25f;
    public float _bulletForce = 250f;

}

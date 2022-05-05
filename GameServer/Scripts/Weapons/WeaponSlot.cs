using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    public string _weaponName;
    public int _weaponID;

    public bool _canUseSight;
    public bool _canUseGrip;
    public bool _canUseMuzzle;

    public int _sightID;
    public int _gripID;
    public int _muzzleID;

    public int _loadedAmmo;
    public int _totalAmmo;
    public int _maxLoadAmmo = 30;

    public enum _firemodes
    {
        FullAuto,Semi,Brust
    }

    public float _switchCooldown = 2f;
    [HideInInspector] public float _lastSiwtch;

    public _firemodes _firemode = _firemodes.FullAuto;
}

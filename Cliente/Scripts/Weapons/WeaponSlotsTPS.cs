using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotsTPS : MonoBehaviour
{
    public WeaponController[] _weapons;

    [HideInInspector]
    public WeaponController _currentWeapon = null;

    public void setWeaponActive(int _weaponID)
    {
        HideSlot();
        _currentWeapon.transform.gameObject.SetActive(false);
        _currentWeapon = _weapons[_weaponID];
        _weapons[_weaponID].transform.gameObject.SetActive(true);

    }

    public void setCurrentWeapon(int _weaponID)
    {
        _currentWeapon = _weapons[_weaponID];
    }

    public void HideSlot()
    {
        _currentWeapon.transform.gameObject.SetActive(false);
    }
}

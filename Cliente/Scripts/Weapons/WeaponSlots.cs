using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlots : MonoBehaviour
{
    // Start is called before the first frame updat

    public WeaponControllerFPS[] _weapons;

    [HideInInspector]
    public WeaponControllerFPS _currentWeapon = null;

    public void setWeaponActive(int _weaponID)
    {
        _currentWeapon.gameObject.SetActive(false);
        _currentWeapon = _weapons[_weaponID];
        _weapons[_weaponID].gameObject.SetActive(true);

    }

    public void setCurrentWeapon(int _weaponID)
    {
        _currentWeapon = _weapons[_weaponID];
    }

    public void HideSlot()
    {
        _currentWeapon.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{


    public int _id;
    public string _name;
    public GuardWeapon[] _weapons;
    Animator _anim;

    public enum _weaponList
    {
        SA58 = 0,
        MK47 = 1,
        MP7 = 2,
        AK47 = 3,
        VSS = 4,
        ACR = 5,
        DVL = 6,
        HK416 = 7,
        SKS = 8,
        UMP = 9,
        MP5 = 10,
    }

    public _weaponList _weapon;

    public void Initialize(int id, string name , _weaponList _cWeapon)
    {
        _id = id;
        _name = name;
        _weapon = _cWeapon;
        _weapons[(int)_weapon].gameObject.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SyncGuard(int _key, Vector3 _position, Quaternion _rotation)
    {
        if (_key == 0)
        {
            _anim.SetBool("idle", true);
            _anim.SetBool("walk", false);
        }
        else if (_key == 1)
        {
            _anim.SetBool("walk", true);
            _anim.SetBool("idle", false);
        }
        else if (_key == 2)
        {
            _anim.SetBool("walk backward", true);
            _anim.SetBool("walk", false);
            _anim.SetBool("idle", false);
        }
        transform.position = _position;
        transform.rotation = _rotation;
    }

    public void ShotWeapon(Vector3 _poition, Quaternion _rotation, bool isShooting)
    {
        _weapons[(int)_weapon].shotWeapon(isShooting, _rotation);
    }

}

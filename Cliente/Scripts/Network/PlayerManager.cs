using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public float health;
    public float maxHealth = 100f;
    public int itemCount = 0;
    public MeshRenderer model;
    public WeaponSlotsTPS _controller;
    public WeaponControllerFPS _fpscontroller;

    public Quaternion _spineRotation;
    public Transform _spine;

    public Healthbar _healthbar;


    [Header("First Person")]
    public WeaponSlots _firstSlot;
    public WeaponSlots _secondSlot;
    [Header("Audio")]
    public AudioSource _footstepsAudio;
    public AudioClip[] _concrete;
    public AudioClip[] _wood;
    public AudioClip[] _dirt;
    public AudioClip[] _asphalt;
    public AudioSource _characterAudio;
    public AudioClip[] _hitAudio;
    public AudioClip[] _deathAudio;
    [HideInInspector] public bool isCrouched = false;
    public void Initialize(int _id, string _username, int _weaponID)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        if (_firstSlot && _secondSlot)
        {
            _firstSlot.setCurrentWeapon(_weaponID);
            _secondSlot.setCurrentWeapon(_weaponID);
            _fpscontroller = _firstSlot._currentWeapon;
            _healthbar = GameManager.instance._localHealthbar;
        }
        if (_controller)
        {
            _controller.setCurrentWeapon(_weaponID);
            _controller.setWeaponActive(_weaponID);
        }
        if (_healthbar)
        {
            _healthbar.setHealth(maxHealth);
            _healthbar.setName(_username);
        }
    }

    public void SetHealth(float _health)
    {
        health = _health;
        if (_healthbar)
            _healthbar.setHealth(_health);
        characterHitSounds();
        if (health <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        model.enabled = false;
    }

    public void Respawn()
    {
        model.enabled = true;
        SetHealth(maxHealth);
    }

    public void LateUpdate()
    {
        if (_spine)
            _spine.localRotation = _spineRotation;
    }

    public void InputPressed(int _input, int _weaponID)
    {
        if (_fpscontroller)
        {
            if (_input == 8)
            {
                _firstSlot.setWeaponActive(_weaponID);
                _secondSlot.HideSlot();
                _fpscontroller = _firstSlot._currentWeapon;
            }
            else if (_input == 9)
            {
                _secondSlot.setWeaponActive(_weaponID);
                _firstSlot.HideSlot();
                _fpscontroller = _secondSlot._currentWeapon;
            }
            else
            {
                _fpscontroller.setAnimValue(_input);
            }
        }

        if (_controller)
        {
            if (_input == 8 || _input == 9)
            {
                _controller.HideSlot();
                _controller.setCurrentWeapon(_weaponID);
                _controller.setWeaponActive(_weaponID);
            } else
            {
                _controller._currentWeapon.setAnimValue(_input);
            }
        }
        if (_input == 0 || _input == 1 || _input == 2 || _input == 3)
        {
            if (!_footstepsAudio.isPlaying)
            {
                _footstepsAudio.clip = _concrete[Random.Range(0, _concrete.Length)];
                _footstepsAudio.Play();
            }
        }
        if (_input == 11)
        {
            if (!isCrouched)
                isCrouched = true;
            else
                isCrouched = false;
        }
    }


    public void characterHitSounds()
    {
        if (health > 0)
        {
            if (!_characterAudio.isPlaying)
            {
                _characterAudio.clip = _hitAudio[Random.Range(0, _hitAudio.Length)];
                _characterAudio.Play();
            }
        } else
        {
            _characterAudio.clip = _deathAudio[Random.Range(0, _deathAudio.Length)];
            _characterAudio.Play();
        }
    }

    public void WeaponAttachment(string type, int _id, int _attachID)
    {
        if (_fpscontroller)
        {
            _fpscontroller.SwitchAttachment(type, _attachID);
        } else
        {
            _controller._currentWeapon.SwitchAttachment(type, _attachID);
        }
    }

    public void WeaponFiremode(string _firemode)
    {
        if (_fpscontroller)
        {
            _fpscontroller.SwitchFiremode(_firemode);
        }
    }
}
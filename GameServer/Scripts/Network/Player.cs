using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public int id;
    public string _playerTag;
    public string _chatColor = "white";
    public CharacterController controller;
    public Transform shootOrigin;
    public float gravity = -9.81f;
    public float walkSpeed = 5.0f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public float crouchSpeed = 2f;
    public float throwForce = 600f;
    public float runSpeed = 7.0f;
    public float health;
    public float maxHealth = 100f;
    public int itemAmount = 0;
    public int maxItemAmount = 3;

    private bool[] inputs;
    private float yVelocity = 0;

    private float lastFired;
    private float _fireRate = 100;

    public Transform bulletPrefab;

    public Transform _spine;

    public WeaponSlot _primarySlot;
    public WeaponSlot _secondarySlot;
    public bool _reloading = false;
    public bool isInSafeZone = false;
    public bool isCrouched = false;
    [HideInInspector] public float _experience = 0f;

    Animator _anim;

    [HideInInspector]
    public WeaponSlot _currentSlot;
    private Quaternion _spineRotQ;
    bool isProned = false;
    float lastCrouch;
    public bool isInInterior = false;
    [HideInInspector] public SafeZone _currentSafeZone = null;
    public bool hasBackpack = false, hasTacticalRig = false, hasArmor = false, hasHelmet = false, hasHeadset = false, hasGoogles = false;


    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        runSpeed *= Time.fixedDeltaTime;
        walkSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
        crouchSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        name = _username;
        health = maxHealth;

        inputs = new bool[13];
        _currentSlot = _primarySlot;
        _anim = GetComponent<Animator>();
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void FixedUpdate()
    {
        if (health <= 0f)
        {
            return;
        }

        Vector2 _inputDirection = Vector2.zero;
        int _input = 6;

        for (int i= 0; i < inputs.Length; i++)
        {
            if (!inputs[i])
            {
                if (!isCrouched)
                {
                    _anim.SetBool("strafe left", false);
                    _anim.SetBool("strafe right", false);
                    _anim.SetBool("walk backward", false);
                    _anim.SetBool("walk", false);
                    _anim.SetBool("reload", false);
                    _anim.SetBool("idle", true);
                }
                else
                {
                    _anim.SetBool("Crouched", true);
                    _anim.SetBool("CrouchFdw", false);
                    _anim.SetBool("CrouchBwd", false);
                    _anim.SetBool("CrouchLeft", false);
                    _anim.SetBool("CrouchRight", false);
                }
            }
        }
        if (inputs[0])
        {
            _inputDirection.y += 1;
            _input = 0;
            if (!isCrouched)
            {
                _anim.SetBool("strafe left", false);
                _anim.SetBool("strafe right", false);
                _anim.SetBool("walk backward", false);
                _anim.SetBool("reload", false);
                _anim.SetBool("walk", true);
                _anim.SetBool("idle", false);
            } else
            {
                _anim.SetBool("CrouchFdw", true);
                _anim.SetBool("CrouchBwd", false);
                _anim.SetBool("CrouchLeft", false);
                _anim.SetBool("CrouchRight", false);
            }
        }
        if (inputs[1])
        {
            _inputDirection.y -= 1;
            _input = 1;
            if (!isCrouched)
            {
                _anim.SetBool("strafe left", false);
                _anim.SetBool("strafe right", false);
                _anim.SetBool("walk backward", true);
                _anim.SetBool("reload", false);
                _anim.SetBool("walk", false);
                _anim.SetBool("idle", false);
            } else
            {
                _anim.SetBool("CrouchFdw", false);
                _anim.SetBool("CrouchBwd", true);
                _anim.SetBool("CrouchLeft", false);
                _anim.SetBool("CrouchRight", false);
            }
        }
        if (inputs[2])
        {
            _inputDirection.x -= 1;
            _input = 2;
            if (!isCrouched)
            {
                _anim.SetBool("strafe left", true);
                _anim.SetBool("strafe right", false);
                _anim.SetBool("walk backward", false);
                _anim.SetBool("reload", false);
                _anim.SetBool("walk", false);
                _anim.SetBool("idle", false);
            } else
            {
                _anim.SetBool("CrouchFdw", false);
                _anim.SetBool("CrouchBwd", false);
                _anim.SetBool("CrouchLeft", true);
                _anim.SetBool("CrouchRight", false);
            }
        }
        if (inputs[3])
        {
            _inputDirection.x += 1;
            _input = 3;
            if (!isCrouched)
            {
                _anim.SetBool("strafe right", true);
                _anim.SetBool("strafe left", false);
                _anim.SetBool("walk backward", false);
                _anim.SetBool("reload", false);
                _anim.SetBool("walk", false);
                _anim.SetBool("idle", false);
            } else
            {
                _anim.SetBool("CrouchFdw", false);
                _anim.SetBool("CrouchBwd", false);
                _anim.SetBool("CrouchLeft", false);
                _anim.SetBool("CrouchRight", true);
            }
        }
        if (inputs[5] && _currentSlot != _primarySlot)
        {
            _input = 8;
            _currentSlot = _primarySlot;
            Weapon _weapon = Database.getWeaponByID(_primarySlot._weaponID);
            _fireRate = _weapon._fireRate;
            _primarySlot._weaponName = _weapon._weaponName;
            _primarySlot._canUseSight = _weapon._canUseSight;
            _primarySlot._canUseGrip = _weapon._canUseGrip;
            _primarySlot._canUseMuzzle = _weapon._canUseMuzzle;
            _primarySlot._maxLoadAmmo = _weapon._maxLoadAmmo;
        }
        if (inputs[6] && _currentSlot != _secondarySlot)
        {
            _input = 9;
            _currentSlot = _secondarySlot;
            Weapon _weapon = Database.getWeaponByID(_secondarySlot._weaponID);
            _fireRate = _weapon._fireRate;
            _secondarySlot._weaponName = _weapon._weaponName;
            _secondarySlot._canUseSight = _weapon._canUseSight;
            _secondarySlot._canUseGrip = _weapon._canUseGrip;
            _secondarySlot._canUseMuzzle = _weapon._canUseMuzzle;
            _secondarySlot._maxLoadAmmo = _weapon._maxLoadAmmo;
        }
        if(inputs[7] && !_reloading)
        {

            if (_currentSlot._totalAmmo > 0)
            {
                _input = 10;
                if (!isCrouched)
                    _anim.SetBool("reload", true);
                else
                    _anim.Play("CrouchReload", -1, 0f);
                _reloading = true;
                StartCoroutine(reloadCooldown());
                if (_currentSlot._totalAmmo >= _currentSlot._maxLoadAmmo)
                {
                    _currentSlot._totalAmmo -= _currentSlot._maxLoadAmmo - _currentSlot._loadedAmmo;
                    _currentSlot._loadedAmmo = _currentSlot._maxLoadAmmo;
                } else
                {
                    _currentSlot._loadedAmmo = _currentSlot._totalAmmo;
                    _currentSlot._totalAmmo = 0;
                }
               
            }
        }
        
        if (inputs[8])
        {
            interact();
        }
        if (inputs[9] && Time.time - _currentSlot._lastSiwtch > 1 / _currentSlot._switchCooldown)
        {
            _currentSlot._lastSiwtch = Time.time;
            switchFiremode();
        }
        if(inputs[10])
        {
            if (Time.time - lastCrouch > 1 / 3f)
            {
                lastCrouch = Time.time;
                if (isCrouched)
                {
                    isCrouched = false;
                    controller.center = new Vector3(0f, 0.9f, 0f);
                    controller.height = 1.8f;
                }
                else
                {
                    controller.center = new Vector3(0f, 0.5f, 0f);
                    controller.height = 1f;
                    isCrouched = true;
                }
                _input = 11;
                _anim.SetBool("Crouched", isCrouched);
            }
        }
        if (inputs[12])
        {
            moveSpeed = runSpeed;
        }else
        {
            moveSpeed = walkSpeed;
            _input = 12;
        }

        Move(_inputDirection,_input, _currentSlot._weaponID);
    }

    private IEnumerator reloadCooldown()
    {
        yield return new  WaitForSeconds(Database.getWeaponByID(_currentSlot._weaponID)._reloadTime);
        _reloading = false;
    }

    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    private void Move(Vector2 _inputDirection, int _input, int _wpID)
    {
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;

        if (controller.isGrounded)
        {
            if (!isCrouched)
                 _moveDirection *= moveSpeed;
            else
                _moveDirection *= crouchSpeed;
            yVelocity = 0f;
            if (inputs[4])
            {
                _input = 4;
                yVelocity = jumpSpeed;
            }
        } else
        {
            _moveDirection *= moveSpeed / 2;
        }
        yVelocity += gravity;

        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);

        ServerSend.PlayerPosition(this, _input, _wpID);
        ServerSend.PlayerRotation(this);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] _inputs, Quaternion _rotation, Quaternion _spineRot)
    {
        inputs = _inputs;

        //float _rotX = Mathf.Clamp((_spineRot.x / 2), 45, -45);

        _spineRotQ = _spineRot;

        transform.rotation = new Quaternion(0f, _rotation.y, 0f, _rotation.w);
    }

    private void LateUpdate()
    {
        displayItem();
        _spine.localRotation = _spineRotQ;
    }

    public void Shoot(Vector3 _viewDirection, Quaternion _rotation, bool _still)
    {
        if (health <= 0f || isInSafeZone)
        {
            return;
        }
        if (_currentSlot._loadedAmmo > 0 && !_reloading)
        {
            if (_still)
            {

                if (Time.time - lastFired > 1 / _fireRate)
                {

                    float rotx = Random.Range(0f, 0f);
                    float roty = Random.Range(0f, 0f);
                    float rotz = Random.Range(0f, 0f);

                    float _multiplier = 1f;

                    if (inputs[0] || inputs[1] || inputs[2] || inputs[3])
                    {
                        _multiplier = 2f;
                        rotx = Random.Range(-2.5f, 2.5f);
                        roty = Random.Range(-2.5f, 2.5f);
                        rotz = Random.Range(-2.5f, 2.5f);
                    }
                    else if (inputs[4])
                    {
                        _multiplier = 4f;
                        rotx = Random.Range(-5f, 5f);
                        roty = Random.Range(-5f, 5f);
                        rotz = Random.Range(-5f, 5f);
                    }
                    else
                        _multiplier = 1f;
                    if (!isCrouched)
                        _anim.Play("Fire", -1, 0f);
                    else
                        _anim.Play("CrouchFire", -1, 0f);
                    lastFired = Time.time;

                    _currentSlot._loadedAmmo--;

                    var bullet = (Transform)Instantiate(
                                    bulletPrefab,
                                    _viewDirection,
                                    _rotation);
                    Weapon _weapon = Database.getWeaponByID(_currentSlot._weaponID);
                    bullet.Rotate(rotx,roty,rotz);
                    bullet.GetComponent<Rigidbody>().velocity =
                                bullet.transform.forward * _weapon._bulletForce;

                    
                    bullet.transform.gameObject.GetComponent<Bullet>().bulletDamage = _weapon._weaponDamage;
                    bullet.transform.gameObject.GetComponent<Bullet>()._owner = this;
                    bullet.transform.gameObject.GetComponent<Bullet>()._spawnPoint = transform.position;
                    ServerSend.PlayerShoot(this, _viewDirection, bullet.transform.rotation, true, _still, _multiplier, _weapon._bulletForce);
                }

            }
            else
            {
                ServerSend.PlayerShoot(this, _viewDirection, shootOrigin.transform.rotation, true, false, 1f, 250f);
            }
        }
        else
        {
            ServerSend.PlayerShoot(this, shootOrigin.transform.position, shootOrigin.transform.rotation, false, false, 1f, 250f);
        }

    }
    
    public void switchFiremode()
    {
        switch (_currentSlot._firemode)
        {
            case WeaponSlot._firemodes.FullAuto:
              _currentSlot._firemode = WeaponSlot._firemodes.Semi;
                break;
            case WeaponSlot._firemodes.Semi:
                _currentSlot._firemode = WeaponSlot._firemodes.Brust;
                break;
            case WeaponSlot._firemodes.Brust:
                _currentSlot._firemode = WeaponSlot._firemodes.FullAuto;
                break;
            default:
                _currentSlot._firemode = WeaponSlot._firemodes.FullAuto;
                break;
        }

        ServerSend.WeaponFiremode(this, _currentSlot._firemode.ToString());
    }
    public void displayItem()
    {
        RaycastHit rayHit = new RaycastHit();
        if (Physics.Raycast(shootOrigin.position, shootOrigin.transform.forward, out rayHit, 3f))
        {
            Item _item = rayHit.collider.gameObject.transform.GetComponent<Item>();
            if (_item)
            {
                ServerSend.InteractMenu(_item._itemName, this.id);
            } else
            {
                ServerSend.InteractMenu("None", this.id);
            }
        }
    }
    public void interact()
    {
        RaycastHit rayHit = new RaycastHit();
        if (Physics.Raycast(shootOrigin.position, shootOrigin.transform.forward, out rayHit, 3f))
        {
            Item _item = rayHit.collider.gameObject.transform.GetComponent<Item>();
            if (_item)
            {
                if (_item.itemType.Equals(Item._itemType.weapon))
                {
                    Weapon _weapon = Database.getWeaponByID(_item._itemID);
                    _fireRate = _weapon._fireRate;
                    _currentSlot._weaponID = _item._itemID;
                    _currentSlot._weaponName = _weapon._weaponName;
                    _currentSlot._canUseSight = _weapon._canUseSight;
                    _currentSlot._canUseGrip = _weapon._canUseGrip;
                    _currentSlot._canUseMuzzle = _weapon._canUseMuzzle;
                    _currentSlot._maxLoadAmmo = _weapon._maxLoadAmmo;
                    if (_currentSlot == _primarySlot)
                    {
                        ServerSend.PlayerPosition(this, 8, _currentSlot._weaponID);
                    } else
                    {
                        ServerSend.PlayerPosition(this, 9, _currentSlot._weaponID);
                    }
                } else if (_item.itemType.Equals(Item._itemType.scope))
                {
                    if (_currentSlot._canUseSight)
                    {
                        _currentSlot._sightID = _item._itemID;
                        ServerSend.WeaponAttachment(this, 1, "scope", _item._itemID);
                    }
                }
                else if (_item.itemType.Equals(Item._itemType.muzzle))
                {
                    if (_currentSlot._canUseSight)
                    {
                        _currentSlot._sightID = _item._itemID;
                        ServerSend.WeaponAttachment(this, 1, "muzzle", _item._itemID);
                    }
                }
                else if (_item.itemType.Equals(Item._itemType.grip))
                {
                    if (_currentSlot._canUseGrip)
                    {
                        _currentSlot._sightID = _item._itemID;
                        ServerSend.WeaponAttachment(this, 1, "grip", _item._itemID);
                    }
                }
            }

        }
    }

    public void ThrowItem(Vector3 _viewDirection)
    {
        if (health <= 0f)
        {
            return;
        }

        if (itemAmount > 0)
        {
            itemAmount--;
            NetworkManager.instance.InstantiateProjectile(shootOrigin).Initialize(_viewDirection, throwForce, id);
        }
    }

    public void TakeDamage(float _damage, Player _attacker)
    {
        if (!isInSafeZone)
        {
            if (health <= 0f)
            {
                return;
            }

            health -= _damage;
            if (health <= 0f)
            {
                health = 0f;
                controller.enabled = false;
                transform.position = new Vector3(0f, 25f, 0f);
                ServerSend.PlayerPosition(this, 6, 0);
                StartCoroutine(Respawn());
            }

            ServerSend.PlayerHealth(this);
        }
    }

    public void TakeDamage(float _damage)
    {
        bool takeDamage = true;
        if (_currentSafeZone)
            if (isInSafeZone)
                takeDamage = false;
            else if (isInSafeZone && _currentSafeZone.isUnderAttack)
                takeDamage = true;
        if (takeDamage)
        {
            if (health <= 0f)
            {
                return;
            }

            health -= _damage;
            if (health <= 0f)
            {
                health = 0f;
                controller.enabled = false;
                transform.position = new Vector3(0f, 25f, 0f);
                ServerSend.PlayerPosition(this, 6, 0);
                StartCoroutine(Respawn());
            }

            ServerSend.PlayerHealth(this);
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxHealth;
        controller.enabled = true;
        ServerSend.PlayerRespawned(this);
    }

    public bool AttemptPickupItem()
    {
        if (itemAmount >= maxItemAmount)
        {
            return false;
        }

        itemAmount++;
        return true;
    }
}
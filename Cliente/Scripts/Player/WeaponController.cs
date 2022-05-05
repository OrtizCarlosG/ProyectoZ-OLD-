using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{



    [Header("Weapon Settings")]
    public Transform _bulletPrefab;
    public Transform _muzzle;
    public Transform _caseEjector;
    public Transform _casePrefab;
    public Transform _weaponSmoke;
    public Transform _weaponSmokeAfterFire;

    public Transform _hand;
    public Transform _leftHandPoint;
    public Transform _leftHand;

    public Animator _anim;
    [System.Serializable]
    public class weaponData
    {
        public string scopeName;
        public GameObject _scopeObject;
    }
    // -0.108 0.33 0.69
    [Header("Weapon Attachments")]
    public weaponData[] _weaponScopes;
    public Transform[] _weaponMuzzle;
    public Transform[] _weaponGrips;

    [HideInInspector]
    public bool isSilenced;
    public int _currentScope = 0;
    public int _currentMuzzle = 0;
    public int _currentGrip = 0;

    [Header("Audio")]
    public AudioClip _drawSound;
    public AudioClip _gunFire;
    public AudioClip _tail;
    public AudioClip _noAmmo;
    public AudioClip _silencedFire;
    public AudioClip _silencedTail;
    public AudioClip _gunFireInterior;
    public AudioClip _gunfireInteriorTail;
    public AudioClip _gunFireInteriorSilenced;
    public AudioClip _gunfireInteriorSilencedTail;
    public AudioSource _weaponAudio;
    public AudioClip[] _spinAudio;
    public AudioSource _caseAudio;
    public AudioClip[] _concreteClips;

    static bool isCrouched = false;
    bool _shooting;

    AudioSource _audio;

    private void Awake()
    {
        //transform.SetParent(_hand);
    }
    // Start is called before the first frame update
    void Start()
    {
        _audio = this.GetComponent<AudioSource>();
        _audio.clip = _drawSound;
        _audio.Play();
    }

    public void weaponShoot(bool _canShot, bool _fullAuto, Vector3 _position, Quaternion _rot, bool isInterior, float _bulletForce)
    {

        _shooting = _fullAuto;
        weaponShootSound(_fullAuto, isInterior);
        if (_canShot)
        {
            if (_fullAuto)
            {
                var bullet = (Transform)Instantiate(
                                   _bulletPrefab,
                                   _muzzle.transform.position,
                                   _rot);

                bullet.GetComponent<Rigidbody>().velocity =
                            bullet.transform.forward * _bulletForce;
                var bulletCase = (Transform)Instantiate(
                                 _casePrefab,
                                 _caseEjector.transform.position,
                                 _caseEjector.transform.rotation);
                bulletCase.SetParent(_caseEjector);
                StartCoroutine(destroyObject(bulletCase));
                var smoke = (Transform)Instantiate(
                      _weaponSmoke,
                      _muzzle.transform.position,
                      _muzzle.transform.rotation);
                smoke.SetParent(_muzzle);
                StartCoroutine(destroyObject(smoke));
                StartCoroutine(caseSound());
            }
        }
        else
        {
            if (!_audio.isPlaying)
            {
                _audio.clip = _noAmmo;
                _audio.loop = false;
                _audio.Play();
            } else if (!_audio.clip == _gunFire)
            {
                _audio.clip = _noAmmo;
                _audio.loop = false;
                _audio.Play();
            }
        }
    }

    public void weaponShootSound(bool _shooting, bool isInterior)
    {
        if (_shooting)
        {
            if (!_audio.isPlaying)
            {
                if (!isInterior)
                {
                    if (!isSilenced)
                    {
                        _audio.clip = _gunFire;
                    }
                    else
                    {
                        _audio.clip = _silencedFire;
                    }
                }
                else
                {
                    if (!isSilenced)
                    {
                        _audio.clip = _gunFireInterior;
                    }
                    else
                    {
                        _audio.clip = _gunFireInteriorSilenced;
                    }
                }
                _audio.loop = true;
                _audio.Play();
            }
            else if (_audio.clip == _tail || _audio.clip == _silencedTail || _audio.clip == _gunfireInteriorTail || _audio.clip == _gunfireInteriorSilencedTail)
            {
                if (!isInterior)
                {
                    if (!isSilenced)
                    {
                        _audio.clip = _gunFire;
                    }
                    else
                    {
                        _audio.clip = _silencedFire;
                    }
                }
                else
                {
                    if (!isSilenced)
                    {
                        _audio.clip = _gunFireInterior;
                    }
                    else
                    {
                        _audio.clip = _gunFireInteriorSilenced;
                    }
                }
                _audio.loop = true;
                _audio.Play();
            }

        }
        else
        {
            if (_audio.clip == _gunFire || _audio.clip == _silencedFire || _audio.clip == _gunFireInterior || _audio.clip == _gunFireInteriorSilenced)
            {
                if (!isInterior)
                {
                    if (!isSilenced)
                    {
                        _audio.clip = _tail;
                    }
                    else
                    {
                        _audio.clip = _silencedTail;
                    }
                }
                else
                {
                    if (!isSilenced)
                    {
                        _audio.clip = _gunfireInteriorTail;
                    }
                    else
                    {
                        _audio.clip = _gunfireInteriorSilencedTail;
                    }
                }
                _audio.loop = false;
                _audio.Play();
                var smoke = (Transform)Instantiate(
                _weaponSmokeAfterFire,
                _muzzle.transform.position,
                _muzzle.transform.rotation);
                smoke.SetParent(_muzzle);
                StartCoroutine(destroyObject(smoke));
            }

        }
    }

    public void SwitchAttachment(string type, int id)
    {
        if (type.Equals("scope"))
        {
            if (_weaponScopes[_currentScope]._scopeObject)
            {
                _weaponScopes[_currentScope]._scopeObject.SetActive(false);
            }
            _weaponScopes[id]._scopeObject.SetActive(true);
            _currentScope = id;
        }
        else if (type.Equals("muzzle"))
        {
            _weaponMuzzle[_currentMuzzle].gameObject.SetActive(false);
            _weaponMuzzle[id].gameObject.SetActive(true);
            _currentMuzzle = id;
            isSilenced = true;
        }
        else if (type.Equals("grip"))
        {
            _weaponGrips[_currentGrip].gameObject.SetActive(false);
            _weaponGrips[id].gameObject.SetActive(true);
            _currentGrip = id;
        }
    }

    public void setAnimValue(int _input)
    {
        if (_anim)
        {
            if (_input == 6)
            {
                _anim.enabled = true;
                if (!isCrouched)
                {
                    _anim.SetBool("idle", true);
                    _anim.SetBool("walk", false);
                    _anim.SetBool("walk backward", false);
                    _anim.SetBool("strafe left", false);
                    _anim.SetBool("strafe right", false);
                    _anim.SetBool("Crouched", false);
                    _anim.SetBool("CrouchBwd", false);
                    _anim.SetBool("CrouchFwd", false);
                    _anim.SetBool("CrouchLeft", false);
                    _anim.SetBool("CrouchRight", false);
                }
                else
                {
                    _anim.SetBool("Crouched", true);
                    _anim.SetBool("CrouchBwd", false);
                    _anim.SetBool("CrouchFwd", false);
                    _anim.SetBool("CrouchLeft", false);
                    _anim.SetBool("CrouchRight", false);
                    _anim.SetBool("strafe left", false);
                    _anim.SetBool("walk", false);
                    _anim.SetBool("idle", false);
                    _anim.SetBool("walk backward", false);
                    _anim.SetBool("strafe right", false);
                }
            }
            else if (_input == 0 )
            {
                if (!isCrouched)
                {
                    _anim.SetBool("walk", true);
                    _anim.SetBool("idle", false);
                    _anim.SetBool("walk backward", false);
                    _anim.SetBool("strafe left", false);
                    _anim.SetBool("strafe right", false);
                    _anim.SetBool("Crouched", false);
                    _anim.SetBool("CrouchBwd", false);
                    _anim.SetBool("CrouchFwd", false);
                    _anim.SetBool("CrouchLeft", false);
                    _anim.SetBool("CrouchRight", false);
                }
                else
                {
                    _anim.SetBool("Crouched", true);
                    _anim.SetBool("CrouchBwd", false);
                    _anim.SetBool("CrouchFwd", true);
                    _anim.SetBool("CrouchLeft", false);
                    _anim.SetBool("CrouchRight", false);
                    _anim.SetBool("strafe left", false);
                    _anim.SetBool("walk", false);
                    _anim.SetBool("idle", false);
                    _anim.SetBool("walk backward", false);
                    _anim.SetBool("strafe right", false);
                }
            }
            else if (_input == 1)
            {
                if (!isCrouched)
                {
                    _anim.SetBool("walk backward", true);
                    _anim.SetBool("walk", false);
                    _anim.SetBool("idle", false);
                    _anim.SetBool("strafe left", false);
                    _anim.SetBool("strafe right", false);
                    _anim.SetBool("Crouched", false);
                    _anim.SetBool("CrouchBwd", false);
                    _anim.SetBool("CrouchFwd", false);
                    _anim.SetBool("CrouchLeft", false);
                    _anim.SetBool("CrouchRight", false);
                }
                else
                {
                    _anim.SetBool("Crouched", true);
                    _anim.SetBool("CrouchBwd", true);
                    _anim.SetBool("CrouchFwd", false);
                    _anim.SetBool("CrouchLeft", false);
                    _anim.SetBool("CrouchRight", false);
                    _anim.SetBool("strafe left", false);
                    _anim.SetBool("walk", false);
                    _anim.SetBool("idle", false);
                    _anim.SetBool("walk backward", false);
                    _anim.SetBool("strafe right", false);
                }
            } else if (_input == 2)
            {
                if (!isCrouched)
                {
                    _anim.SetBool("strafe left", true);
                    _anim.SetBool("walk", false);
                    _anim.SetBool("idle", false);
                    _anim.SetBool("walk backward", false);
                    _anim.SetBool("strafe right", false);
                    _anim.SetBool("Crouched", false);
                    _anim.SetBool("CrouchBwd", false);
                    _anim.SetBool("CrouchFwd", false);
                    _anim.SetBool("CrouchLeft", false);
                    _anim.SetBool("CrouchRight", false);
                }
                else
                {
                    _anim.SetBool("Crouched", true);
                    _anim.SetBool("CrouchBwd", false);
                    _anim.SetBool("CrouchFwd", false);
                    _anim.SetBool("CrouchLeft", true);
                    _anim.SetBool("CrouchRight", false);
                    _anim.SetBool("strafe left", false);
                    _anim.SetBool("walk", false);
                    _anim.SetBool("idle", false);
                    _anim.SetBool("walk backward", false);
                    _anim.SetBool("strafe right", false);
                }
            } else if(_input == 3)
            {
                if (!isCrouched)
                {
                    _anim.SetBool("strafe right", true);
                    _anim.SetBool("walk", false);
                    _anim.SetBool("idle", false);
                    _anim.SetBool("walk backward", false);
                    _anim.SetBool("strafe left", false);
                    _anim.SetBool("Crouched", false);
                    _anim.SetBool("CrouchBwd", false);
                    _anim.SetBool("CrouchFwd", false);
                    _anim.SetBool("CrouchLeft", false);
                    _anim.SetBool("CrouchRight", false);
                }
                else
                {
                    _anim.SetBool("Crouched", true);
                    _anim.SetBool("CrouchBwd", false);
                    _anim.SetBool("CrouchFwd", false);
                    _anim.SetBool("CrouchLeft", false);
                    _anim.SetBool("CrouchRight", true);
                    _anim.SetBool("strafe left", false);
                    _anim.SetBool("walk", false);
                    _anim.SetBool("idle", false);
                    _anim.SetBool("walk backward", false);
                    _anim.SetBool("strafe right", false);
                }
            }
            else if (_input == 4)
            {
                _anim.Play("jump", -1, 0f);
            }
            else if (_input == 7)
            {
                if (!isCrouched)
                    _anim.Play("Fire", -1, 0f);
                else
                    _anim.Play("CrouchedFire", -1, 0f);
            } else if (_input == 10)
            {
                if (!isCrouched)
                _anim.SetBool("reload", true);
                else
                    _anim.SetBool("Crouched Reload", true);
            } else if (_input == 11)
            {
                if (isCrouched)
                    isCrouched = false;
                else
                    isCrouched = true;
                _anim.SetBool("Crouched", isCrouched);
            }
            if (!_weaponAudio.isPlaying && _input != 6)
            {
                _weaponAudio.clip = _spinAudio[Random.Range(0, _spinAudio.Length)];
                _weaponAudio.Play();
            }
        }
    }

    private void LateUpdate()
    {
        _leftHand.position = _leftHandPoint.position;
    }

    private IEnumerator destroyObject(Transform _object)
    {
        yield return new WaitForSeconds(1f);
        Destroy(_object.gameObject);
    }

    private IEnumerator caseSound()
    {
        yield return new WaitForSeconds(1f);
        _caseAudio.clip = _concreteClips[Random.Range(0, _concreteClips.Length)];
        _caseAudio.Play();
    }
}

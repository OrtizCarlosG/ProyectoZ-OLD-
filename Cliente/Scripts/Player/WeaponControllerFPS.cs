using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControllerFPS : MonoBehaviour
{
    [Header("Gun Camera")]
    public Camera gunCamera;

    [Header("Gun Camera Options")]
    [Tooltip("How fast the camera field of view changes when aiming.")]
    public float fovSpeed = 15.0f;
    [Tooltip("Default value for camera field of view (40 is recommended).")]
    public float defaultFov = 40.0f;
    public float sprintFov = 50.0f;

    [Header("Weapon Sway")]
    [Tooltip("Toggle weapon sway.")]
    public bool weaponSway;

    public float swayAmount = 0.02f;
    public float maxSwayAmount = 0.06f;
    public float swaySmoothValue = 4.0f;

    public float rotationAmount = 4f;
    public float maxRotationAmount = 5f;
    public float smothRotation = 12f;

    private Quaternion initialTiltRotation;
    private Vector3 initialSwayPosition;

    [Header("Weapon Settings")]
    public Transform _weaponsHolder;
    public Transform _bulletPrefab;
    public Transform _muzzle;
    public Transform _caseEjector;
    public Transform _casePrefab;
    public Transform _weaponSmoke;
    public Transform _weaponSmokeAfterFire;
    public WeaponRecoil _recoil;
    public Transform _Slider;
    public Vector3 _SliderMovment;
    public bool canAim;
    public _firemodes _firemode = _firemodes.FullAuto;
    public Transform _leftHand;
    public enum _firemodes
    {
        FullAuto,Semi,Burst
    }
    [System.Serializable]
    public class weaponData
    {
        public string scopeName;
        public GameObject _scopeObject;
        public Vector3 aimPosition;
        public float _fov;
        public float _kickBackMultiplier = -0.2f;
    }
    [System.Serializable]
    public class gripData
    {
        public string gripName;
        public Transform _grip;
    }
    // -0.108 0.33 0.69
    [Header("Weapon Attachments")]
    public Vector3 initCameraPos;
    public weaponData[] _weaponScopes;
    public Transform[] _weaponMuzzle;
    public gripData[] _weaponGrips;
    [HideInInspector]
    public bool isAiming;
    public bool isSilenced;
    public int _currentScope = 0;
    public int _currentMuzzle = 0;
    public int _currentGrip = 0;

    [Header("Audio")]
    public AudioClip _drawSound;
    public AudioClip _magout;
    public AudioClip _magin;
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


    [Header("Animators")]
    public Animator _legsAnim;
    [Header("Testing")]
    //public Transform _weaponsContainer;
    //public Transform _camBone;

    static bool isCrouched = false;
    bool _shooting;
    Animator _anim;
    AudioSource _audio;
    [HideInInspector] public bool isMoving = false;
    // Start is called before the first frame update
    bool moveSlider = false;
    bool useGrip = false;
    Vector3 initialSliderPosition;
    void Start()
    {
        _anim = this.GetComponent<Animator>();
        _audio = this.GetComponent<AudioSource>();
        _audio.clip = _drawSound;
        _audio.Play();
        initialSwayPosition = transform.localPosition;
        initialSliderPosition = _Slider.localPosition;
        initialTiltRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        

        if (canAim)
        {
            if (Input.GetMouseButton(1))
            {
                isAiming = true;
                //transform.localPosition = _weaponScopes[0].aimPosition;
                _anim.SetBool("Aiming", true);
                _recoil.aim = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                isAiming = false;
                transform.localPosition = initCameraPos;
                _anim.SetBool("Aiming", false);
                _recoil.aim = false;
                gunCamera.fieldOfView = 60f;
            }
        }

        if ((Input.GetKey(KeyCode.F)))
        {
           _anim.Play("Attachments", -1, 0f);
        }

        if (_firemode.Equals(_firemodes.FullAuto))
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                ClientSend.PlayerShoot(_muzzle.transform.position, _muzzle.transform.rotation, true);
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                ClientSend.PlayerShoot(_muzzle.transform.position, _muzzle.transform.rotation, false);
            }
        } else
        {
            if (Input.GetMouseButtonDown(0))
            {
                ClientSend.PlayerShoot(_muzzle.transform.position, _muzzle.transform.rotation, true);
                ClientSend.PlayerShoot(_muzzle.transform.position, _muzzle.transform.rotation, false);
            }
        }


    }

    private void LateUpdate()
    {
        if (weaponSway && !isAiming && (Cursor.lockState == CursorLockMode.Locked))
        {
            float movementX = -Input.GetAxis("Mouse X");
            float movementY = -Input.GetAxis("Mouse Y");
            movementX = Mathf.Clamp
                (movementX * swayAmount, -maxSwayAmount, maxSwayAmount);
            movementY = Mathf.Clamp
                (movementY * swayAmount, -maxSwayAmount, maxSwayAmount);
            Vector3 finalSwayPosition = new Vector3 (movementX, movementY, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalSwayPosition + initialSwayPosition, Time.deltaTime * swaySmoothValue);
            float inputX = -Input.GetAxis("Mouse X");
            float inputY = -Input.GetAxis("Mouse Y");
            float tiltX = Mathf.Clamp(inputX * rotationAmount, -maxRotationAmount, maxRotationAmount);
            float tiltY = Mathf.Clamp(inputY * rotationAmount, -maxRotationAmount, maxRotationAmount);
            Quaternion finalTiltRotation = Quaternion.Euler(new Vector3(tiltX, 0f, tiltY));
            transform.localRotation = Quaternion.Slerp(transform.localRotation, finalTiltRotation * initialTiltRotation, Time.deltaTime * smothRotation);

        }

        if (isAiming)
        {
            //  _anim.enabled = false;
            // Vector3 _recoil = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - 0.2f);
            // Vector3 _recoil2 = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 0.2f);
            // this.gameObject.transform.localPosition = Vector3.Lerp(transform.localPosition, _recoil, Time.deltaTime * 0.4f);
            // this.gameObject.transform.localPosition = Vector3.Lerp(transform.localPosition, _recoil2, Time.deltaTime * 0.8f);
            if (_shooting)
            {
               // Vector3 _movePos = new Vector3(_weaponScopes[_currentScope].aimPosition.x, _weaponScopes[_currentScope].aimPosition.y, _weaponScopes[_currentScope].aimPosition.z - _weaponScopes[_currentScope]._kickBackMultiplier);
               //  _Slider.localPosition = Vector3.Lerp(initialSliderPosition, _SliderMovment, Time.fixedDeltaTime * 20f);
               //  _Slider.localPosition = Vector3.Lerp(_SliderMovment, initialSliderPosition, Time.deltaTime * 20f * 8f);
               // transform.localPosition = Vector3.Lerp(_weaponScopes[_currentScope].aimPosition, _movePos, Time.fixedDeltaTime * 10f);
               // transform.localPosition = Vector3.Lerp(_movePos, _weaponScopes[_currentScope].aimPosition, Time.deltaTime * 20f * 4f);
            }
           // _anim.enabled = true;
               float movementX = -Input.GetAxis("Mouse X") * swayAmount;
               float movementY = -Input.GetAxis("Mouse Y") * swayAmount;
               movementX = Mathf.Clamp
                   (movementX, -maxSwayAmount, maxSwayAmount);
               movementY = Mathf.Clamp
                   (movementY, -maxSwayAmount, maxSwayAmount);
               Vector3 finalSwayPosition = new Vector3
                   (movementX, movementY, 0);
               transform.localPosition = Vector3.Lerp
                   (transform.localPosition, finalSwayPosition +
                   _weaponScopes[_currentScope].aimPosition, Time.deltaTime * swaySmoothValue);
            float tiltX = Mathf.Clamp(movementX * rotationAmount, -maxRotationAmount, maxRotationAmount);
            float tiltY = Mathf.Clamp(movementY * rotationAmount, -maxRotationAmount, maxRotationAmount);
            Quaternion finalTiltRotation = Quaternion.Euler(new Vector3(tiltX, 0f, tiltY));
            transform.localRotation = Quaternion.Slerp(transform.localRotation, finalTiltRotation * initialTiltRotation, Time.deltaTime * smothRotation);
            gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView, _weaponScopes[_currentScope]._fov, 5f * Time.deltaTime);
        } else
        {
            gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView, 60f, 10f * Time.deltaTime);
           // _weaponsContainer.transform.localPosition = Vector3.Lerp (_weaponsContainer.transform.localPosition, new Vector3(0, 0,0), 5f * Time.deltaTime);
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
        } else if (type.Equals("muzzle"))
        {
            _weaponMuzzle[_currentMuzzle].gameObject.SetActive(false);
            _weaponMuzzle[id].gameObject.SetActive(true);
            _currentMuzzle = id;
            isSilenced = true;
        } else if (type.Equals("grip"))
        {
            if (_weaponGrips[_currentGrip]._grip)
            {
                _weaponGrips[_currentGrip]._grip.gameObject.SetActive(false);
            }
            _weaponGrips[id]._grip.gameObject.SetActive(true);
            _currentGrip = id;
            _anim.SetInteger("Grip", id);
        }
    }

    public void SwitchFiremode(string _fmode)
    {
        if (_fmode.Equals("FullAuto"))
        {
            _firemode = _firemodes.FullAuto;
        } else if (_fmode.Equals("Semi"))
        {
            _firemode = _firemodes.Semi;
        } else
        {
            _firemode = _firemodes.Burst;
        }
    }
    public void weaponShoot(bool _canShot, bool _shoot, Vector3 _position, Quaternion _rot, float _multiplier, bool isInterior, float _bulletForce)
    {
        _shooting = _shoot;
        weaponShootSound(_shoot, isInterior);
        if (_canShot)
        {
            if (_shoot)
            {
                _recoil.Fire(_multiplier);
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
                } else
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
                } else
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
                } else
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

    public void setAnimValue(int _input)
    {
        if (_anim)
        {
            if (_input == 6)
            {
                 _anim.SetBool("idle", true);
                 _anim.SetBool("walk", false);
                 _anim.SetBool("reload", false);
                _anim.SetBool("run", false);
                if (!isCrouched)
                {
                    _legsAnim.SetBool("idle", true);
                    _legsAnim.SetBool("walk", false);
                    _legsAnim.SetBool("walk backward", false);
                    _legsAnim.SetBool("strafe left", false);
                    _legsAnim.SetBool("strafe right", false);
                    _legsAnim.SetBool("Crouched", false);
                    _legsAnim.SetBool("CrouchBwd", false);
                    _legsAnim.SetBool("CrouchFwd", false);
                    _legsAnim.SetBool("CrouchLeft", false);
                    _legsAnim.SetBool("CrouchRight", false);
                } else
                {
                    _legsAnim.SetBool("Crouched", true);
                    _legsAnim.SetBool("CrouchBwd", false);
                    _legsAnim.SetBool("CrouchFwd", false);
                    _legsAnim.SetBool("CrouchLeft", false);
                    _legsAnim.SetBool("CrouchRight", false);
                    _legsAnim.SetBool("idle", false);
                    _legsAnim.SetBool("walk", false);
                    _legsAnim.SetBool("walk backward", false);
                    _legsAnim.SetBool("strafe left", false);
                    _legsAnim.SetBool("strafe right", false);
                }
                isMoving = false;
            }
            else if (_input == 0)
            {
                 _anim.SetBool("walk", true);
                 _anim.SetBool("idle", false);
                 _anim.SetBool("reload", false);
                _anim.SetBool("run", false);
                if (!isCrouched)
                {
                    _legsAnim.SetBool("walk", true);
                    _legsAnim.SetBool("idle", false);
                    _legsAnim.SetBool("walk backward", false);
                    _legsAnim.SetBool("strafe left", false);
                    _legsAnim.SetBool("strafe right", false);
                    _legsAnim.SetBool("Crouched", false);
                    _legsAnim.SetBool("CrouchBwd", false);
                    _legsAnim.SetBool("CrouchFwd", false);
                    _legsAnim.SetBool("CrouchLeft", false);
                    _legsAnim.SetBool("CrouchRight", false);
                }
                else
                {
                    _legsAnim.SetBool("Crouched", true);
                    _legsAnim.SetBool("CrouchBwd", false);
                    _legsAnim.SetBool("CrouchFwd", true);
                    _legsAnim.SetBool("CrouchLeft", false);
                    _legsAnim.SetBool("CrouchRight", false);
                    _legsAnim.SetBool("idle", false);
                    _legsAnim.SetBool("walk", false);
                    _legsAnim.SetBool("walk backward", false);
                    _legsAnim.SetBool("strafe left", false);
                    _legsAnim.SetBool("strafe right", false);
                }
                isMoving = true;
            }
            else if (_input == 2)
            {
                _anim.SetBool("walk", true);
                _anim.SetBool("idle", false);
                _anim.SetBool("reload", false);
                _anim.SetBool("run", false);
                if (!isCrouched)
                {
                    _legsAnim.SetBool("strafe left", true);
                    _legsAnim.SetBool("walk", false);
                    _legsAnim.SetBool("idle", false);
                    _legsAnim.SetBool("walk backward", false);
                    _legsAnim.SetBool("strafe right", false);
                    _legsAnim.SetBool("Crouched", false);
                    _legsAnim.SetBool("CrouchBwd", false);
                    _legsAnim.SetBool("CrouchFwd", false);
                    _legsAnim.SetBool("CrouchLeft", false);
                    _legsAnim.SetBool("CrouchRight", false);
                } else
                {
                    _legsAnim.SetBool("Crouched", true);
                    _legsAnim.SetBool("CrouchBwd", false);
                    _legsAnim.SetBool("CrouchFwd", false);
                    _legsAnim.SetBool("CrouchLeft", true);
                    _legsAnim.SetBool("CrouchRight", false);
                    _legsAnim.SetBool("idle", false);
                    _legsAnim.SetBool("walk", false);
                    _legsAnim.SetBool("walk backward", false);
                    _legsAnim.SetBool("strafe left", false);
                    _legsAnim.SetBool("strafe right", false);
                }
                isMoving = true;
            }
            else if (_input == 3)
            {
                 _anim.SetBool("walk", true);
                 _anim.SetBool("idle", false);
                 _anim.SetBool("reload", false);
                _anim.SetBool("run", false);
                if (!isCrouched)
                {
                    _legsAnim.SetBool("strafe right", true);
                    _legsAnim.SetBool("walk", false);
                    _legsAnim.SetBool("idle", false);
                    _legsAnim.SetBool("walk backward", false);
                    _legsAnim.SetBool("strafe left", false);
                    _legsAnim.SetBool("Crouched", false);
                    _legsAnim.SetBool("CrouchBwd", false);
                    _legsAnim.SetBool("CrouchFwd", false);
                    _legsAnim.SetBool("CrouchLeft", false);
                    _legsAnim.SetBool("CrouchRight", false);
                } else
                {
                    _legsAnim.SetBool("Crouched", true);
                    _legsAnim.SetBool("CrouchBwd", false);
                    _legsAnim.SetBool("CrouchFwd", false);
                    _legsAnim.SetBool("CrouchLeft", false);
                    _legsAnim.SetBool("CrouchRight", true);
                    _legsAnim.SetBool("idle", false);
                    _legsAnim.SetBool("walk", false);
                    _legsAnim.SetBool("walk backward", false);
                    _legsAnim.SetBool("strafe left", false);
                    _legsAnim.SetBool("strafe right", false);
                }
                isMoving = true;
            }
            else if (_input == 1)
            {
                _anim.SetBool("walk", true);
                _anim.SetBool("idle", false);
                _anim.SetBool("reload", false);
                _anim.SetBool("run", false);
                if (!isCrouched)
                {
                    _legsAnim.SetBool("walk backward", true);
                    _legsAnim.SetBool("walk", false);
                    _legsAnim.SetBool("idle", false);
                    _legsAnim.SetBool("strafe left", false);
                    _legsAnim.SetBool("strafe right", false);
                    _legsAnim.SetBool("Crouched", false);
                    _legsAnim.SetBool("CrouchBwd", false);
                    _legsAnim.SetBool("CrouchFwd", false);
                    _legsAnim.SetBool("CrouchLeft", false);
                    _legsAnim.SetBool("CrouchRight", false);
                } else
                {
                    _legsAnim.SetBool("Crouched", true);
                    _legsAnim.SetBool("CrouchBwd", true);
                    _legsAnim.SetBool("CrouchFwd", false);
                    _legsAnim.SetBool("CrouchLeft", false);
                    _legsAnim.SetBool("CrouchRight", false);
                    _legsAnim.SetBool("idle", false);
                    _legsAnim.SetBool("walk", false);
                    _legsAnim.SetBool("walk backward", false);
                    _legsAnim.SetBool("strafe left", false);
                    _legsAnim.SetBool("strafe right", false);
                }
                isMoving = true;
            }
            else if (_input == 4)
            {
                useGrip = true;
                _legsAnim.Play("Jump", -1, 0f);
                _anim.enabled = true;
            }
            else if (_input == 7 && _shooting)
            {
                useGrip = true;
                if (!isAiming)
                {
                    if (_currentGrip == 0)
                        _anim.Play("Fire", -1, 0f);
                    else if (_currentGrip == 1)
                        _anim.Play("FireGrip", -1, 0f);
                    else if (_currentGrip == 2)
                        _anim.Play("FireAngular", -1, 0f);
                    _anim.enabled = true;
                } else
                {
                    if (_currentGrip == 0 && !_anim.GetBool("walk"))
                        _anim.Play("Aim_Pose_Fire", -1, 0f);
                }
            } else if (_input == 10)
            {
                _weaponAudio.clip = _magout;
                _weaponAudio.Play();
                _anim.Play("reload", -1, 0f);
                _anim.enabled = true;
                StartCoroutine(reloadSound());
                useGrip = false;
            } else if (_input == 11)
            {
                useGrip = true;
                LocalRotator _rotator = _weaponsHolder.GetComponent<LocalRotator>();
                if (isCrouched)
                {
                    _rotator.yChange = -1.4f;
                    _rotator.zChange = -0.18f;
                    isCrouched = false;
                }
                else
                {
                    _rotator.zChange = -0.18f;
                    _rotator.yChange = -1f;
                    isCrouched = true;
                }
                _legsAnim.SetBool("Crouched", isCrouched);
            } else if (_input == 12)
            {
                _anim.SetBool("run", true);
                _anim.SetBool("idle", false);
                _anim.SetBool("walk", false);
                _anim.SetBool("reload", false);
            }
            if (!_weaponAudio.isPlaying && _input != 6)
            {
                _weaponAudio.clip = _spinAudio[Random.Range(0, _spinAudio.Length)];
                _weaponAudio.Play();
            } 
        }
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

    private IEnumerator reloadSound()
    {
        yield return new WaitForSeconds(_magout.length);
        _weaponAudio.clip = _magin;
        _weaponAudio.Play();

    }
}

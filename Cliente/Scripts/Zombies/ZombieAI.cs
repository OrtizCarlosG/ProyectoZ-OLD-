using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
public class ZombieAI : MonoBehaviour
{
    Animator _anim;
    public int id;
    public float health;
    public float maxHealth = 100f;
    public Transform _projectilePrefab;
    public Transform _shootOrigin;
    private string _zombieType;
    public Healthbar _healthbar;
    public Transform _rotFiles;
    [Header("Audio")]
    public AudioClip[] _idleSounds;
    public AudioClip[] _walkSounds;
    public AudioClip[] _runSounds;
    public AudioClip[] _attackSounds;
    public AudioClip[] _screamSound;
    public AudioClip[] _agonizeSound;
    public AudioClip[] _specialAttacks;
    public float _runPitch = 1f;
    public float _walkPitch = 1f;

    RagdollDismembermentVisual _dismemberment;
    AudioSource _audio;

    public RuntimeAnimatorController[] animator;

    public void Initialize(int _id, string type, int variant, bool hasHead, bool hasShoulderL, bool hasShoulderR, bool hasElbowL, bool hasElbowR, bool hasUpperLegL, bool hasUpperLegR, bool hasLowerLegL, bool hasLowerLegR, bool isRotten)
    {
        id = _id;
        _zombieType = type;
        health = maxHealth;
        _anim = this.GetComponent<Animator>();
        _audio = this.GetComponent<AudioSource>();
        _anim.runtimeAnimatorController = animator[variant];
        _dismemberment = GetComponent<RagdollDismembermentVisual>();
        if (!hasHead)
            _dismemberment.Dismember("Head");
        if (!hasShoulderL)
            _dismemberment.Dismember("ShoulderL");
        if (!hasShoulderR)
            _dismemberment.Dismember("ShoulderR");
        if (!hasElbowL)
            _dismemberment.Dismember("Elbow_L");
        if (!hasElbowR)
            _dismemberment.Dismember("Elbow_R");
        if (!hasUpperLegL)
            _dismemberment.Dismember("UpperLegL");
        if (!hasUpperLegR)
            _dismemberment.Dismember("UpperLegR");
        if (!hasLowerLegL)
            _dismemberment.Dismember("LowerLegL");
        if (!hasLowerLegR)
            _dismemberment.Dismember("LowerLegR");
        if (isRotten)
            RotFiles();
    }

    public void SetHealth(float _health)
    {
        health = _health;
        if (_healthbar)
             _healthbar.setHealth(_health);
        if (health <= 0f)
        {
            GameManager.enemies.Remove(id);
            Destroy(gameObject);
            //_anim.Play("die", -1, 0f);
            //StartCoroutine(dieCouroutine());
        }
    }

    public void reciveAnimation(int _key)
    {
        if (_anim)
        {
            float _distance = Vector3.Distance(transform.position, CameraShaker.Instance.gameObject.transform.position);
            if (_key == 1)
            {
                _anim.SetBool("idle", true);
                _anim.SetBool("Chasing", false);
                _anim.SetBool("Running", false);
                _anim.SetBool("Eating", false);
                if (!_audio.isPlaying)
                {
                    _audio.clip = _idleSounds[Random.Range(0, _idleSounds.Length)];
                    _audio.Play();
                }
            } else if (_key == 2)
            {
                _anim.SetBool("idle", false);
                _anim.SetBool("Chasing", true);
                _anim.SetBool("Running", false);
                _anim.SetBool("Eating", false);
                if (_distance < 15 && _zombieType.Equals("GiantZombie"))
                {
                    CameraShaker.Instance.ShakeOnce(4f - _distance / 6f, 1f, .1f, 1f);
                }
                if (!_audio.isPlaying)
                {
                    _audio.clip = _walkSounds[Random.Range(0, _walkSounds.Length)];
                    _audio.pitch = _walkPitch;
                    _audio.Play();
                }
            } else if (_key == 3)
            {
                _anim.SetBool("idle", false);
                _anim.SetBool("Chasing", false);
                _anim.SetBool("Running", true);
                _anim.SetBool("Eating", false);
                if (!_audio.isPlaying)
                {
                    _audio.clip = _runSounds[Random.Range(0, _runSounds.Length)];
                    _audio.pitch = _runPitch;
                    _audio.Play();
                }
            } else if (_key == 4)
            {
                _anim.SetBool("idle", false);
                _anim.SetBool("Chasing", false);
                _anim.SetBool("Running", false);
                _anim.SetBool("Eating", true);
            } else if (_key == 5)
            {
                _anim.SetTrigger("Attack");
                if (_distance < 3 && _zombieType.Equals("GiantZombie"))
                {
                    CameraShaker.Instance.ShakeOnce(2f, 1f, .1f, 1f);
                }
                if (!_audio.isPlaying)
                {
                    _audio.clip = _attackSounds[Random.Range(0, _attackSounds.Length)];
                    _audio.Play();
                }
            } else if (_key == 6)
            {
                _anim.SetBool("Crawling", true);
                _anim.SetBool("Chasing", false);
                _anim.SetBool("Running", false);
                _anim.SetBool("idle", false);
            } else if (_key == 7)
            {
                _anim.SetBool("StartCrawl", true);
                _anim.SetBool("Chasing", false);
                _anim.SetBool("Running", false);
                _anim.SetBool("idle", false);

            } else if (_key == 8)
            {
                _anim.Play("Scream", -1, 0f);
               // _audio.clip = _screamSound[Random.Range(0, _screamSound.Length)];
               // _audio.pitch = _walkPitch;
               // _audio.Play();
            } else if (_key == 9)
            {
                _anim.Play("Agonizing", -1, 0f);
              //_audio.clip = _agonizeSound[Random.Range(0, _agonizeSound.Length)];
              //_audio.pitch = _walkPitch;
              //_audio.Play();
            }
        }
    }

    public void shootProjectile(Vector3 _position, Quaternion _rotation, float time, string type, Vector3 _direction)
    {
        if (type == "GiantZombie")
        {
            _anim.Play("Throw", -1, 0f);
            StartCoroutine(RockAttack(_direction, time));
        } else if (type == "Charger")
        {
            StartCoroutine(ChargerAttack(5f));
        }
    }

    private IEnumerator RockAttack(Vector3 _direction, float time)
    {
        var bullet = (Transform)Instantiate(
                           _projectilePrefab,
                          _shootOrigin.transform.position,
                           Quaternion.LookRotation(_direction));
        bullet.SetParent(_shootOrigin);

        yield return new WaitForSeconds(time);
        Vector3 aim = (_direction - _shootOrigin.transform.position).normalized;
        float _distance = Vector3.Distance(CameraShaker.Instance.gameObject.transform.position, transform.position);
        if (_distance < 10f)
        {
            CameraShaker.Instance.ShakeOnce(4f, 4f, .1f, 1f);
        }
        bullet.transform.gameObject.AddComponent<Rigidbody>();
        bullet.SetParent(null);
        bullet.GetComponent<Rigidbody>().velocity =
                        aim * (_distance + 8);

    }

    private IEnumerator ChargerAttack(float time)
    {
        _anim.SetBool("Charge", true);
        _anim.SetBool("Chasing", false);
        _anim.SetBool("idle", false);
        _anim.SetBool("Running", false);
        _anim.SetBool("Eating", false);
        _audio.pitch = 1.8f;
        yield return new WaitForSeconds(time);
        _anim.SetBool("Charge", false);
        _audio.pitch = 1f;
    }


    public void Dismember(string part)
    {
        _dismemberment.Dismember(part);
    }
    private void Update()
    {
        if (_zombieType.Equals("GiantZombie"))
        {
            AnimatorStateInfo _state = _anim.GetCurrentAnimatorStateInfo(0);
            if (_state.IsName("Charge"))
            {
                float _distance = Vector3.Distance(transform.position, CameraShaker.Instance.gameObject.transform.position);
                if (_distance < 30f)
                {
                    CameraShaker.Instance.ShakeOnce(0.3f, 0.3f, .1f, 1f);
                }
            }
        }
    }

    public void RotFiles()
    {
        var files = (Transform)Instantiate(
                          _rotFiles,
                         new Vector3(transform.localPosition.x, transform.localPosition.y +1.4f, transform.localPosition.z),
                          new Quaternion());
        files.SetParent(transform);
    }



}

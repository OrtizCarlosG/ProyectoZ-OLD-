using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardWeapon : MonoBehaviour
{

    public Transform _muzzle;
    public AudioClip _gunFire;
    public AudioClip _tail;
    public AudioClip _silencedFire;
    public AudioClip _silencedTail;
    public Transform _bulletPrefab;
    public bool isSilenced = false;
    AudioSource _audio;
    public void shotWeapon(bool _shooting, Quaternion _rotation)
    {
        if (_shooting)
        {
            if (!_audio.isPlaying)
            {
                if (!isSilenced)
                {
                    _audio.clip = _gunFire;
                }
                else
                {
                    _audio.clip = _silencedFire;
                }
                _audio.loop = true;
                _audio.Play();
            }
            else if (_audio.clip == _tail || _audio.clip == _silencedTail)
            {
                if (!isSilenced)
                {
                    _audio.clip = _gunFire;
                }
                else
                {
                    _audio.clip = _silencedFire;
                }
                _audio.loop = true;
                _audio.Play();
            }
            var bullet = (Transform)Instantiate(
                                _bulletPrefab,
                                _muzzle.transform.position,
                                _rotation);

            bullet.GetComponent<Rigidbody>().velocity =
                        bullet.transform.forward * 250;
        }
        else
        {
            if (_audio.clip == _gunFire || _audio.clip == _silencedFire)
            {
                if (!isSilenced)
                {
                    _audio.clip = _tail;
                }
                else
                {
                    _audio.clip = _silencedTail;
                }
                _audio.loop = false;
                _audio.Play();
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
       _audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

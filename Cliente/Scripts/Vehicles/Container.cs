using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioClip _landSound;
    public AudioClip[] _containerSound;

    AudioSource _audio;
    void Start()
    {
        _audio = this.GetComponent<AudioSource>();
    }

    public void setContainer(bool destroy, Vector3 _position)
    {
        transform.position = _position;
        if (!destroy)
        {
            if (!_audio.isPlaying)
            {
                _audio.clip = _containerSound[Random.Range(0, _containerSound.Length)];
                _audio.Play();
                _audio.volume = 0.5f;
            }
        }
        else
        {
            _audio.volume = 1f;
            _audio.clip = _landSound;
            _audio.loop = false;
            _audio.Play();
            Destroy(gameObject, 0.5f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ButtonSound : MonoBehaviour, IPointerEnterHandler
{

    [SerializeField]public AudioClip _sound;

    AudioSource _audio;
    // Start is called before the first frame update
    void Start()
    {
        _audio = transform.gameObject.GetComponent<AudioSource>();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_audio)
        {
            _audio.clip = _sound;
            _audio.Play();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

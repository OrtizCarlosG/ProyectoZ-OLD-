using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SafeZone : MonoBehaviour
{
    Animator _anim;

    public Text _SafeZoneMessage;
    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void showSafeZone(string _message, int _state)
    {
        if (_state == 0)
        {
            _anim.Play("SafeZoneEnter", -1, 0f);
        }
        else
        {
            
            _anim.Play("SafeZoneLeave", -1, 0f);
        }
        _SafeZoneMessage.text = _message;
    }
}

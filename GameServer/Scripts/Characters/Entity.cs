using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public string name;
    Animator _anim;
    private void Start()
    {
        _anim = GetComponent<Animator>();
    }
}

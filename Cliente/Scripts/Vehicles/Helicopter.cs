using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform _helice;
    public Transform _heliceTrasera;
    public Container _container;
    [HideInInspector] public int id;

    [HideInInspector]public Vector3 _destination;


    public void Initialize(int _id, Vector3 destination)
    {
        id = _id;
        _destination = destination;
        
    }

    // Update is called once per frame
    void Update()
    {
        _helice.transform.Rotate(Vector3.down * Time.deltaTime * 1000);
        _heliceTrasera.transform.Rotate(Vector3.left * Time.deltaTime * 1000);
    }
}

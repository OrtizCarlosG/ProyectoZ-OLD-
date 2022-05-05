using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHelicopter : MonoBehaviour
{
    // Start is called before the first frame update

    [HideInInspector] public static List<BossHelicopter> _helicopters = new List<BossHelicopter>();

    public Vector3 _destination;
    public float _speed;
    public Transform box;

    private Rigidbody HelicopterModel;
    private float EngineForce = 0f;
    [HideInInspector] public int id = 0;
    private static int nextID = 1;
    void Start()
    {
        HelicopterModel = gameObject.GetComponent<Rigidbody>();
        id = nextID;
        nextID++;
        _helicopters.Add(this);
        ServerSend.SpawnHelicopter(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.localPosition, _destination, Time.deltaTime * _speed);
        // transform.LookAt(_destination);
        var rotation = Quaternion.LookRotation(_destination - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _speed);
        if (transform.position == _destination)
        {
            if (box)
            {
                if (!box.gameObject.GetComponent<Rigidbody>())
                    box.gameObject.AddComponent<Rigidbody>();
                box.SetParent(null);
            }
        }
        ServerSend.HelicopterEvent(id, transform.position, transform.rotation);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHeadLossExample : MonoBehaviour
{

    private RagdollDismembermentVisual _dismemberment;
    private CharacterJoint _joint;
    // Start is called before the first frame update
    void Start()
    {
        _dismemberment = GetComponentInParent<RagdollDismembermentVisual>();
        _joint = GetComponent<CharacterJoint>();
        _joint.breakForce = 0f;
    }


    private void OnJointBreak(float breakForce)
    {
        _dismemberment.Dismember("Head");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

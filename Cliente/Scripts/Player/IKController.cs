using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    public static IKController instance;

    Animator anim;

    [Header("Right Hand IK")]
    [Range(0, 1)] public float rightHandWeight;
    public Transform rightHandObj = null;
    public Transform rightHandHint = null;

    [Header("Left Hand IK")]
    [Range(0, 1)] public float leftHandWeight;
    public Transform leftHandObj = null;
    public Transform leftHandHint = null;

    public GameObject leftHandIKGoal = null;
    public GameObject rightHandIKGoal = null;

    void Awake()
    {
        instance = this;
    }

    protected GameObject CreateIKGoal(string name)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = name;
        obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        return obj;
    }

    void Start()
    {
        anim = GetComponent<Animator>();

        if (leftHandIKGoal == null) leftHandIKGoal = CreateIKGoal("LeftHandGoal");
        if (rightHandIKGoal == null) rightHandIKGoal = CreateIKGoal("RightHandGoal");
    }

    private void OnAnimatorIK()
    {
        if (anim)
        {

            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKGoal.transform.position);
            //anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKGoal.transform.rotation);

            anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandIKGoal.transform.position);
            //anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandIKGoal.transform.rotation);

            // if (rightHandObj != null)
            // {
            //     anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
            //     anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
            //     anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
            //     anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
            // }
            //
            // if (rightHandHint != null)
            // {
            //     anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
            //     anim.SetIKHintPosition(AvatarIKHint.RightElbow, rightHandHint.position);
            // }


            if (leftHandObj != null)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
            }

            if (rightHandHint != null)
            {
                anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);
                anim.SetIKHintPosition(AvatarIKHint.LeftElbow, leftHandHint.position);
            }
        }
    }
}

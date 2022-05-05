using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [Header("Recoil_Transform")]
    public Transform RecoilPositionTranform;
    public Transform RecoilRotationTranform;
    public Transform _Camera;
    public Transform _container;
    [Space(10)]
    [Header("Recoil_Settings")]
    public float PositionDampTime;
    public float RotationDampTime;
    [Space(10)]
    public float Recoil1;
    public float Recoil2;
    public float Recoil3;
    public float Recoil4;
    [Space(10)]
    public Vector3 RecoilRotation;
    public Vector3 RecoilKickBack;

    public Vector3 RecoilRotation_Aim;
    public Vector3 RecoilKickBack_Aim;
    public Vector3 RecoilRotation_AimWalk;
    public Vector3 RecoilKickBack_AimWalk;
    [Space(10)]
    public Vector3 CurrentRecoil1;
    public Vector3 CurrentRecoil2;
    public Vector3 CurrentRecoil3;
    public Vector3 CurrentRecoil4;
    [Space(10)]
    public Vector3 RotationOutput;
    private Vector3 _initialPosition;
    [Space(10)]
    public float _walkSway = 0.1f;
    public float _walkSwayX = 0.1f;
    public float _walkSwayY = 0.1f;
    public float _MaxwalkSwayX = 0.1f;
    public float _MaxwalkSwayY = 0.1f;
    public bool walk;

    public bool aim;

    private void Start()
    {
        _initialPosition = transform.localPosition;
    }

    void FixedUpdate()
    {
        CurrentRecoil1 = Vector3.Lerp(CurrentRecoil1, Vector3.zero, Recoil1 * Time.deltaTime);
        CurrentRecoil2 = Vector3.Lerp(CurrentRecoil2, CurrentRecoil1, Recoil2 * Time.deltaTime);
        CurrentRecoil3 = Vector3.Lerp(CurrentRecoil3, Vector3.zero, Recoil3 * Time.deltaTime);
        CurrentRecoil4 = Vector3.Lerp(CurrentRecoil4, CurrentRecoil3, Recoil4 * Time.deltaTime);

        
        RecoilPositionTranform.localPosition = Vector3.Slerp(RecoilPositionTranform.localPosition, CurrentRecoil3, PositionDampTime * Time.fixedDeltaTime);
        RotationOutput = Vector3.Slerp(RotationOutput, CurrentRecoil1, RotationDampTime * Time.fixedDeltaTime);
        RecoilRotationTranform.localRotation = Quaternion.Euler(RotationOutput);
        if (aim)
        {
            //_Camera.localPosition = Vector3.Slerp(RecoilPositionTranform.localPosition, CurrentRecoil3, PositionDampTime * Time.fixedDeltaTime);
            _Camera.localRotation = Quaternion.Euler(RotationOutput);

        }
    }
    public void Fire(float _multiplier)
    {

        if (aim == true)
        {

            CurrentRecoil1 += new Vector3(RecoilRotation_Aim.x, Random.Range(-RecoilRotation_Aim.y, RecoilRotation_Aim.y) * _multiplier, Random.Range(-RecoilRotation_Aim.z, RecoilRotation_Aim.z) * _multiplier);
            CurrentRecoil3 += new Vector3(Random.Range(-RecoilKickBack_Aim.x, RecoilKickBack_Aim.x) * _multiplier, Random.Range(-RecoilKickBack_Aim.y, RecoilKickBack_Aim.y) * _multiplier, RecoilKickBack_Aim.z);
        }
        if (aim == false)
        {

            CurrentRecoil1 += new Vector3(RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotation.y) * _multiplier, Random.Range(-RecoilRotation.z, RecoilRotation.z) * _multiplier);
            CurrentRecoil3 += new Vector3(Random.Range(-RecoilKickBack.x, RecoilKickBack.x) * _multiplier, Random.Range(-RecoilKickBack.y, RecoilKickBack.y) * _multiplier, RecoilKickBack.z);
        }
    }
}

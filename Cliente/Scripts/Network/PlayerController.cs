using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform camTransform;

    public Transform _spine;

    private void Start()
    {
    }

    private void Update()
    {
       // if (Input.GetKey(KeyCode.Mouse0))
       // {
       //     ClientSend.PlayerShoot(camTransform.forward, true);
       // } else  if (Input.GetKeyUp(KeyCode.Mouse0))
       // {
       //     ClientSend.PlayerShoot(camTransform.forward, false);
       // }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ClientSend.PlayerThrowItem(camTransform.forward);
        }
    }

    private void FixedUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            SendInputToServer();
        }
    }

    /// <summary>Sends player input to the server.</summary>
    private void SendInputToServer()
    {
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space),
            Input.GetKey(KeyCode.Alpha1),
            Input.GetKey(KeyCode.Alpha2),
            Input.GetKey(KeyCode.R),
            Input.GetKey(KeyCode.E),
            Input.GetKey(KeyCode.V),
            Input.GetKey(KeyCode.C),
            Input.GetKey(KeyCode.Z),
            Input.GetKey(KeyCode.LeftShift)
    };

        ClientSend.PlayerMovement(_inputs, _spine.localRotation);
    }
}
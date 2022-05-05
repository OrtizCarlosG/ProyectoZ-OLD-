using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interior : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        Player _player = other.transform.GetComponent<Player>();
        if (_player)
            _player.isInInterior = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Player _player = other.transform.GetComponent<Player>();
        if (_player)
            _player.isInInterior = false;
    }
}

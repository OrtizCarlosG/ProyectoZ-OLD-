using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{

    public static List<SafeZone> _safeZoneList = new List<SafeZone>();

    public string enterMessage = "Entrando a la zona segura";
    public string exitMessage = "saliendo de la zona segura";
    public string _safeZoneName = "El Tunel";
    public int _safeZoneLevel = 1;
    public bool isUnderAttack = false;

    public List<Transform> objectsInZone = new List<Transform>();
    public List<Transform> getZoneObjects() { return objectsInZone; }

    private void OnTriggerEnter(Collider other)
    {
        Player _player = other.transform.gameObject.GetComponent<Player>();
        if (_player)
        {
            string message = $"{enterMessage}: {_safeZoneName}";
            ServerSend.SafeZone(_player.id, message, 0);
            _player.isInSafeZone = true;
            Debug.Log($"Player: {_player.name} joined the safezone {_safeZoneName}");
        }
        if (other.tag.Equals("SafeZone Build"))
            if (!objectsInZone.Contains(other.transform)) { objectsInZone.Add(other.transform); }
    }

    private void OnTriggerExit(Collider other)
    {
        Player _player = other.transform.gameObject.GetComponent<Player>();
        if (_player)
        {
            string message = $"{exitMessage}: {_safeZoneName}";
            ServerSend.SafeZone(_player.id, message, 1);
            _player.isInSafeZone = false;
            Debug.Log($"Player: {_player.name} leaved the safezone {_safeZoneName}");
        }
        if (other.tag.Equals("SafeZone Build"))
            objectsInZone.Remove(other.transform);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("SafeZone Build"))
            if (!objectsInZone.Contains(other.transform)) { objectsInZone.Add(other.transform); }
    }
    // Start is called before the first frame update
    void Start()
    {
        _safeZoneList.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneThreading : MonoBehaviour
{
    public enum Zonetype
    {
        Low,
        Medium,
        High,
        Insane,
        BlackZone
    }

    public Zonetype _ZoneThreading;

    public List<ZombieSpot> _spots;

    public string _zoneName;

    public bool isExtension = false;

    private void Start()
    {
        foreach (ZombieSpot _spot in _spots)
            _spot._zoneThreading = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isExtension)
        {
            Player _player = other.transform.GetComponent<Player>();
            if (_player)
                ServerSend.ZoneThreat(_player.id, this);
        }
    }
}

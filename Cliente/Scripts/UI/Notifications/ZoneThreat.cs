using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ZoneThreat : MonoBehaviour
{

    public Text _zoneText;
    Animator _anim;
    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void displayZone(string _zone, string threat)
    {
        if (threat.Equals("Low"))
            _anim.Play("DisplayLow", -1, 0f);
        else if (threat.Equals("Medium"))
            _anim.Play("DisplayMedium", -1, 0f);
        else if (threat.Equals("High"))
            _anim.Play("DisplayHigh", -1, 0f);
        else if (threat.Equals("Insane"))
            _anim.Play("DisplayInsane", -1, 0f);
        else if (threat.Equals("BlackZone"))
            _anim.Play("DisplayBlackZone", -1, 0f);
        _zoneText.text = _zone;
    }
}

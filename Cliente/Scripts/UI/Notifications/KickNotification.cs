using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KickNotification : MonoBehaviour
{
    // Start is called before the first frame update

    public Text _reason;

    public void setMessage(string reason)
    {
        _reason.text = $"{reason}";
    }

    public void acceptKick()
    {
        Application.Quit();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

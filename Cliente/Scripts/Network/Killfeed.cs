using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Killfeed : MonoBehaviour
{

    public Text _style;
    public Text _style2;
    public Text _style3;
    public Text _style4;
    public Text _style5;
    public Text _style6;

    int lastFeed = 0;
    // Start is called before the first frame update
    void Start()
    {
        //_style.text = "<size=20>Charles killed Zombie boss with <color=red>SA-58</color> text</size>";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void displayFeed(string text)
    {
        if (lastFeed == 0)
        {
            _style.text = text;
            lastFeed++;
        } else if (lastFeed == 1)
        {
            if (!string.IsNullOrEmpty(_style2.text))
            {
                _style.text = _style2.text;
            }
            _style2.text = text;
            lastFeed++;
        } else if (lastFeed == 2)
        {
            if (!string.IsNullOrEmpty(_style3.text))
            {
                _style.text = _style2.text;
                _style2.text = _style3.text;
            }
            _style3.text = text;
            lastFeed++;
        }
        else if (lastFeed == 3)
        {
            if (!string.IsNullOrEmpty(_style4.text))
            {
                _style.text = _style2.text;
                _style2.text = _style3.text;
                _style3.text = _style4.text;
            }
            _style4.text = text;
            lastFeed++;
        }
        else if (lastFeed == 4)
        {
            if (!string.IsNullOrEmpty(_style5.text))
            {
                _style.text = _style2.text;
                _style2.text = _style3.text;
                _style3.text = _style4.text;
                _style4.text = _style5.text;
            }
            _style5.text = text;
            lastFeed++;
        }
        else if (lastFeed == 5)
        {
            if (!string.IsNullOrEmpty(_style6.text))
            {
                _style.text = _style2.text;
                _style2.text = _style3.text;
                _style3.text = _style4.text;
                _style4.text = _style5.text;
                _style5.text = _style6.text;
            }
            _style6.text = text;
        }
    }
}

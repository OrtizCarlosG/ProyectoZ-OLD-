using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractMenu : MonoBehaviour
{
    public Text _displayText;
    public Text _firemode;

    public void setDisplayTextValue(string _message)
    {
        if (!_message.Equals("None"))
        {
            _displayText.text = _message;
        }
        else
        {
            _displayText.text = "";
        }

    }

    public void SwitchFiremode(string _fmode)
    {
        _firemode.text = _fmode;
        StartCoroutine(_destroyFmode());

    }

    private IEnumerator _destroyFmode()
    {
        yield return new WaitForSeconds(2f);
        _firemode.text = "";
    }
}

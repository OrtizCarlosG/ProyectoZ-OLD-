using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Healthbar : MonoBehaviour
{
    // Start is called before the first frame update

    public Image _healthbar;
    public TextMeshProUGUI _txt;

    public void setHealth(float _health)
    {
        _healthbar.fillAmount = _health / 100f;
    }

    public void setName(string name)
    {
        if (_txt)
        _txt.text = name;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

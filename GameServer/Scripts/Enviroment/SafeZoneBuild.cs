using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZoneBuild : MonoBehaviour
{

    public float _health = 100f;
    public float _defenseLevel = 2f;

    public void TakeDamage(float damage)
    {
        if (_health <= 0)
            Destroy(gameObject);
        return;
        _health -= damage / _defenseLevel;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreBullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<BulletScript>())
        {
            Debug.Log("Bullet in head");
            Physics.IgnoreCollision(collision.transform.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}

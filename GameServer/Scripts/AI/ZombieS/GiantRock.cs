using UnityEngine;
using System.Collections;
public class GiantRock : MonoBehaviour
{
    // Start is called before the first frame update
    public float rockHealth = 200f;
    public float damage = 50f;
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>(), true);
        }

        if (collision.gameObject.GetComponent<Bullet>())
        {
            Bullet _bullet = collision.gameObject.GetComponent<Bullet>();
            rockHealth -= _bullet.bulletDamage;
            if (rockHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
        if (collision.gameObject.tag.Equals("Player"))
        {
            Player _player = collision.gameObject.GetComponent<Player>();
            if (_player)
                _player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

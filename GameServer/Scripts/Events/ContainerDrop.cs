using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerDrop : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform carga;
    public BossHelicopter _helicopter;
    public float containerHealth = 500f;

    bool destroy = false;
    bool Attacked = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Bullet>())
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>(), true);
            Bullet _bullet = collision.gameObject.GetComponent<Bullet>();
            containerHealth -= _bullet.bulletDamage;
            if (containerHealth <= 0 && !Attacked)
            {
                Attacked = true;
                gameObject.AddComponent<Rigidbody>();
                ServerSend.SendKillFeed($" <color=red>{_bullet._owner.name}</color> ha roto el container con {_bullet._owner.GetComponent<Player>()._currentSlot._weaponName}");
                transform.SetParent(null);
            }
        }
        else
        {
            destroy = true;
            _helicopter._destination = new Vector3(0, 36f);
            NetworkManager.instance.InstantiateEnemy(transform.position, 1);
            ServerSend.ContainerDrop(destroy, transform.position);
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ServerSend.ContainerDrop(destroy, transform.position);

        if (containerHealth <= 0)
            Physics.IgnoreCollision(GameObject.Find("Bullet_Prefab(Clone)").GetComponent<Collider>(), GetComponent<Collider>(), true);
    }
}

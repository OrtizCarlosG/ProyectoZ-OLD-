using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

	public Entity _owner;
	[Range(5, 100)]
	[Tooltip("After how long time should the bullet prefab be destroyed?")]
	public float destroyAfter;
	[Tooltip("If enabled the bullet destroys on impact")]
	public bool destroyOnImpact = false;
	[Tooltip("Minimum time after impact that the bullet is destroyed")]
	public float minDestroyTime;
	[Tooltip("Maximum time after impact that the bullet is destroyed")]
	public float maxDestroyTime;

	public float bulletDamage;

	[HideInInspector]public Vector3 _spawnPoint;

    private void Start()
    {
		//Start destroy timer
		StartCoroutine(DestroyAfter());
	}


	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "Enemy")
		{
			BodyDamage _damageMul = collision.transform.GetComponent<BodyDamage>();
			if (_damageMul)
			{
				ZombieInstance _zombie = _damageMul._Owner.GetComponent<ZombieInstance>();
				if (_zombie)
				{
					if (_zombie.hp > 0)
						if (_spawnPoint != null)
						{
							float _distance = Vector3.Distance(transform.position, _spawnPoint);
							float calDamage = (bulletDamage * _damageMul._damageMultiplier) - _distance / 50f;
							if (_owner.GetComponent<Player>())
								_zombie.TakeDamage(calDamage, _owner.GetComponent<Player>());
							else
								_zombie.TakeDamage(calDamage, _owner.GetComponent<Guard>());
						}
					if (_damageMul.bodyPartHealth <= 0)
					{
						//RagdollDismembermentVisual _dismemberment = _zombie.GetComponent<RagdollDismembermentVisual>();
						//_dismemberment.Dismember(_damageMul._bodypartName);
						_zombie.setBodyStatus(_damageMul._bodypartName);
						ServerSend.EnemyDismemberment(_zombie, _damageMul._bodypartName);
					}
					if (_damageMul._bodypartName.Equals("UpperLegL") || _damageMul._bodypartName.Equals("UpperLegR") || _damageMul._bodypartName.Equals("LowerLegL") || _damageMul._bodypartName.Equals("LowerLegR"))
					{
						if (_damageMul.bodyPartHealth <= 0 && _zombie.canCrawl)
						{
							_zombie.isCrawler = true;
							_zombie.playCrawl();
						}
					}
					_damageMul.bodyPartHealth -= bulletDamage;
				}
			}
			ServerSend.CreateDecal("blood", this.transform.position);
			Destroy(gameObject);
		}
		else if (collision.transform.tag == "Concrete")
		{
			ServerSend.CreateDecal("concrete", this.transform.position);
			Destroy(gameObject);
		}
		else if (collision.transform.tag == "Dirt")
		{
			ServerSend.CreateDecal("dirt", this.transform.position);
			Destroy(gameObject);
		}
		else if (collision.transform.tag == "Metal")
		{
			ServerSend.CreateDecal("metal", this.transform.position);
			Destroy(gameObject);
		}
		else if (collision.transform.tag == "Wood")
		{
			ServerSend.CreateDecal("wood", this.transform.position);
			Destroy(gameObject);
		}
		else if (collision.transform.tag == "Player")
		{
			BodyDamage _damageMul = collision.transform.GetComponent<BodyDamage>();
			if (_owner)
			{
				if (_owner != _damageMul._Owner.GetComponent<Player>())
				{
					if (_damageMul)
					{

						Player _player = _damageMul._Owner.GetComponent<Player>();
						if (_spawnPoint != null)
						{
							float _distance = Vector3.Distance(transform.position, _spawnPoint);
							float calDamage = (bulletDamage * _damageMul._damageMultiplier) - _distance / 50f;
							if (_owner.GetComponent<Player>())
								_player.TakeDamage(calDamage, _owner.GetComponent<Player>());
							else
								_player.TakeDamage(calDamage);

						}
					}
				}
				else
				{
					Physics.IgnoreCollision(collision.collider, GetComponent<Collider>(), true);
				}
				ServerSend.CreateDecal("blood", this.transform.position);
			} 
		}

		//If destroy on impact is false, start 
		//coroutine with random destroy timer
		if (!destroyOnImpact)
		{
			StartCoroutine(DestroyTimer());
		}
		//Otherwise, destroy bullet on impact
		else
		{
			Destroy(gameObject);
		}
	}



	private IEnumerator DestroyTimer()
	{
		//Wait random time based on min and max values
		yield return new WaitForSeconds
			(Random.Range(minDestroyTime, maxDestroyTime));
		//Destroy bullet object
		Destroy(gameObject);
	}

	private IEnumerator DestroyAfter()
	{
		//Wait for set amount of time
		yield return new WaitForSeconds(destroyAfter);
		//Destroy bullet object
		Destroy(gameObject);
	}

	public void setBulletDamage(float damage)
	{
		this.bulletDamage = damage;
	}

	public void setBulletOwner(Player _Bowner)
    {
		_owner = _Bowner;
    }
}

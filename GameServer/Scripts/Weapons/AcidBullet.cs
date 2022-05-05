using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidBullet : MonoBehaviour
{

	public ZombieInstance _owner;
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
	public Transform acidPrefab;

    private void Start()
    {
		//Start destroy timer
		StartCoroutine(DestroyAfter());
	}


	private void OnCollisionEnter(Collision collision)
	{ 

		 if (collision.transform.tag == "Concrete")
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
			if (_damageMul)
			{
				Player _player = _damageMul._Owner.GetComponent<Player>();
			}
			ServerSend.CreateDecal("blood", this.transform.position);
		}

		var acid = (Transform)Instantiate(
								  acidPrefab,
								  transform.position,
								  new Quaternion());

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

	public void setBulletOwner(ZombieInstance _Bowner)
    {
		_owner = _Bowner;
    }
}

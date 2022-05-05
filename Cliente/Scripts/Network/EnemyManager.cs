using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int id;
    public float health;
    public float maxHealth = 100f;
    public Transform _projectilePrefab;



    Animator _anim;

    public void Initialize(int _id)
    {
        id = _id;
        health = maxHealth;
        _anim = this.GetComponent<Animator>();
    }

    public void SetHealth(float _health)
    {
        health = _health;

        if (health <= 0f)
        {
            GameManager.enemies.Remove(id);
            _anim.Play("die", -1, 0f);
            StartCoroutine(dieCouroutine());
        }
    }

    IEnumerator dieCouroutine()
    {
        yield return new  WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length + _anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        Destroy(gameObject);
    }

    public void setAnimValue(string _state)
    {
        if (_state.Equals("idle"))
        {
            _anim.SetBool("idle", true);
            _anim.SetBool("chase", false);
            _anim.SetBool("walk", false);
        } else if (_state.Equals("patrol"))
        {
            _anim.SetBool("idle", false);
            _anim.SetBool("chase", false);
            _anim.SetBool("walk", true);
        } else if (_state.Equals("chase"))
        {
            _anim.SetBool("idle", false);
            _anim.SetBool("walk", false);
            _anim.SetBool("chase", true);
        }
    }

    public void shootProjectile(Vector3 _position, Quaternion _rotation)
    {
        var bullet = (Transform)Instantiate(
                           _projectilePrefab,
                           _position,
                           _rotation);

        bullet.GetComponent<Rigidbody>().velocity =
                    bullet.transform.forward * 250;
    }


}
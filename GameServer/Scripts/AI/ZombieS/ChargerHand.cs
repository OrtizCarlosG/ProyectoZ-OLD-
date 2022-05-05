using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerHand : MonoBehaviour
{

   public ZombieInstance _zombie;
    Animator _anim;
    private void Start()
    {
        _anim = _zombie.transform.gameObject.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
      if (other.transform.tag == "Player")
        {
            AnimatorStateInfo state = _anim.GetCurrentAnimatorStateInfo(0);
            if (_anim.GetBool("doCharge") || state.IsName("Attack"))
            {
                BodyDamage bodyPart = other.transform.GetComponent<BodyDamage>();
                if (bodyPart)
                {
                    CharacterController _controller = bodyPart._Owner.GetComponent<CharacterController>();
                    _controller.Move(_zombie.transform.forward * 15);
                }
                else
                {
                    Player _player = other.transform.GetComponent<Player>();
                    CharacterController _controller = _player.gameObject.GetComponent<CharacterController>();
                    if (_player)
                    {
                        _player.TakeDamage(25f);
                        _controller.Move(_zombie.transform.forward * 15);
                    }
                }
            }
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log(hit);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class Guard : Entity
{

    public static List<Guard> _guardList = new List<Guard>();

    public enum _weaponList
    {
        SA58 = 0,
        MK47 = 1,
        MP7 = 2,
        AK47 = 3,
        VSS = 4,
        ACR = 5,
        DVL = 6,
        HK416 = 7,
        SKS = 8,
        UMP = 9,
        MP5 = 10,
    }

    public _weaponList _weapon;
    public float AttackDistance = 120f, _fov, _wanderSpeed = 2f, _RotationSpeed = 4f, _minWanderCooldown = 0.2f, _maxWanderCooldown = 0.08f, missChance = 25.0f;
    public Transform[] _guardWanderPoints;
    public Transform _enemy;
    public Transform _viewPoint;
    public Transform bulletPrefab;
    public Transform _column;
    public Weapon _currentWeapon;
    public int _key = 0, _currentWander;
    private Animator _anim;
    private UnityEngine.AI.NavMeshAgent _agent;
    private bool isChasing = false;
    private float _lastWander, _WanderCooldown, _fireRate, _lastFire;
    public int _id;
    public static int nextID = 1;

    private void Start()
    {
        _WanderCooldown = Random.Range(_minWanderCooldown, _maxWanderCooldown);
         _anim = GetComponent<Animator>();
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _fireRate = 8.0f;
        _guardList.Add(this);
        _id = nextID;
        nextID++;
    }


    private void FixedUpdate()
    {
        if (_key == 0)
        {
            _anim.SetBool("idle", true);
            _anim.SetBool("walk", false);
        } else if (_key == 1)
        {
            _anim.SetBool("walk", true);
            _anim.SetBool("idle", false);
        } else if (_key == 2)
        {
            _anim.SetBool("walk backward", true);
            _anim.SetBool("walk", false);
            _anim.SetBool("idle", false);
        }
        GameObject Newenemy = GameObject.FindWithTag("Enemy");
        if (Newenemy)
        {
            _enemy = Newenemy.transform;
            checkView();
        } else
        {
            doWander();
        }
        if (!isChasing)
        {
            doWander();
        }
        if (reachedPathEnd())
        {
            _key = 0;
        }

        ServerSend.GuardPosition(this);
    }

    private bool isEnemy(Collider _object)
    {
        if (_object.tag.Equals("Enemy"))
            return true;
        if (_object.tag.Equals("Bandit"))
            return true;
        return false;
    }

    void checkView()
    {
        RaycastHit hit = new RaycastHit();
        Vector3 checkPosition = _enemy.position - _viewPoint.position;
        if (Vector3.Angle(checkPosition, _viewPoint.forward) < _fov)
        {
            if (Physics.Raycast(_viewPoint.position, checkPosition, out hit, AttackDistance))
            {
                if (isEnemy(hit.collider))
                {
                    ShotEnemy(hit.transform);
                    isChasing = true;
                }
            }
        }
        else
        {
            isChasing = false;
        }
    }

    bool reachedPathEnd()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0.0f)
            {
                return true;
            }
            return true;
        }
        return false;
    }

    private void doWander()
    {
        if ((Time.time - _lastWander > 1 / _WanderCooldown))
        {
            if (reachedPathEnd())
            {
                _WanderCooldown = Random.Range(_minWanderCooldown, _maxWanderCooldown);
                _agent.destination = _guardWanderPoints[_currentWander].position;
                _currentWander++;
                _key = 1;
                _lastWander = Time.time;
                _agent.speed = _wanderSpeed;
                if (_currentWander > _guardWanderPoints.Length - 1)
                    _currentWander = 0;
            }
        }
    }
    private void ShotEnemy(Transform _enemy)
    {
        RotateToPoint(_enemy);
        if ((Time.time - _lastFire > 1 / _fireRate))
        {
            float hitChance = Random.Range(0f, 100f);


            _lastFire = Time.time;
            var bullet = (Transform)Instantiate(
                                   bulletPrefab,
                                   _viewPoint.position,
                                   _viewPoint.rotation);
            float rotx = Random.Range(0f, 0f);
            float roty = Random.Range(0f, 0f);
            float rotz = Random.Range(0f, 0f);

            if (hitChance <= missChance)
            {
                rotx = Random.Range(-5f, 5f);
                roty = Random.Range(-5f, 5f);
                rotz = Random.Range(-5f, 5f);
            }

            bullet.Rotate(rotx, roty, rotz);
            Weapon _weaponDB = Database.getWeaponByID((int)_weapon);
            bullet.GetComponent<Rigidbody>().velocity =
                        bullet.transform.forward * _weaponDB._bulletForce;
            bullet.transform.gameObject.GetComponent<Bullet>().bulletDamage = _weaponDB._weaponDamage;
            bullet.transform.gameObject.GetComponent<Bullet>()._owner = this;
            bullet.transform.gameObject.GetComponent<Bullet>()._spawnPoint = transform.position;
            _fireRate = _weaponDB._fireRate;
            ServerSend.GuardShot(this, true);
        }
        ServerSend.GuardShot(this, false);
        //_anim.Play("Fire", -1, 0f);

    }

    public void RotateToPoint(Transform _target)
    {
         Quaternion _lookRotation;
         Vector3 _direction;
        _direction = (_target.position - transform.position).normalized;

        //create the rotation we need to be in to look at the target
        _lookRotation = Quaternion.LookRotation(_direction);

        //rotate us over time according to speed until we are in the required rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * _RotationSpeed);
        _viewPoint.LookAt(_target);
        //_column.rotation = Quaternion.Slerp(_column.rotation, _lookRotation, Time.deltaTime * _RotationSpeed);
        //Vector3 targetPoint = new Vector3(_target.transform.position.x, _target.position.y, _target.transform.position.z) - _viewPoint.position;
        //Quaternion targetRotation = Quaternion.LookRotation(targetPoint);
        //_viewPoint.localRotation = targetRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isEnemy(other))
        {
            RotateToPoint(other.transform);
            ShotEnemy(other.transform);
            _agent.SetDestination(other.transform.position);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isEnemy(other))
        {
            RotateToPoint(other.transform);
            ShotEnemy(other.transform);
            _agent.SetDestination(other.transform.position);
        }
    }

}

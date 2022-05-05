using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpot : MonoBehaviour
{

    [HideInInspector] public static List<ZombieInstance> _zombies = new List<ZombieInstance>();
    public int maxEnemyInSpot = 1;
    public ZoneThreading _zoneThreading;
    [HideInInspector]public int inSpot = 0;
    public float spawnTime = 3f;

    public static ZombieSpot instance;
    public bool spawnRandomPos = false;
    public float minX, minZ;
    public float maxX, maxZ;
    Vector3 Min, Max;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        int randomSkin = Random.Range(2, 48);
        SetRanges();
        float _xAxis = UnityEngine.Random.Range(minX, maxX);
        float _zAxis = UnityEngine.Random.Range(minZ, maxZ);
        _xAxis = Random.Range(_xAxis, -_xAxis);
        _zAxis = Random.Range(_zAxis, -_zAxis);
        Vector3 _randomPosition = new Vector3(transform.position.x + _xAxis, 1f, transform.position.z  + _zAxis);
        yield return new WaitForSeconds(spawnTime);

        if (inSpot < maxEnemyInSpot)
        {
            inSpot++;
            if (NetworkManager.instance.enemyPrefab[2].GetComponent<ZombieInstance>().canRespawn)
            {
                int _zombieLevel = 1;
                bool canRun = false;
                if (_zoneThreading._ZoneThreading.ToString().Equals("Low"))
                {
                    _zombieLevel = 1;
                }
                else if (_zoneThreading._ZoneThreading.ToString().Equals("Medium"))
                {
                    _zombieLevel = Random.Range(1, 2);
                    float runChance = Random.Range(0f, 1f);
                    if (runChance < .33)
                        canRun = true;
                }
                else if (_zoneThreading._ZoneThreading.ToString().Equals("High"))
                {
                    _zombieLevel = Random.Range(1, 3);
                    float runChance = Random.Range(0f, 1f);
                    if (runChance < .5)
                        canRun = true;
                }
                else if (_zoneThreading._ZoneThreading.ToString().Equals("Insane"))
                {
                    _zombieLevel = 3;
                    float runChance = Random.Range(0f, 1f);
                    if (runChance < .75)
                        canRun = true;
                }
                else if (_zoneThreading._ZoneThreading.ToString().Equals("BlackZone"))
                {
                    _zombieLevel = 3;
                    canRun = true;
                }
                ZombieInstance _zombie = NetworkManager.instance.InstantiateEnemy(_randomPosition, randomSkin).GetComponent<ZombieInstance>();
                _zombie.ZombieLevel = _zombieLevel;
                _zombie.runnerZombie = canRun;
                _zombie._spot = this;
            }
        }
        StartCoroutine(SpawnEnemy());
    }

    private void SetRanges()
    {
        Min = new Vector3(minX, 1, minZ); //Random value.
        Max = new Vector3(maxX, 1, maxZ); //Another ramdon value, just for the example.
    }
}

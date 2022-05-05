using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public float timeSpeed = 0.01f;
    public GameObject playerPrefab;
    public GameObject[] enemyPrefab;
    public GameObject projectilePrefab;
    public GameObject _Helicopter;

    public Transform _spawnPoint;
    [SerializeField, Range(0, 1)] private float TimeOfDay;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(50, 26950);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, _spawnPoint.position, Quaternion.identity).GetComponent<Player>();
    }

    public GameObject InstantiateEnemy(Vector3 _position, int id)
    {
       return Instantiate(enemyPrefab[id], _position, Quaternion.identity);
        
    }

    public Projectile InstantiateProjectile(Transform _shootOrigin)
    {
        return Instantiate(projectilePrefab, _shootOrigin.position + _shootOrigin.forward * 0.7f, Quaternion.identity).GetComponent<Projectile>();
    }

    public GameObject InstantiateHeli(Vector3 _position)
    {
        return Instantiate(_Helicopter, _position, Quaternion.identity);
    }

    private void Update()
    {
        float elapsedTime = Time.time - 0.0f; // startTime can be equal to 0
        float output = Mathf.PingPong(2 * elapsedTime / timeSpeed, 1.0f);
        TimeOfDay = output;
        ServerSend.SendServerTime(output);
    }
}
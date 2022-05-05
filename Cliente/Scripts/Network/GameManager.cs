using System;
using System.Collections;
using System.Collections.Generic;
using ICWM;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, ItemSpawner> itemSpawners = new Dictionary<int, ItemSpawner>();
    public static Dictionary<int, ProjectileManager> projectiles = new Dictionary<int, ProjectileManager>();
    public static Dictionary<int, ZombieAI> enemies = new Dictionary<int, ZombieAI>();
    public static Dictionary<int, Helicopter> _helicopters = new Dictionary<int, Helicopter>();
    public static Dictionary<int, Guard> _guards = new Dictionary<int, Guard>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject itemSpawnerPrefab;
    public GameObject projectilePrefab;
    public GameObject[] enemyPrefab;
    public GameObject guardPrefab;
    public GameObject heliPrefab;
    public Killfeed _killfeed;
    public NotificationManager _notificationManager;
    public InteractMenu _interactMenu;
    public ChatWindow _chatMenu;


    public Transform _bloodDecal;
    public Transform _dirtDecal;
    public Transform _concreteDecal;
    public Transform _metalDecal;
    public Transform _woodDecal;

    public AudioClip _bloodClip;
    public AudioClip _dirtClip;
    public AudioClip _concreteClip;
    public AudioClip _metalClip;
    public AudioClip _woodClip;

    //Scene References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;

    public PolyverseSkies _sky;
    public Healthbar _localHealthbar;
    //Variables

    private void Start()
    {
        Client.instance.ConnectToServer();
        ClientSend.SendIntoGame();
    }

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
   
        if (DirectionalLight != null)
            return;
   
        //Search for lighting tab sun
        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        //Search scene for light that fits criteria (directional)
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }

    public void setTimeOfDay(float _time)
    {
        _sky.timeOfDay = _time;
       // if (Preset == null)
       //     return;
       //UpdateLighting(_time / 1f);
    }

   private void UpdateLighting(float timePercent)
   {
       //Set ambient and fog
       RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
       RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);
   
       //If the directional light is set then rotate and set it's color, I actually rarely use the rotation because it casts tall shadows unless you clamp the value
       if (DirectionalLight != null)
       {
           DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
   
           DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
       }
   
   }

    /// <summary>Spawns a player.</summary>
    /// <param name="_id">The player's ID.</param>
    /// <param name="_name">The player's name.</param>
    /// <param name="_position">The player's starting position.</param>
    /// <param name="_rotation">The player's starting rotation.</param>
    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation, int _weaponID)
    {
        GameObject _player;
        if (_id == Client.instance.myId)
        {
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
            InventoryBase.instance.m_Player = _player;
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        _player.GetComponent<PlayerManager>().Initialize(_id, _username, _weaponID);
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

    public void CreateItemSpawner(int _spawnerId, Vector3 _position, bool _hasItem)
    {
        GameObject _spawner = Instantiate(itemSpawnerPrefab, _position, itemSpawnerPrefab.transform.rotation);
        _spawner.GetComponent<ItemSpawner>().Initialize(_spawnerId, _hasItem);
        itemSpawners.Add(_spawnerId, _spawner.GetComponent<ItemSpawner>());
    }

    public void SpawnHelicopter(int _id, Vector3 _position, Vector3 _destination)
    {
        GameObject _helicopter = Instantiate(heliPrefab, _position, new Quaternion());
        _helicopter.GetComponent<Helicopter>().Initialize(_id, _destination);
        _helicopters.Add(_id, _helicopter.GetComponent<Helicopter>());
        _notificationManager.ShowHelicopter();
    }

    public void SpawnProjectile(int _id, Vector3 _position)
    {
        GameObject _projectile = Instantiate(projectilePrefab, _position, Quaternion.identity);
        _projectile.GetComponent<ProjectileManager>().Initialize(_id);
        projectiles.Add(_id, _projectile.GetComponent<ProjectileManager>());
    }

    public void SpawnEnemy(int _id, Vector3 _position, int skinID, string _type, int variant, bool hasHead, bool hasShoulderL, bool hasShoulderR, bool hasElbowL, bool hasElbowR, bool hasUpperLegL, bool hasUpperLegR, bool hasLowerLegL, bool hasLowerLegR, bool isRotten)
    {

        GameObject _enemy = Instantiate(enemyPrefab[skinID], _position, Quaternion.identity);
        _enemy.GetComponent<ZombieAI>().Initialize(_id, _type, variant, hasHead, hasShoulderL, hasShoulderR, hasElbowL, hasElbowR, hasUpperLegL, hasUpperLegR, hasLowerLegL, hasLowerLegR, isRotten);
        enemies.Add(_id, _enemy.GetComponent<ZombieAI>());
    }

    public void SpawnGuard(int  _id, Vector3 _position, string _weapon, string _name)
    {
        GameObject _guard = Instantiate(guardPrefab, _position, Quaternion.identity);
        _guard.GetComponent<Guard>().Initialize(_id, _name, (Guard._weaponList) Enum.Parse(typeof(Guard._weaponList), _weapon));
        _guards.Add(_id, _guard.GetComponent<Guard>());
    }

    public void InstantiateItem(Vector3 _position)
    {
        Instantiate(itemSpawnerPrefab, _position, Quaternion.identity);
    }
    public void CreateDecal (string _decal, Vector3 _position)
    {
        Transform _decalType = instance._metalDecal;
        AudioSource _audio = _decalType.GetComponent<AudioSource>();
        if (_decal == "blood")
        {
            _decalType = instance._bloodDecal;
            _audio = _decalType.GetComponent<AudioSource>();
            _audio.clip = instance._bloodClip;
        }
        else if (_decal == "concrete")
        {
            _decalType = instance._concreteDecal;
            _audio = _decalType.GetComponent<AudioSource>();
            _audio.clip = instance._concreteClip;
        }
        else if (_decal == "dirt")
        {
            _decalType = instance._dirtDecal;
            _audio = _decalType.GetComponent<AudioSource>();
            _audio.clip = instance._dirtClip;
        }
        else if (_decal == "metal")
        {
            _decalType = instance._metalDecal;
            _audio = _decalType.GetComponent<AudioSource>();
            _audio.clip = instance._metalClip;
        }
        else if (_decal == "wood")
        {
            _decalType = instance._woodDecal;
            _audio = _decalType.GetComponent<AudioSource>();
            _audio.clip = instance._woodClip;
        }
        var decal = (Transform)Instantiate(
                          _decalType,
                          _position,
                          new Quaternion(0f, 0f, 0f, 0f));
        _audio.enabled = true;
        _audio.Play();

        StartCoroutine(deleteDecal(decal, _audio));
    }

    private IEnumerator deleteDecal(Transform _decal, AudioSource _audio)
    {
        yield return new WaitForSeconds(_audio.clip.length);
        Destroy(_decal.gameObject);
    }
}
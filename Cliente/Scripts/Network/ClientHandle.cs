using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.SClientKey();

        // Now that we have the client's id, connect UDP
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void CharactersMenu(Packet _packet)
    {

        MenuManager._instance._CharactersMenu.SetActive(true);
        MenuManager._instance._MainMenu.SetActive(false);
    }

    public static void MainMenu(Packet _packet)
    {
        string _character = _packet.ReadString();
        MenuManager._instance._characterName.text = _character;
        MenuManager._instance._CharactersMenu.SetActive(false);
        MenuManager._instance._MainMenu.SetActive(true);
    }
    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        int _weaponID = _packet.ReadInt();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation, _weaponID);
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        int _input = _packet.ReadInt();
        int _weaponID = _packet.ReadInt();

        if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
        {
            _player.transform.position = _position;
            _player.InputPressed(_input, _weaponID);
        }
    }
    public static void WeaponAttachment(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _slot = _packet.ReadInt();
        string type = _packet.ReadString();
        int _attachID = _packet.ReadInt();

        if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
        {
            _player.WeaponAttachment(type, _slot, _attachID);
        }
    }

    public static void WeaponFiremode(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _firemode = _packet.ReadString();

        if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
        {
            _player.WeaponFiremode(_firemode);
            GameManager.instance._interactMenu.SwitchFiremode(_firemode);
        }
    }

    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();
        Quaternion _spineRotation = _packet.ReadQuaternion();

        if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
        {
            _player._spineRotation = _spineRotation;
            _player.transform.rotation = _rotation;
        }
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }

    public static void PlayerHealth(Packet _packet)
    {
        int _id = _packet.ReadInt();
        float _health = _packet.ReadFloat();

        //if (!_id.Equals(Client.instance.myId))
        GameManager.players[_id].SetHealth(_health);
        //else
          //  GameManager.instance._localHealthbar.setHealth(_health);
    }

    public static void PlayerRespawned(Packet _packet)
    {
        int _id = _packet.ReadInt();

        GameManager.players[_id].Respawn();
    }

    public static void PlayerShoot(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rot = _packet.ReadQuaternion();
        bool _canShoot = _packet.ReadBool();
        bool _fullAuto = _packet.ReadBool();
        float _multiplier = _packet.ReadFloat();
        bool isInterior = _packet.ReadBool();
        float _bulletForce = _packet.ReadFloat();
        if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
        {
            if (_id != Client.instance.myId)
            {
                _player._controller._currentWeapon.setAnimValue(7);
                _player._controller._currentWeapon.weaponShoot(_canShoot, _fullAuto, _position, _rot, isInterior, _bulletForce);
            } else
            {
                _player._fpscontroller.setAnimValue(7);
                _player._fpscontroller.weaponShoot(_canShoot, _fullAuto, _position, _rot, _multiplier,isInterior, _bulletForce);
            }
        }
    }


    public static void CreateItemSpawner(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        Vector3 _spawnerPosition = _packet.ReadVector3();
        bool _hasItem = _packet.ReadBool();

        GameManager.instance.CreateItemSpawner(_spawnerId, _spawnerPosition, _hasItem);
    }

    public static void ItemSpawned(Packet _packet)
    {
        Vector3 position = _packet.ReadVector3();

        GameManager.instance.InstantiateItem(position);
    }

    public static void ItemPickedUp(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        int _byPlayer = _packet.ReadInt();

        GameManager.itemSpawners[_spawnerId].ItemPickedUp();
        GameManager.players[_byPlayer].itemCount++;
    }

    public static void SpawnProjectile(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        int _thrownByPlayer = _packet.ReadInt();

        GameManager.instance.SpawnProjectile(_projectileId, _position);
        GameManager.players[_thrownByPlayer].itemCount--;
    }

    public static void ProjectilePosition(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        if (GameManager.projectiles.TryGetValue(_projectileId, out ProjectileManager _projectile))
        {
            _projectile.transform.position = _position;
        }
    }

    public static void ProjectileExploded(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.projectiles[_projectileId].Explode(_position);
    }

    public static void SpawnEnemy(Packet _packet)
    {
        int _enemyId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        int _skinID = _packet.ReadInt();
        string _type = _packet.ReadString();
        int variant = _packet.ReadInt();
        bool hasHead = _packet.ReadBool();
        bool hasShoulderL = _packet.ReadBool();
        bool hasShoulderR = _packet.ReadBool();
        bool hasElbowL = _packet.ReadBool();
        bool hasElbowR = _packet.ReadBool();
        bool hasUpperLegL = _packet.ReadBool();
        bool hasUpperLegR = _packet.ReadBool();
        bool hasLowerLegL = _packet.ReadBool();
        bool hasLowerLegR = _packet.ReadBool();
        bool isRotten = _packet.ReadBool();
        GameManager.instance.SpawnEnemy(_enemyId, _position, _skinID, _type, variant, hasHead, hasShoulderL, hasShoulderR, hasElbowL, hasElbowR, hasUpperLegL, hasUpperLegR, hasLowerLegL, hasLowerLegR, isRotten);
    }

    public static void EnemyPosition(Packet _packet)
    {
        int _enemyId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        int _key = _packet.ReadInt();

        if (GameManager.enemies.TryGetValue(_enemyId, out ZombieAI _enemy))
        {
            _enemy.transform.position = _position;
            _enemy.transform.rotation = _rotation;
            _enemy.reciveAnimation(_key);
        }
    }

    public static void EnemyHealth(Packet _packet)
    {
        int _enemyId = _packet.ReadInt();
        float _health = _packet.ReadFloat();

        GameManager.enemies[_enemyId].SetHealth(_health);
    }

    public static void CreateDecal(Packet _packet)
    {
        string _decal = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();

        GameManager.instance.CreateDecal(_decal, _position);
    }

    public static void KillFeed(Packet _packet)
    {
        string _feed = _packet.ReadString();
        GameManager.instance._killfeed.displayFeed(_feed);
    }
    public static void EnemyDismemberment(Packet _packet)
    {
        int _enemyId = _packet.ReadInt();
        string _enemyPart = _packet.ReadString();
        if (GameManager.enemies.TryGetValue(_enemyId, out ZombieAI _enemy))
        {
            _enemy.Dismember(_enemyPart);
        }
    }

    public static void EnemyRotten(Packet _packet)
    {
        int _enemyId = _packet.ReadInt();
        if (GameManager.enemies.TryGetValue(_enemyId, out ZombieAI _enemy))
        {
            _enemy.RotFiles();
        }
    }

    public static void EnemySpecialAttack(Packet _packet)
    {
        int _enemyId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        float _time = _packet.ReadFloat();
        string type = _packet.ReadString();
        Vector3 _direction = _packet.ReadVector3();

        GameManager.enemies[_enemyId].shootProjectile(_position, _rotation, _time, type, _direction);
    }

    public static void ServerTime(Packet _packet)
    {
        float _time = _packet.ReadFloat();
        if (GameManager.instance)
             GameManager.instance.setTimeOfDay(_time);
    }

    public static void NotifyPlayer(Packet _packet)
    {
        int _type = _packet.ReadInt();
        string _message = _packet.ReadString();
        Debug.Log(_message);
        if (_type == 1)
        {
            GameManager.instance._notificationManager.showNotification(_message);
        } else
        {
            GameManager.instance._notificationManager.showKickNotification(_message);
        }
    }

    public static void SpawnHelicopter(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Vector3 _destination = _packet.ReadVector3();

        GameManager.instance.SpawnHelicopter(_id, _position, _destination);
    }
    public static void HelicopterMovment(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _destination = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager._helicopters[_id].transform.position = _destination;
        GameManager._helicopters[_id].gameObject.transform.rotation = _rotation;
    }

    public static void SpawnGuard(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        string _name = _packet.ReadString();
        string _weapon = _packet.ReadString();
        GameManager.instance.SpawnGuard(_id, _position, _weapon, _name);
    }

    public static void GuardPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        int _key = _packet.ReadInt();
        if (GameManager._guards.TryGetValue(_id, out Guard _guard))
        {
            _guard.SyncGuard(_key, _position, _rotation);
        }
    }


    public static void GuardShot(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        bool _isShooting = _packet.ReadBool();
        if (GameManager._guards.TryGetValue(_id, out Guard _guard))
        {
            _guard.ShotWeapon(_position, _rotation, _isShooting);
        }
    }
    public static void ContainerMovment(Packet _packet)
    {
       //bool _destroy = _packet.ReadBool();
       //Vector3 _destination = _packet.ReadVector3();
       //
       //Helicopter.instance._container.setContainer(_destroy, _destination);
    }

    public static void InteractMenuCall(Packet _packet)
    {
        string _message = _packet.ReadString();

        GameManager.instance._interactMenu.setDisplayTextValue(_message);
    }

    public static void ChatMessage(Packet _packet)
    {
        string _message = _packet.ReadString();
        string image = _packet.ReadString();
        GameManager.instance._chatMenu.appendText(_message, image);
    }

    public static void SafeZone(Packet _packet)
    {
        string _message = _packet.ReadString();
        int _type = _packet.ReadInt();
       GameManager.instance._notificationManager._safezone.showSafeZone(_message, _type);
    }

    public static void ZoneThreat(Packet _packet)
    {
        string _zoneText = _packet.ReadString();
        string _zoneThreat = _packet.ReadString();
        GameManager.instance._notificationManager._zone.displayZone(_zoneText, _zoneThreat);
    }

}
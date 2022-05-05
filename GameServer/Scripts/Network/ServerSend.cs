using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    /// <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to a client via UDP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via TCP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

    /// <summary>Sends a packet to all clients via UDP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via UDP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    #region Packets
    /// <summary>Sends a welcome message to the given client.</summary>
    /// <param name="_toClient">The client to send the packet to.</param>
    /// <param name="_msg">The message to send.</param>
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SendCharsMenu(int _toClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.charactersMenu))
        {
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SendMainMenu(int _toClient, string character)
    {
        using (Packet _packet = new Packet((int)ServerPackets.MainMenu))
        {
            _packet.Write(character);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>Tells a client to spawn a player.</summary>
    /// <param name="_toClient">The client that should spawn the player.</param>
    /// <param name="_player">The player to spawn.</param>
    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.name);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);
            _packet.Write(_player._currentSlot._weaponID);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>Sends a player's updated position to all clients.</summary>
    /// <param name="_player">The player whose position to update.</param>
    public static void PlayerPosition(Player _player, int _key, int _wpID)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);
            _packet.Write(_key);
            _packet.Write(_wpID);

            SendUDPDataToAll(_packet);
        }
    }

    public static void WeaponAttachment(Player _player, int slot, string type, int id)
    {
        using (Packet _packet = new Packet((int)ServerPackets.weaponAttachment))
        {
            _packet.Write(_player.id);
            _packet.Write(slot);
            _packet.Write(type);
            _packet.Write(id);

            SendUDPDataToAll(_packet);
        }
    }

    public static void WeaponFiremode(Player _player, string _firemode)
    {
        using (Packet _packet = new Packet((int)ServerPackets.switchFiremode))
        {
            _packet.Write(_player.id);
            _packet.Write(_firemode);

            SendUDPDataToAll(_packet);
        }
    }

    /// <summary>Sends a player's updated rotation to all clients except to himself (to avoid overwriting the local player's rotation).</summary>
    /// <param name="_player">The player whose rotation to update.</param>
    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);
            _packet.Write(_player._spine.localRotation);

            SendUDPDataToAll(_player.id, _packet);
        }
    }

    public static void PlayerDisconnected(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerHealth(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerHealth))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.health);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerRespawned(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRespawned))
        {
            _packet.Write(_player.id);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerShoot(Player _player, Vector3 _position, Quaternion _rot, bool _canShoot, bool _fullAuto, float _multiplier, float _bulletForce)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerShoot))
        {
            _packet.Write(_player.id);
            _packet.Write(_position);
            _packet.Write(_rot);
            _packet.Write(_canShoot);
            _packet.Write(_fullAuto);
            _packet.Write(_multiplier);
            _packet.Write(_player.isInInterior);
            _packet.Write(_bulletForce);

            SendUDPDataToAll(_packet);
        }
    }



    public static void CreateItemSpawner(int _toClient, int _spawnerId, Vector3 _spawnerPosition, bool _hasItem)
    {
        using (Packet _packet = new Packet((int)ServerPackets.createItemSpawner))
        {
            _packet.Write(_spawnerId);
            _packet.Write(_spawnerPosition);
            _packet.Write(_hasItem);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void ItemSpawned(Vector3 _position)
    {
        using (Packet _packet = new Packet((int)ServerPackets.itemSpawned))
        {
            _packet.Write(_position);

            SendTCPDataToAll(_packet);
        }
    }

    public static void ItemPickedUp(int _spawnerId, int _byPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.itemPickedUp))
        {
            _packet.Write(_spawnerId);
            _packet.Write(_byPlayer);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnProjectile(Projectile _projectile, int _thrownByPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnProjectile))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);
            _packet.Write(_thrownByPlayer);

            SendTCPDataToAll(_packet);
        }
    }

    public static void ProjectilePosition(Projectile _projectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.projectilePosition))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);

            SendUDPDataToAll(_packet);
        }
    }

    public static void ProjectileExploded(Projectile _projectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.projectileExploded))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnEnemy(ZombieInstance _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            SendTCPDataToAll(SpawnEnemy_Data(_enemy, _packet));
        }
    }
    public static void SpawnEnemy(int _toClient, ZombieInstance _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            SendTCPData(_toClient, SpawnEnemy_Data(_enemy, _packet));
        }
    }

    private static Packet SpawnEnemy_Data(ZombieInstance _enemy, Packet _packet)
    {
        _packet.Write(_enemy.id);
        _packet.Write(_enemy.transform.position);
        _packet.Write(_enemy._zombieSkin);
        _packet.Write(_enemy._zombieType.ToString());
        _packet.Write(_enemy._animatorVariant);
        _packet.Write(_enemy.hasHead);
        _packet.Write(_enemy.hasShoulderL);
        _packet.Write(_enemy.hasShoulderR);
        _packet.Write(_enemy.hasElbowL);
        _packet.Write(_enemy.hasElbowR);
        _packet.Write(_enemy.hasUpperLegL);
        _packet.Write(_enemy.hasUpperLegR);
        _packet.Write(_enemy.hasLowerLegL);
        _packet.Write(_enemy.hasLowerLegR);
        _packet.Write(_enemy.isRotten);
        return _packet;
    }

    public static void SpawnGuard(Guard _guard)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnGuard))
        {
            SendTCPDataToAll(SpawnGuard_Data(_guard, _packet));
        }
    }

    public static void SpawnGuard(int _toClient, Guard _guard)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnGuard))
        {
            SendTCPData(_toClient, SpawnGuard_Data(_guard, _packet));
        }
    }


    private static Packet SpawnGuard_Data(Guard _guard, Packet _packet)
    {
        _packet.Write(_guard._id);
        _packet.Write(_guard.transform.position);
        _packet.Write(_guard.name);
        _packet.Write(_guard._weapon.ToString());
        return _packet;
    }

    public static void GuardPosition(Guard _guard)
    {
        using (Packet _packet = new Packet((int)ServerPackets.guardPosition))
        {
            _packet.Write(_guard._id);
            _packet.Write(_guard.transform.position);
            _packet.Write(_guard.transform.rotation);
            _packet.Write(_guard._key);

            SendUDPDataToAll(_packet);
        }
    }

    public static void GuardShot(Guard _guard, bool isShooting)
    {
        using (Packet _packet = new Packet((int)ServerPackets.guardFire))
        {
            _packet.Write(_guard._id);
            _packet.Write(_guard._viewPoint.position);
            _packet.Write(_guard._viewPoint.rotation);
            _packet.Write(isShooting);

            SendUDPDataToAll(_packet);
        }
    }

    public static void EnemyPosition(ZombieInstance _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyPosition))
        {
            _packet.Write(_enemy.id);
            _packet.Write(_enemy.transform.position);
            _packet.Write(_enemy.transform.rotation);
            _packet.Write(_enemy._key);

            SendUDPDataToAll(_packet);
        }
    }

    public static void EnemyHealth(ZombieInstance _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyHealth))
        {
            _packet.Write(_enemy.id);
            _packet.Write(_enemy.hp);

            SendTCPDataToAll(_packet);
        }
    }
    public static void EnemyDismemberment(ZombieInstance _enemy, string part)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyDismemberment))
        {
            _packet.Write(_enemy.id);
            _packet.Write(part);

            SendTCPDataToAll(_packet);
        }
    }

    public static void EnemyRotten(ZombieInstance _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyRotten))
        {
            _packet.Write(_enemy.id);

            SendTCPDataToAll(_packet);
        }
    }

    public static void EnemySpecialAttack(ZombieInstance _enemy, Vector3 _position , Quaternion _rotation, float time, string type, Vector3 direction)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemeySpecialAttack))
        {
            _packet.Write(_enemy.id);
            _packet.Write(_position);
            _packet.Write(_rotation);
            _packet.Write(time);
            _packet.Write(type);
            _packet.Write(direction);

            SendUDPDataToAll(_packet);
        }
    }

    public static void CreateDecal(string _decal, Vector3 _position)
    {
        using (Packet _packet = new Packet((int)ServerPackets.bulletDecals))
        {
            _packet.Write(_decal);
            _packet.Write(_position);

            SendUDPDataToAll(_packet);
        }
    }

    public static void SendKillFeed(string _feed)
    {
        using (Packet _packet = new Packet((int)ServerPackets.feedPacket))
        {
            _packet.Write(_feed);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SendServerTime(float _time)
    {
        using (Packet _packet = new Packet((int)ServerPackets.serverTime))
        {
            _packet.Write(_time);

            SendUDPDataToAll(_packet);
        }
    }

    public static void NotifyPlayer(int id, int type, string _message)
    {
        using (Packet _packet = new Packet((int)ServerPackets.notificationManager))
        {
            _packet.Write(type);
            _packet.Write(_message);

            SendTCPData(id, _packet);
        }
    }

    public static Packet _HelicopterPacket(BossHelicopter _helicopter, Packet _packet)
    {
        _packet.Write(_helicopter.id);
        _packet.Write(_helicopter.transform.position);
        _packet.Write(_helicopter._destination);
        return _packet;
    }
    public static void SpawnHelicopter(BossHelicopter _helicopter)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnHelicopter))
        {
            SendTCPDataToAll(_HelicopterPacket(_helicopter, _packet));
        }
    }

    public static void SpawnHelicopter(int _toClient, BossHelicopter _helicopter)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnHelicopter))
        {
            SendTCPData(_toClient ,_HelicopterPacket(_helicopter, _packet));
        }
    }
    public static void HelicopterEvent(int id, Vector3 _helicopter, Quaternion _rotation)
    {
        using (Packet _packet = new Packet((int)ServerPackets.helicopterEvent))
        {
            _packet.Write(id);
            _packet.Write(_helicopter);
            _packet.Write(_rotation);

            SendTCPDataToAll(_packet);
        }
    }

    public static void ContainerDrop(bool _destroy, Vector3 position)
    {
        using (Packet _packet = new Packet((int)ServerPackets.containerDrop))
        {
            _packet.Write(_destroy);
            _packet.Write(position);

            SendTCPDataToAll(_packet);
        }
    }

    public static void InteractMenu(string _text, int id)
    {
        using (Packet _packet = new Packet((int)ServerPackets.interactMenu))
        {
            _packet.Write(_text);

           SendTCPData(id, _packet);
        }
    }

    public static void ChatMessage(string _text, string image)
    {
        using (Packet _packet = new Packet((int)ServerPackets.chatMessage))
        {
            _packet.Write(_text);
            _packet.Write(image);
            SendTCPDataToAll(_packet);
        }
    }

    public static void SafeZone(int _toClient, string _message, int status)
    {
        using (Packet _packet = new Packet((int)ServerPackets.SafeZone))
        {
            _packet.Write(_message);
            _packet.Write(status);
            SendTCPData(_toClient, _packet);
        }
    }

    public static void ZoneThreat(int _toClient, ZoneThreading _zone)
    {
        using (Packet _packet = new Packet((int)ServerPackets.zoneEnter))
        {
            _packet.Write(_zone._zoneName);
            _packet.Write(_zone._ZoneThreading.ToString());
            SendTCPData(_toClient, _packet);
        }
    }

    #endregion
}
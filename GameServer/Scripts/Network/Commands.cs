using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commands : MonoBehaviour
{
    // Start is called before the first frame update

    private enum _commands
    {
        tp,
        feed,
        move,
        kick,
        ban,
        spawnZombieAt,
        spawnZombie,
        spawnHeli,
        setTag,
        setColor
    }

    private _commands commands;

    private static int searchPlayer(string player)
    {
        int id = -1;
        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                if (_client.player.name.Equals(player))
                {
                    id = _client.player.id;
                    return id;
                }
            }
        }
        return id;
    }
    public static bool isCommand(string text)
    {
        bool command = false;
        foreach (string _command in Enum.GetNames(typeof(_commands)))
        {
            if (_command.Equals(text))
                return true;
        }
        return command;
    }

    public static void TPPlayer(string player, string toPlayer)
    {
        int id = -1, toid = -1;
        id = searchPlayer(player);
        toid = searchPlayer(toPlayer);
        if (id != -1 && toid != -1)
        {
            if (id != toid)
            {
                Server.clients[id].player.transform.position = Server.clients[toid].player.transform.position;
                Debug.Log($"Player {Server.clients[id].player.name} moved to: {Server.clients[toid].player.name}");
            }
        }
    }

    public static void Feed(string message)
    {
        ServerSend.SendKillFeed(message);
    }

    public static void MovePlayer(string player, float x, float y, float z)
    {
        int id = -1;
        id = searchPlayer(player);
        if (id != -1)
        {
            Server.clients[id].player.transform.position = new Vector3(x, y, z);
            Debug.Log($"Player {Server.clients[id].player.name} moved to: {x}, {y}, {z}");
        } else
        {
            Debug.Log($"Player {player} does not exits");
        }
    }

    public static void KickPlayer(string player, string reason = "")
    {
        int id = -1;
        id = searchPlayer(player);
        if (id != -1)
        {
            ServerSend.NotifyPlayer(id, 2, $"Has sido expulsado: {reason}");
            Server.clients[id].KickPlayer(reason);
            Debug.Log($"Player {Server.clients[id].player.name} has been kicked: {reason}");
        } else
        {
            Debug.Log($"Player {player} does not exits");
        }
    }

    public static void SpawnZombieAt(string player, int zombieID)
    {
        int id = -1;
        id = searchPlayer(player);
        if (id != -1)
        {
            Debug.Log($"Spawned zombie id {zombieID} at {player}");
            NetworkManager.instance.InstantiateEnemy(Server.clients[id].player.transform.position, zombieID);
        }
        else
        {
            Debug.Log($"Player {player} does not exits");
        }
    }

    public static void SpawnZombie(float x, float y, float z, int zombieID)
    {
        NetworkManager.instance.InstantiateEnemy(new Vector3(x, y, z), zombieID);
    }

    public static void SpawnHeli(float x, float y, float z, string playerDest, float speed)
    {
        int id = -1;
        id = searchPlayer(playerDest);
        if (id != -1)
        {
            Debug.Log("BOSS Helicopter spawned");
            BossHelicopter _heli = NetworkManager.instance.InstantiateHeli(new Vector3(x, y, z)).GetComponent<BossHelicopter>();
            _heli._destination = new Vector3(Server.clients[id].player.transform.position.x, Server.clients[id].player.transform.position.y + 36f, Server.clients[id].player.transform.position.z);
            _heli._speed = speed;
        } else
        {
            Debug.Log($"Player {playerDest} does not exits");
        }

    }

    public static void SetTag(string player, string tag)
    {
        int id = -1;
        id = searchPlayer(player);
        if (id != -1)
        {
            Server.clients[id].player._playerTag = tag;
            Debug.Log($"Player {Server.clients[id].player.name} tag changed to: {tag}");
        } else
        {
            Debug.Log($"Player {player} does not exits");
        }
    }

    public static void SetColor(string player, string color)
    {
        int id = -1;
        id = searchPlayer(player);
        if (id != 1)
        {
            Server.clients[id].player._chatColor = color;
            Debug.Log($"Player {Server.clients[id].player.name} tag color changed to: {color}");
        } else
        {
            Debug.Log($"Player {player} does not exits");
        }

    }
}

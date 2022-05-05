using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    /// <summary>Sends a packet to the server via TCP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to the server via UDP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    /// <summary>Lets the server know that the welcome message was received.</summary>
    public static void SClientKey()
    {
        using (Packet _packet = new Packet((int)ClientPackets.ClientKey))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(Client._clientKey);

            SendTCPData(_packet);
        }
    }

    public static void CreateCharacter(string characterName)
    {
        using (Packet _packet = new Packet((int)ClientPackets.createCharacter))
        {
            _packet.Write(characterName);

            SendTCPData(_packet);
        }
    }

    public static void SendIntoGame()
    {
        using (Packet _packet = new Packet((int)ClientPackets.joinGame))
        {
            _packet.Write("Gv3kOM)zBU / 0WYZ");
            SendTCPData(_packet);
        }
    }

    public static void JoinServer()
    {
        using (Packet _packet = new Packet((int)ClientPackets.joinServer))
        {
            _packet.Write(ClientKey.getClientKey());
            SendTCPData(_packet);
        }
        Client.instance.Disconnect();
    }

    /// <summary>Sends player input to the server.</summary>
    /// <param name="_inputs"></param>
    public static void PlayerMovement(bool[] _inputs, Quaternion _spineRot)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }
            _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);
            _packet.Write(_spineRot);
            SendUDPData(_packet);
        }
    }

    public static void PlayerShoot(Vector3 _facing, Quaternion _rot, bool _still)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerShoot))
        {
            _packet.Write(_facing);
            _packet.Write(_rot);
            _packet.Write(_still);

            SendTCPData(_packet);
        }
    }

    public static void PlayerThrowItem(Vector3 _facing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerThrowItem))
        {
            _packet.Write(_facing);

            SendTCPData(_packet);
        }
    }

    public static void ChatMessage(string text)
    {
        using (Packet _packet = new Packet((int)ClientPackets.chatMessage))
        {
            _packet.Write(text);

            SendTCPData(_packet);
        }
    }
    #endregion
}
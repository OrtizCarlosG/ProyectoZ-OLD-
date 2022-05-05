using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _clientKey = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_clientKey}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }

        Debug.Log($"Recived client key from {Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} KEY: {_clientKey}");
        SQLManager._instance.openDB();
        if (Logins.CheckCode(_clientKey))
        {
            Debug.Log($"Client key {Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} accepted.");
            Server.clients[_fromClient].ClientKey = _clientKey;
            string _account = Logins.getAccountFromCode(_clientKey);
            string _status = Logins.getAccountStatus(_account);
            if (!_status.Equals("IN GAME") || string.IsNullOrEmpty(_status))
            {
                Debug.Log("Character is not in game");
                Logins.updateAccountStatus(_account, "LOBBY");
                if (!Logins.CheckCharacter(_account))
                {
                    string _character = Logins.getCharacterName(_account);
                    ServerSend.SendMainMenu(_fromClient, _character);
                }
                else
                {
                    ServerSend.SendCharsMenu(_fromClient);
                }
            }
            else
            {
                string account = Logins.getAccountFromCode(_clientKey);
                string _character = Logins.getCharacterName(account);
                Server.clients[_fromClient].SendIntoGame(_character);
                Server.clients[_fromClient].profileImage = Logins.getAccountProfile(account);
                Debug.Log($"Client {Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} joined the game");
            }
        } else
        {
            Server.clients[_fromClient].KickPlayer("Codigo incorrecto");
        }
        SQLManager._instance.closeDB();
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
        Quaternion _rotation = _packet.ReadQuaternion();
        Quaternion _spineRot = _packet.ReadQuaternion();
        Server.clients[_fromClient].player.SetInput(_inputs, _rotation, _spineRot);
    }

    public static void PlayerShoot(int _fromClient, Packet _packet)
    {
        Vector3 _shootDirection = _packet.ReadVector3();
        Quaternion _shootRotation = _packet.ReadQuaternion();
        bool _still = _packet.ReadBool();

        Server.clients[_fromClient].player.Shoot(_shootDirection, _shootRotation, _still);
    }

    public static void PlayerThrowItem(int _fromClient, Packet _packet)
    {
        Vector3 _throwDirection = _packet.ReadVector3();

        Server.clients[_fromClient].player.ThrowItem(_throwDirection);
    }

    public static void ChatMessage(int _fromClient, Packet _packet)
    {
        string _message = _packet.ReadString();
        if (_message.Equals("/startAttack"))
            EventManager.instance.startOrde();
        if (string.IsNullOrEmpty(Server.clients[_fromClient].player._playerTag))
        {
            ServerSend.ChatMessage($" <color={Server.clients[_fromClient].player._chatColor}>{Server.clients[_fromClient].player.name}</color>: {_message}", Server.clients[_fromClient].profileImage);
        } else
        {
            ServerSend.ChatMessage($"<color={Server.clients[_fromClient].player._chatColor}>[{Server.clients[_fromClient].player._playerTag}] {Server.clients[_fromClient].player.name}</color>: {_message}", Server.clients[_fromClient].profileImage);
        }
        Debug.Log($"{Server.clients[_fromClient].player.name}: {_message}");
    }

    public static void CreateCharacter(int _fromClient, Packet _packet)
    {
        string _charName = _packet.ReadString();
        SQLManager._instance.openDB();
        string _account = Logins.getAccountFromCode(Server.clients[_fromClient].ClientKey);
        Debug.Log($"Account: {_account} tryed to create a character");
        if (Logins.CheckCharacter(_account))
        {
            if (!Logins.doesCharacterExists(_charName))
            {
                Debug.Log($"Account: {_account} cant not create character (already exists)");
                Logins.UpadteCharacterName(_account, _charName);
                ServerSend.SendMainMenu(_fromClient, _charName);
            }
        }else
        {
            Debug.Log($"Account: {_account} does not need another character");
        }
        SQLManager._instance.closeDB();
    }

    public static void JoinGame(int _fromClient, Packet _packet)
    {
        //string _clientKey = _packet.ReadString();
        //SQLManager._instance.openDB();
        //string account = Logins.getAccountFromCode(_clientKey);
        //string _character = Logins.getCharacterName(account);
        ////Server.clients[_fromClient].SendIntoGame(_character);
        //SQLManager._instance.closeDB();
    }

    public static void JoinServer(int _fromClient, Packet _packet)
    {
        string _clientKey = _packet.ReadString();
        SQLManager._instance.openDB();
        string account = Logins.getAccountFromCode(_clientKey);
        string _character = Logins.getCharacterName(account);
        Logins.updateAccountStatus(account, "IN GAME");
        //Server.clients[_fromClient].Disconnect();
        SQLManager._instance.closeDB();
    }
}
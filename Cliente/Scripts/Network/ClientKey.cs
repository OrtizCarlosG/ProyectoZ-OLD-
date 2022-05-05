using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientKey
{
    private static string _clientKey;

    public static void setClientKey(string _key)
    {
        _clientKey = _key;
    }

    public static string getClientKey()
    {
        return _clientKey;
    }
}

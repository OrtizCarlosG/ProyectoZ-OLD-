using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LaunchParams : MonoBehaviour
{
    //Obtiene el argumento del exe
    private string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        foreach (string arg in args)
            if (arg.Split('=')[0].Equals(name))
                return arg.Split('=')[1];
        return "";
    }

    void Start()
    {
        Client.instance.ConnectToServer();
        Client._clientKey = GetArg("-ClientKey");
        ClientKey.setClientKey(GetArg("-ClientKey"));
    }
}

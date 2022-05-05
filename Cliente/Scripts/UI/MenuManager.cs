using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour
{
    public static MenuManager _instance;

    public GameObject _CharactersMenu;
    public GameObject _MainMenu;
    public Text _characterName;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void StartGame()
    {
        ClientSend.JoinServer();
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
    }
}

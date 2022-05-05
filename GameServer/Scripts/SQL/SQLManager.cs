using UnityEngine;
using System.Data;
using System;
using System.Data.SqlClient;
public class SQLManager : MonoBehaviour
{
    public string _Server;
    public int _Port;
    public string _User;
    public string _Password;
    public string _Database;

    [HideInInspector]private SqlConnection _connection;

    public static SQLManager _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        ConnectToServer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ConnectToServer()
    {
       string connetionString = "Data Source=127.0.0.1,1433;Initial Catalog=ProyectoZ;User Id=sa;Password=1234;MultipleActiveResultSets=true";
        _connection = new System.Data.SqlClient.SqlConnection(connetionString);
       try
       {
            _connection.Open();
            Debug.Log("Conectado a la base de datos");
            _connection.Close();
       }
       catch (Exception ex)
       {
           Debug.LogError("No se ha poodido conectar " + ex);
       }

    }

    public void openDB()
    {
        if (_connection != null && _connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    public void closeDB()
    {
        _connection.Close();
    }
    public SqlConnection getConnection()
    {
        return _connection;
    }
}

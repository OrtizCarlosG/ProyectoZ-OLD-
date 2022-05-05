using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data.SqlClient;
using System.Data;

public class Logins : MonoBehaviour
{
    // Start is called before the first frame update
    public static SQLManager connection = SQLManager._instance;
    public static void UpadteCharacterName(string account, string charName)
    {
        string commandText = "UPDATE Accounts SET CharName = @character WHERE Account = @account";
        SqlCommand command = new SqlCommand(commandText, connection.getConnection());
        command.Parameters.Add("@character", SqlDbType.VarChar, 11).Value = charName;
        command.Parameters.Add("@account", SqlDbType.VarChar, 10).Value = account;
        command.ExecuteNonQuery();
    }

    public static void updateAccountStatus(string account, string status)
    {
        string commandText = "UPDATE AccountStatus SET Activity = @status WHERE Account = @account";
        SqlCommand command = new SqlCommand(commandText, connection.getConnection());
        command.Parameters.Add("@status", SqlDbType.VarChar, 11).Value = status;
        command.Parameters.Add("@account", SqlDbType.VarChar, 10).Value = account;
        command.ExecuteNonQuery();
    }
    public static bool CheckCharacter(string account)
    {
        bool hasChar = false;
        string commandText = "SELECT * FROM Accounts WHERE Account = @account AND CharName = ''";
        SqlCommand command = new SqlCommand(commandText, connection.getConnection());
        command.Parameters.Add("@account", SqlDbType.VarChar, 10).Value = account;
        var value = command.ExecuteScalar();
        if (value != null)
            hasChar = true;
        return hasChar;
    }

    public static bool CheckCode(string code)
    {
        bool hasChar = false;
        string commandText = "SELECT * FROM AccountStatus WHERE Code = @code";
        SqlCommand command = new SqlCommand(commandText, connection.getConnection());
        command.Parameters.Add("@code", SqlDbType.VarChar, 15).Value = code;
        var value = command.ExecuteScalar();
        if (value != null)
            hasChar = true;
        return hasChar;
    }

    public static bool doesCharacterExists(string character)
    {
        bool hasChar = false;
        string commandText = "SELECT * FROM Accounts WHERE CharName = @char";
        SqlCommand command = new SqlCommand(commandText, connection.getConnection());
        command.Parameters.Add("@char", SqlDbType.VarChar, 11).Value = character;
        var value = command.ExecuteScalar();
        if (value != null)
            hasChar = true;
        return hasChar;
    }

    public static string getAccountFromCode(string code)
    {
        string account = "";
        string commandText = "SELECT * FROM AccountStatus WHERE Code = @code";
        SqlCommand command = new SqlCommand(commandText, connection.getConnection());
        command.Parameters.Add("@code", SqlDbType.VarChar, 15).Value = code;
        SqlDataReader reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                account = reader.GetString(1);
            }
        }
        return account;
    }

    public static string getCharacterName(string accountName)
    {
        string account = "";
        string commandText = "SELECT * FROM Accounts WHERE Account = @account";
        SqlCommand command = new SqlCommand(commandText, connection.getConnection());
        command.Parameters.Add("@account", SqlDbType.VarChar, 10).Value = accountName;
        SqlDataReader reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                account = reader.GetString(4);
            }
        }
        return account;

    }

    public static string getAccountStatus(string account)
    {
        string status = "";
        string commandText = "SELECT * FROM AccountStatus WHERE Account = @acc";
        SqlCommand command = new SqlCommand(commandText, connection.getConnection());
        command.Parameters.Add("@acc", SqlDbType.VarChar, 10).Value = account;
        SqlDataReader reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                status = reader.GetString(7);
            }
        }
        return status;
    }

    public static string getAccountProfile(string account)
    {
        string image = "";
        SqlCommand sqlCmd = new SqlCommand();
        sqlCmd.Connection = connection.getConnection();
        sqlCmd.CommandType = CommandType.Text;
        sqlCmd.CommandText = "SELECT * FROM Accounts WHERE Account = @usuario";
        sqlCmd.Parameters.Add("@usuario", SqlDbType.VarChar, 10).Value = account;
        var value = sqlCmd.ExecuteScalar();
        if (value != null)
        {
            SqlDataReader reader = sqlCmd.ExecuteReader();
            while (reader.Read())
            {
                image = reader.GetString(10);
            }
        }
        return image;
    }

}

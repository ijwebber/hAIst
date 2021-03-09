using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class DB_Controller : MonoBehaviour
{
    public void Start()
    {
        Debug.Log("TEST");
        string connectionString =
          "Server=sql4.freemysqlhosting.net;" +
          "Database=sql4396407;" +
          "User ID=sql4396407;" +
          "Password=X8eR8int2f;" +
          "Pooling=false";
        IDbConnection dbcon;
        dbcon = new MySqlConnection(connectionString);
        dbcon.Open();
        IDbCommand dbcmd = dbcon.CreateCommand();
        // requires a table to be created named employee
        // with columns firstname and lastname
        // such as,
        //        CREATE TABLE employee (
        //           firstname varchar(32),
        //           lastname varchar(32));
        string sql =
            "SELECT * " +
            "FROM users";
        dbcmd.CommandText = sql;
        IDataReader reader = dbcmd.ExecuteReader();
        while(reader.Read()) {
                string username = (string) reader["user_id"];
                int balance = (int) reader["coin_balance"];
                Debug.Log("USERID: " +
                    username);
        }
        // clean up
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;
    }
}
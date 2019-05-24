using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using Mono.Data.Sqlite;

public class DBScript : MonoBehaviour {

    public Text dbpath;
    public Text consoleMSG;

    private SqliteConnection con;

    #region Connection
    public void Connect()
    {
        Connection(dbpath.text);
    }

    private void Connection(string database_path)
    {
        try
        {
            if (File.Exists(database_path))
            {
                if (database_path.EndsWith(".sqlite"))
                {
                    con = new SqliteConnection("Data Source = " + database_path + "; " + " Version = 3;");
                    if (con != null)
                    {
                        con.Open();
                        consoleMSG.text = "Connected to the database. \n" +
                                           database_path;
                    }
                }
                else
                {
                    consoleMSG.text = "Database doesn't exists. \n" +
                                      "Please use the full path to database-file.";
                }
            }
            else
            {
                consoleMSG.text = "Please use a SQLite Database";
            }
        }
        catch (Exception e)
        {
            Debug.Log("Failed! \n" + e);
            consoleMSG.text = "Failed to connect.";
        }
    }
    #endregion

    #region Select Tables
    public void SelectTables()
    {
        Selecting(con);
    }
    private void Selecting(SqliteConnection conn)
    {
        try
        {
            Debug.Log("Uff");
            //DOTO: Solve Problem Selecting -> TODO.txt
        }
        catch (Exception e)
        {
            Debug.Log("Failed! \n" + e);
            consoleMSG.text = "Failed to read tables.";
        }
    }
    #endregion

    #region Exit
    public void CloseConnection()
    {
        Exit();
    }
    private void Exit()
    {
        try
        {
            if (con != null)
            {
                con.Close();
                consoleMSG.text = "Closed connection.";
            }
        }
        catch (Exception e)
        {
            consoleMSG.text = "No connection was established.";
            Debug.Log("Failed. \n" + e);
        }
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using Mono.Data.Sqlite;

public class DBScript : MonoBehaviour
{
    #region Vars
    public GameObject container;
    public InputField dbpath;
    public InputField num_of_rowsField;
    public InputField table_rows_original;
    public Text consoleMSG;

    private int rows = 0;
    private GameObject rowscontainer;
    private InputField[] rowarray;
    private SqliteConnection con;
    private bool isEntered = false;
    #endregion

    #region Connection
    public void Connect()
    {        
        Connection(@"" + dbpath.text);
    }

    private void Connection(string database_path)
    {
        try
        {
            if (File.Exists(database_path))
            {
                if (num_of_rowsField.text != null)
                {
                    CreateTableRows();
                }

                for (int i = 0; i < database_path.Length; i++)
                {
                    database_path.Replace(@"\", "/");
                }

                if (database_path.EndsWith(".sqlite") && !(rows <=1))
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
                                      "You must have atleast more than 1 'Table Rows'";
                }
            }
            else
            {
                consoleMSG.text = "Please use the full path to database-file.";
            }
        }
        catch (Exception e)
        {
            Debug.Log("Failed! \n" + e);
            consoleMSG.text = "Failed to connect.";
        }
    }
    #endregion

    #region CreateTableRows
    private void CreateTableRows()
    {
        if (!isEntered)
        {        
            rowscontainer = Instantiate(container);
            rowscontainer.name = "RowsContainer";
            rowscontainer.transform.SetParent(container.transform);

            Vector3 table_pos = num_of_rowsField.transform.position;
            rows = int.Parse(num_of_rowsField.text);
            rowarray = new InputField[rows];
            for (int i = 0; i < rows; i++)
            {
                Quaternion instantiateInWorldSpace = default(Quaternion);
                InputField tablefields = Instantiate(table_rows_original, new Vector3(table_pos.x,table_pos.y-((i+1)*35), table_pos.z), instantiateInWorldSpace);
                tablefields.transform.SetParent(rowscontainer.transform);
                tablefields.GetComponent<InputField>().contentType = InputField.ContentType.Alphanumeric;
                tablefields.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "table name";

                tablefields.text = "";
                tablefields.name = "Input_Tabelle_Number_" + (i + 1);
                rowarray[i] = tablefields;
            }
            isEntered = true;
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
                Destroy(rowscontainer);
                isEntered = false;
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
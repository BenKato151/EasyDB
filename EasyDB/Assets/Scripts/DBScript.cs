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
    public InputField deleteID;
    public GameObject deleteContainer;
    public Text consoleMSG;
    public Text userOutputMSG;

    private int rows = 0;
    private GameObject rowscontainer;
    private InputField[] rowarray;
    private SqliteConnection con;
    private bool isEntered = false;
    private bool isReady = false;
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
                for (int i = 0; i < database_path.Length; i++)
                {
                    database_path.Replace(@"\", "/");
                }

                if (database_path.EndsWith(".sqlite") && !(rows <= 0))
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
            consoleMSG.text = "Failed to connect.";
            Debug.LogError("Failed! \n" + e);
        }
    }
    #endregion

    #region CreateTableRows
    public void CreateTableFields()
    {
        if (num_of_rowsField.text != null)
        {
            CreateTableRows();
        }
    }
    private void CreateTableRows()
    {
        try
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
                    tablefields.GetComponent<InputField>().contentType = InputField.ContentType.Custom;
                    tablefields.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "table name";

                    tablefields.text = "";
                    tablefields.name = "Input_Tabelle_Number_" + (i + 1);
                    rowarray[i] = tablefields;
                }
                isEntered = true;
                consoleMSG.text = "";
            }
        }
        catch (Exception e)
        {
            consoleMSG.text = "You must have atleast one table!";
            Debug.LogError("Failed!\n " + e);
        }
    }
    #endregion

    #region Select Tables
    public void SelectTables()
    {
        Selecting(con);
    }
    private void Selecting(SqliteConnection Scon)
    {
        try
        {
            for (int b = 0; b < rowarray.Length; b++)
            {                
                string selecting = "SELECT * FROM " + rowarray[b].text.ToString() + " ;";
                SqliteCommand command = new SqliteCommand(selecting, Scon);
                SqliteDataReader output = command.ExecuteReader();
                
                while (output.Read())
                {
                    for (int i = 0; i < output.FieldCount; i++)
                    {
                        string column_value = output[i].ToString();
                        string table = rowarray[b].text.ToString();
                        string column = output.GetName(i).ToString();
                        string outputtext = "In der Tabelle " + table + 
                                  " ist der Wert der Column " + column + " : " + column_value;

                        userOutputMSG.text = userOutputMSG.text + outputtext + "\n \n";
                    }
                }                
            }
            consoleMSG.text = "Success! ";
        }
        catch (Exception e)
        {
            consoleMSG.text = "Failed to read tables.";
            Debug.LogError("Failed! \n" + e);
        }
    }
    #endregion

    //TODO: Add everything that is explained in comments. Look Up: TODO.txt

    #region Delete Values

    //its not working very well
    public void DeleteValuesInnerDB()
    {
        Delete(con);
    }

    public void SetUp()
    {
        Isset(isReady);
    }

    private void Delete(SqliteConnection Scon)
    {
        if (Isset(isReady))
        {
            try
            {
                for (int i = 0; i < rowarray.Length; i++)
                {
                    string deletion = "DELETE FROM " + rowarray[i].text.ToString() + " WHERE ID = @id;";
                    SqliteCommand command = new SqliteCommand(deletion, Scon);
                    command.Parameters.Add("@id", System.Data.DbType.String).Value = deleteID.text.ToString();

                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                    consoleMSG.text = "Successfully deleted values with ID: " + deleteID.text;
                }
            }
            catch (Exception e)
            {
                consoleMSG.text = "Failed to delete Values";
                Debug.LogError("Failed!\n" + e);
            }
        }
    }

    private bool Isset(bool isit)
    {
        isit = true;
        deleteContainer.SetActive(true);
        consoleMSG.text = "ready";
        return isit;
    }
    
    #endregion

    #region Update Values
    public void UpdateValuesInnerDB()
    {
        UpdateValues(con);
    }

    private void UpdateValues(SqliteConnection Scon)
    {
        try
        {
            string updatecommand = " UPDATE *** " + //Textfield for table
                                       " SET " + /* TextField for column */ " = @wert " +
                                       " WHERE ID = @IDvalue;";

            SqliteCommand Command = new SqliteCommand(updatecommand, Scon);
            Command.Parameters.Add("@wert", System.Data.DbType.Int32).Value = 0; //Textfield for new value
            Command.Parameters.Add("@IDvalue", System.Data.DbType.Int32).Value = 0; //Textfield for new value

            Command.ExecuteNonQuery();
            Command.Parameters.Clear();
            consoleMSG.text = "Updated value in \n" +
                               "\nsuccessfuly!";
        }
        catch (Exception e)
        {
            consoleMSG.text = "Failed to update values";
            Debug.LogError("Failed!\n" + e);
        }
    }
    #endregion

    #region Insert Values
    public void InsertValuesInnerDB()
    {
        InsertValues(con);
    }

    private void InsertValues(SqliteConnection Scon)
    {
        try
        {
            string insertIntoAttributskosten = " INSERT INTO ***(****) " +
                                          " VALUES(****);"; //Fields for table/columns and values + parameters


            SqliteCommand Command = new SqliteCommand(insertIntoAttributskosten, Scon);
            Command.Parameters.Add("@***", System.Data.DbType.Int32).Value = 0; //Fields for values
            Command.Parameters.Add("@***", System.Data.DbType.Int32).Value = 0;
            Command.Parameters.Add("@***", System.Data.DbType.Int32).Value = 0;

            Command.ExecuteNonQuery();
            Command.Parameters.Clear();
            consoleMSG.text = "Inserted values nsuccessfuly!";
        }
        catch (Exception e)
        {
            consoleMSG.text = "Failed to insert values!";
            Debug.LogError("Failed!\n" + e);
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
                userOutputMSG.text = "";
                dbpath.text = "";
                num_of_rowsField.text = "";
                Destroy(rowscontainer);
                isEntered = false;
            }
        }
        catch (Exception e)
        {
            consoleMSG.text = "No connection was established.";
            Debug.LogError("Failed. \n" + e);
        }
    }
    #endregion

}
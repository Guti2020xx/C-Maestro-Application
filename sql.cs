using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Net.NetworkInformation;

namespace Database_Control
{
    public class SQL
    {
        private SqlConnection com;
        private Dictionary<string, SqlDependency> Change = new Dictionary<string, SqlDependency>();
        private Dictionary<string, bool> Ignore = new Dictionary<string, bool>();

        //Closes DB connection
        public void Close()
        {
            SqlDependency.Stop(com.ConnectionString);
            com.Close();
        }

        //Method to connect to Server
        public bool Connect(string Database, string Username, string Password, out Exception? Ex)
        {
            if (com != null)
            {
                Close();
            }
            try
            {
                //Establish new SQL connection with provided parameters(server, user ID, Password)
                string ConnectionString = @"server=" + Database + ";User ID=" + Username + ";Password=" + Password + ";TrustServerCertificate=True;MultipleActiveResultSets=true";
                com = new SqlConnection(ConnectionString);
                SqlDependency.Stop(com.ConnectionString);
                SqlDependency.Start(com.ConnectionString);
                com.Open();
                Ex = null;
                return true;
            }
            catch (Exception ex)
            {
                Ex = ex;
                return false;
            }
        }

        //Enumeration to define the direction of the data that is being processed
        private enum Direction { Reciving, Delivering }

        //Delegation to define the data processing method
        private delegate object Process(object Input, Direction Mode);

        //List of dictionaries to store data processing methods for different columns
        private List<Dictionary<string, Process>> Gate = new List<Dictionary<string, Process>>()
        {
            new Dictionary<string, Process>()
            {
                {
                    "@Pass",
                    ProcessPassword
                }
            },
            new Dictionary<string, Process>()
            {
                {
                    "Password",
                    ProcessPassword
                }
            },
        };

        //Method to check if a data processing method exists for a column
        private bool HasProcess(string Col, object Val, Direction Dir, out object ValResult)
        {
            foreach (var item in Gate)
            {

                if (item.ContainsKey(Col))
                {
                    ValResult = item[Col](Val, Dir);
                    return true;
                }
            }

            ValResult = Val;
            return false;
        }

        //Data processing method for passwords
        private static object ProcessPassword(object Input, Direction Mode)
        {
            if (Mode == Direction.Reciving)
            {
                //decode
                var bytes = Convert.FromBase64String(Input.ToString());
                return Encoding.UTF8.GetString(bytes).ToString();
            }
            else
            {
                //encode
                var bytes = Encoding.UTF8.GetBytes(Input.ToString());
                return Convert.ToBase64String(bytes).ToString();
            }
        }

        //Method to insert data into a table
        public void InsertData(string tableName, params (string Col, object Val)[] Data)
        {
            try
            {
                string columnsString = ""; //Initialize an empty string to store column names
                string parameterPlaceholders = ""; //Initialize an empty string to store parameter placeholder

                //Loop the data that will be inserted
                for (int i = 0; i < Data.Length; i++)
                {
                    columnsString += Data[i].Col + ",";
                    parameterPlaceholders += $"@param{i},";

                    //Checks if data needs processing
                    if (HasProcess(Data[i].Col, Data[i].Val, Direction.Delivering, out object Result))
                    {
                        Data[i].Val = Result; //Stores processed data into result
                    }
                }
                columnsString = columnsString.Substring(0, columnsString.Length - 1);
                parameterPlaceholders = parameterPlaceholders.Substring(0, parameterPlaceholders.Length - 1);

                string sqlInsert = $"INSERT INTO {tableName} ({columnsString}) VALUES ({parameterPlaceholders})"; //Creates INSERT command

                //Using the INSERT command created, adds parameters for each value being inserted, then executes the command
                using (SqlCommand cmd = new SqlCommand(sqlInsert, com))
                {
                    for (int i = 0; i < Data.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@param{i}", Data[i].Val);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting data! " + ex.Message);
            }
        }

        //Method to delete data from a table
        public void DeleteData(string tableName, (string Clause, (string, string)[] WhereParams) whereClause)
        {
            try
            {
                string sqlDelete = $"DELETE FROM {tableName} WHERE {whereClause.Clause}"; //Construct Delete statement

                //Creates new command using the DELETE command created
                using (SqlCommand cmd = new SqlCommand(sqlDelete, com))
                {
                    //Checks to make sure row affected is not NULL
                    if (whereClause.WhereParams != null)
                    {
                        for (int i = 0; i < whereClause.WhereParams.Length; i++)
                        {
                            if (HasProcess(whereClause.WhereParams[i].Item1, whereClause.WhereParams[i].Item2, Direction.Delivering, out object Result))
                            {
                                whereClause.WhereParams[i].Item2 = Result.ToString(); //Replaces orignal value with the processed result
                            }
                            cmd.Parameters.AddWithValue(whereClause.WhereParams[i].Item1, whereClause.WhereParams[i].Item2);
                        }
                    }

                    int rowsAffected = cmd.ExecuteNonQuery(); //Ececute delete command
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting data! " + ex.Message);
            }
        }

        //Method to update data in a column
        public void UpdateData(string tableName, (string Clause, (string, string)[] WhereParams) whereClause, params (string Col, object Val)[] Data)
        {
            try
            {
                string setClause = ""; //Intitialize empty string to setClause for updates

                //Loop through data to be updated
                for (int i = 0; i < Data.Length; i++)
                {
                    setClause += $"{Data[i].Col} = @param{i},"; //Construct setClause with column names and parameter placeholders

                    //Check to see if data needs processing
                    if (HasProcess(Data[i].Col, Data[i].Val, Direction.Delivering, out object Result))
                    {
                        Data[i].Val = Result; //Store new data into result
                    }
                }
                setClause = setClause.Substring(0, setClause.Length - 1);

                string sqlUpdate = $"UPDATE {tableName} SET {setClause}" + (string.IsNullOrEmpty(whereClause.Clause) ? "" : $" WHERE {whereClause.Clause}"); //Create update command

                //Creates new command using sqlUpdate
                using (SqlCommand cmd = new SqlCommand(sqlUpdate, com))
                {
                    //Make sure row being updated is not null
                    if (whereClause.WhereParams != null)
                    {
                        //Check to see if whereClause parameters are provied
                        for (int i = 0; i < whereClause.WhereParams.Length; i++)
                        {
                            //If paramters are, then replace orignal value with new result
                            if (HasProcess(whereClause.WhereParams[i].Item1, whereClause.WhereParams[i].Item2, Direction.Delivering, out object Result))
                            {
                                whereClause.WhereParams[i].Item2 = Result.ToString();
                            }
                            cmd.Parameters.AddWithValue(whereClause.WhereParams[i].Item1, whereClause.WhereParams[i].Item2);
                        }
                    }

                    //Add parameters for the SET clause values 
                    for (int i = 0; i < Data.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@param{i}", Data[i].Val);
                    }
                    int rowsAffected = cmd.ExecuteNonQuery(); //Execute update command
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating data! " + ex.Message);
            }
        }

        //Method to retrive data from a specified Database
        public List<Dictionary<string, object>> GetData(string tableName, (string Clause, (string, string)[] WhereParams) whereClause, params string[] Cols)
        {
            //Construct a SQL select statement that must have specified columns, table, and an optional whereClause
            string sqlGet = $"SELECT {string.Join(", ", Cols)} FROM {tableName}" + (string.IsNullOrEmpty(whereClause.Clause) ? "" : $" WHERE {whereClause.Clause}");
            List<Dictionary<string, object>> Ret = new List<Dictionary<string, object>>();

            //Creates new command using sqlGet
            using (SqlCommand cmd = new SqlCommand(sqlGet, com))
            {
                try
                {

                    //Checks WHERE exists
                    if (whereClause.WhereParams != null)
                    {

                        //if whereClause exist loop through parameters
                        for (int i = 0; i < whereClause.WhereParams.Length; i++)
                        {
                            if (HasProcess(whereClause.WhereParams[i].Item1, whereClause.WhereParams[i].Item2, Direction.Delivering, out object Result))
                            {
                                whereClause.WhereParams[i].Item2 = Result.ToString(); //Store new value 
                            }
                            cmd.Parameters.AddWithValue(whereClause.WhereParams[i].Item1, whereClause.WhereParams[i].Item2);
                        }
                    }
                    SqlDataReader Read = cmd.ExecuteReader();
                    while (Read.Read())
                    {
                        //Decods and adds new data into return dictionary
                        Ret.Add(new Dictionary<string, object>());
                        for (int i = 0; i < Cols.Length; i++)
                        {
                            if (!Ret[Ret.Count - 1].ContainsKey(Cols[i]))
                            {
                                Ret[Ret.Count - 1].Add(Cols[i], null);
                            }
                            object In = Read.GetValue(i);
                            if (HasProcess(Cols[i], In, Direction.Reciving, out object Result))
                            {
                                In = Result;
                            }
                            Ret[Ret.Count - 1][Cols[i]] = In;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error getting data! " + ex.Message);
                }
            }
            return Ret;
        }

        public void ResetCallback(string Name = null)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                if (Change.ContainsKey(Name))
                    Change[Name] = null;
                else
                    Change.Add(Name, null);

                if (Ignore.ContainsKey(Name))
                    Ignore[Name] = false;
                else
                    Ignore.Add(Name, false);
            }
            else
            {
                Change.Clear();
                Ignore.Clear();
            }
        }

        public void IgnoreCallback(string Name)
        {
            if (Ignore.ContainsKey(Name))
                Ignore[Name] = true;
            else
                Ignore.Add(Name, true);
        }

        public delegate void ChangeCallback(MainForm Form, Dictionary<string, object> Data);

        public void CreateChangeCallback(string Name, ChangeCallback Event, MainForm Form, Dictionary<string, object> Data, string tableName, (string Clause, (string, string)[] WhereParams) whereClause, params string[] Cols)
        {
            ResetCallback(Name);
            string sqlGet = $"SELECT {string.Join(", ", Cols)} FROM {tableName}" + (string.IsNullOrEmpty(whereClause.Clause) ? "" : $" WHERE {whereClause.Clause}");
            List<Dictionary<string, object>> Ret = new List<Dictionary<string, object>>();

            //Creates new command using sqlGet
            using (SqlCommand cmd = new SqlCommand(sqlGet, com))
            {
                try
                {
                    //Checks WHERE exists
                    if (whereClause.WhereParams != null)
                    {

                        //if whereClause exist loop through parameters
                        for (int i = 0; i < whereClause.WhereParams.Length; i++)
                        {
                            if (HasProcess(whereClause.WhereParams[i].Item1, whereClause.WhereParams[i].Item2, Direction.Delivering, out object Result))
                            {
                                whereClause.WhereParams[i].Item2 = Result.ToString(); //Store new value 
                            }
                            cmd.Parameters.AddWithValue(whereClause.WhereParams[i].Item1, whereClause.WhereParams[i].Item2);
                        }
                    }

                    Change[Name] = new SqlDependency(cmd);
                    Change[Name].OnChange += (object Sender, SqlNotificationEventArgs e) => 
                    { 
                        if (e.Info == SqlNotificationInfo.Update)
                        {
                            try
                            {
                                Form.Invoke(new Action(() =>
                                {
                                    bool In = false;
                                    if (Ignore.ContainsKey(Name))
                                        In = Ignore[Name];
                                    if (!In)
                                    {
                                        Event(Form, Data);
                                        if (Ignore.ContainsKey(Name))
                                            Ignore[Name] = true;
                                        else
                                            Ignore.Add(Name, true);
                                    }
                                }));
                            }
                            catch(Exception eI)
                            {
                                MessageBox.Show(eI.Message);
                            }
                        }
                    };

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Process the DataReader.
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error getting data! " + ex.Message);
                }
            }
        }

    }
}

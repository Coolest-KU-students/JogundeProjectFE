using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace Orders_Register.DataSources.Global
{
    public static class DatabaseManager
    {

        static string connStr = "Server=127.0.0.1;User=root;Database=jogunde;Port=3306;Password=;SSL Mode=None";

        /// <summary>
        /// Tests if connection to the database is possible
        /// </summary>
        public static bool testConnection()
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                Console.WriteLine("Testing Connection");
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            conn.Close();
            return true;
        }

        /// <summary>
        /// Returns MySqlConnection with the database connection
        /// </summary>
        public static MySqlConnection RetrieveDBConnection()
        {
            if (!testConnection())
            {
                throw new Exception("Connection was not established");
            }

            MySqlConnection DBConnection = new MySqlConnection(connStr);
            return DBConnection;
        }

        /// <summary>
        /// Returns MySqlConnection with an opne database connection. Close connection after usage!
        /// </summary>
        public static MySqlConnection RetrieveOpenDBConnection()
        {
            MySqlConnection DBConnection = RetrieveDBConnection();
            DBConnection.Open();
            return DBConnection;
        }

        /// <summary>
        /// Returns MySqlDataReader with data retrieved from query execution
        /// </summary>
        public static DataTable executeReaderQuery(string Query)
        {
            MySqlConnection DBConnection = RetrieveOpenDBConnection();
            MySqlCommand sqlCommand = new MySqlCommand(Query, DBConnection);
            MySqlDataReader mySqlData = sqlCommand.ExecuteReader();

            DataTable resultTable = new DataTable();
            resultTable.Load(mySqlData);

            mySqlData.Close();
            DBConnection.Close();
            return resultTable;
        }

        /// <summary>
        /// Builds and executes a select query based on Object Name and WhereClause. Returns MySqlDataReader with query results
        /// </summary>
        public static DataTable GetDataFromView(string Name, string WhereClause)
        {
            string query = "SELECT * FROM " + Name + (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE " + WhereClause);
            return executeReaderQuery(query);
  
        }

        /// <summary>
        /// Executes a provided query expecting only 1 result. Returns the result as an object
        /// </summary>
        public static object ExecuteScalarQuery(string query)
        {
            MySqlConnection DBConnection = RetrieveOpenDBConnection();
            MySqlCommand sqlCommand = new MySqlCommand(query, DBConnection);
            
            object sqlResult = sqlCommand.ExecuteScalar();

            DBConnection.Close();

            if (sqlResult != null)
            {
                return sqlResult;
            }
            else
            {
                throw new Exception("object returned null");
            }
        }

        /// <summary>
        /// Executes a provided query expecting only 1 result. Outs the result into second parameter as an int
        /// </summary>
        public static void ExecuteQueryWithResultConversion(string query, out int result)
        {
            result = Convert.ToInt32(ExecuteScalarQuery(query));
        }
        /// <summary>
        /// Executes a provided query expecting only 1 result. Outs the result into second parameter as an string
        /// </summary>
        public static void ExecuteQueryWithResultConversion(string query, out string result)
        {
            result = ExecuteScalarQuery(query).ToString();
        }
        /// <summary>
        /// Executes a provided query expecting only 1 result. Outs the result into second parameter as a boolean
        /// </summary>
        public static void ExecuteQueryWithResultConversion(string query, out bool result)
        {
            result = Convert.ToBoolean(ExecuteScalarQuery(query));
        }
        /// <summary>
        /// Executes a resultless query and return the number of rows affected
        /// </summary>
        public static int ExecuteWithoutResult(string query)
        {
            MySqlConnection mySql = RetrieveOpenDBConnection();
            MySqlCommand command = new MySqlCommand(query, mySql);
            int rowsAffected = command.ExecuteNonQuery();
            mySql.Close();
            return rowsAffected;
        }

        /// <summary>
        /// Rxecutes a COUNT(*) on provided View's name and whereClause. 
        /// </summary>
        public static int GetRowCountFromView(string Name, string WhereClause)
        {
            string query = "SELECT COUNT(1) FROM " + Name + (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE " + WhereClause);
            int count;
            ExecuteQueryWithResultConversion(query, out count);
            return count;
        }
        /// <summary>
        /// Builds and executes a select to check if the columns actually exist in the object
        /// </summary>
        public static void CheckColumnExistenceInTable(string Name, List<string> ColumnNames)
        {
            string query = "SELECT 1 WHERE EXISTS(SELECT @REPLACE@ FROM " + Name + ")";

            string columnNames = "";
            foreach(string column in ColumnNames)
            {
                columnNames += column + ",";
            }
            columnNames += "1";

            query = query.Replace("@REPLACE@", columnNames);
            try
            {
                object result = ExecuteScalarQuery(query);
            }
            catch(Exception ex)
            {
                throw new ArgumentException("A Select with provided column names threw an error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Will insert provided Column:Value pairs into the provided Name.
        /// </summary>
        public static void InsertRowToTable(string Name, Dictionary<string, string> columnValuePairs)
        {
            CheckColumnExistenceInTable(Name, new List<string>(columnValuePairs.Keys));

            string query = "INSERT INTO " + Name + " (@@COLUMN_NAMES@@) VALUES (@@VALUES@@) ";

            string columnNames = "";
            string insertValues = "";
            
            foreach(KeyValuePair<string, string> columnValuePair in columnValuePairs)
            {
                columnNames += columnValuePair.Key + ", ";
                insertValues += "'" + BeautifyStringForQuery(columnValuePair.Value) + "', ";
            }

            columnNames += "~~";
            insertValues += "~~";
            columnNames = columnNames.Replace(", ~~", "");
            insertValues = insertValues.Replace(", ~~", "");

            query = query.Replace("@@COLUMN_NAMES@@", columnNames).Replace("@@VALUES@@", insertValues);

            ExecuteWithoutResult(query);
        }

        /// <summary>
        /// Will insert provided list of Column:Value pairs into the provided Name.
        /// </summary>
        public static void InsertRowsToTable(string Name, List<Dictionary<string, string>> columnValuePairList)
        {
            foreach(Dictionary<string, string> keyValuePairs in columnValuePairList)
            {
                InsertRowToTable(Name, keyValuePairs);
            }
        }
        /// <summary>
        /// Will update provided columnValuePairs based on the where clause and return number of rows that were updated.
        /// </summary>
        public static int UpdateTableValues(string Name, Dictionary<string, string> columnValuePairs, string whereClause)
        {
            string query = "UPDATE " + Name + " SET @@PAIR_LISTING@@ " + (string.IsNullOrEmpty(whereClause) ? "" : " WHERE " + whereClause);

            CheckColumnExistenceInTable(Name, new List<string>(columnValuePairs.Keys));

            string PairListing = "";

            foreach (KeyValuePair<string, string> columnValuePair in columnValuePairs)
            {
                PairListing += columnValuePair.Key + " = '" + BeautifyStringForQuery(columnValuePair.Value) + "', ";
            }

            PairListing += "~~";
            query = query.Replace("@@PAIR_LISTING@@", PairListing.Replace(", ~~", ""));

            return ExecuteWithoutResult(query);
        }


        /// <summary>
        /// Will delete provided data from table based on whereClause. 
        /// </summary>
        public static int DeleteTableValues(string Name, string whereClause, bool whereClauseIsEmpty)
        {
            if(string.IsNullOrEmpty(whereClause) && !whereClauseIsEmpty)
            {
                throw new ArgumentNullException("whereClause was provided Empty");
            }
            else if(!string.IsNullOrEmpty(whereClause) && whereClauseIsEmpty)
            {
                throw new ArgumentNullException("whereClause was provided, while it was specified to be empty");
            }

            string query = "DELETE FROM " + Name + (string.IsNullOrEmpty(whereClause) ? "" : " WHERE " + whereClause);

            return ExecuteWithoutResult(query);
        }

        /// <summary>
        /// Will ensure that the provided string will not break the code.
        /// </summary>
        static string BeautifyStringForQuery(string query)
        {
            return query.Replace("'", "''");
        }
    }
  
}


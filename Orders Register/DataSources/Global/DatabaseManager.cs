using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace WpfApp1.DataSources.Global
{
    public static class DatabaseManager
    {
        public enum ConversionTypes
        {
            INT32 = 1,
            STRING = 2,
            BOOL = 3
        }

        static string connStr = "Server=127.0.0.1;User=root;Database=jogunde;Port=3306;Password=;SSL Mode=None";
        
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

        public static MySqlConnection RetrieveDBConnection()
        {
            if (!testConnection())
            {
                throw new Exception("Connection was not established");
            }

            MySqlConnection DBConnection = new MySqlConnection(connStr);
            return DBConnection;
        }
        public static MySqlConnection RetrieveOpenDBConnection()
        {
            MySqlConnection DBConnection = RetrieveDBConnection();
            DBConnection.Open();
            return DBConnection;
        }

        public static MySqlDataReader executeReaderQuery(string Query)
        {
            MySqlConnection DBConnection = RetrieveOpenDBConnection();
            MySqlCommand sqlCommand = new MySqlCommand(Query, DBConnection);
            MySqlDataReader mySqlData = sqlCommand.ExecuteReader();
            DBConnection.Close();
            return mySqlData;
        }

        public static MySqlDataReader GetDataFromView(string Name, string WhereClause)
        {
            string query = "SELECT * FROM " + Name + (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE " + WhereClause);
            return executeReaderQuery(query);
        }

        public static object ExecuteScalarQuery(string query)
        {
            MySqlConnection DBConnection = RetrieveOpenDBConnection();
            MySqlCommand sqlCommand = new MySqlCommand(query, DBConnection);

            object sqlResult = sqlCommand.ExecuteScalar();

            if (sqlResult != null)
            {
                return sqlResult;
            }
            else
            {
                throw new Exception("object returned null");
            }
        }

        public static void ExecuteQueryWithResultConversion(string query, out int result)
        {
            result = Convert.ToInt32(ExecuteScalarQuery(query));
        }

        public static void ExecuteQueryWithResultConversion(string query, out string result)
        {
            result = ExecuteScalarQuery(query).ToString();
        }
        public static void ExecuteQueryWithResultConversion(string query, out bool result)
        {
            result = Convert.ToBoolean(ExecuteScalarQuery(query));
        }
        public static int GetRowCountFromView(string Name, string WhereClause)
        {
            string query = "SELECT COUNT(1) FROM " + Name + (string.IsNullOrEmpty(WhereClause) ? "" : " WHERE " + WhereClause);
            int count;
            ExecuteQueryWithResultConversion(query, out count);
            return count;
        }

        //public static DataTable getDataFromView(string Name)
        //{
        //    MySqlConnection DBConnection = retrieveDBConnection();

        //}
    }

    
}


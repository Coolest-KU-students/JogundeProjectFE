using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orders_Register.DataSources.Global;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace Orders_Register.DataSources
{
    static class FoodTypesRepo
    {
        private static string TableName = "tbl_FoodTypes";

        public class FoodType : Global.GlobalParentTable
        {
            int ID;
            string Name;

            public FoodType(DateTime Created, DateTime Updated, int ID, string Name)
            {
                this.Created = Created;
                this.Updated = Updated;
                this.ID = ID;
                this.Name = Name;
            }
            public FoodType(string Name)
            {
                this.Name = Name;
            }

            public int getID()
            {
                return ID;
            }

            public string getName()
            {
                return Name;
            }

            public void setName(string Name)
            {
                this.Name = Name;
            }

            public Dictionary<string, string> ToDictionary()
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                result.Add("ID", getID().ToString());
                result.Add("Name", getName());
                
                return result;
            }
            public Dictionary<string, string> ToDictionaryWithoutID()
            {
                Dictionary<string, string> result = ToDictionary();
                result.Remove("ID");
                return result;
            }
        }

        public static FoodType extractFoodType(DataRow vRow)
        {
            return new FoodType(Convert.ToDateTime(vRow["Created"]), Convert.ToDateTime(vRow["Updated"]), Convert.ToInt32(vRow["ID"]), vRow["Name"].ToString());
        }

        public static FoodType getByID(int ID)
        {
            DataRow vRow = DatabaseManager.GetDataFromView(TableName, "ID = '" + ID + "'").Rows[0];
            return extractFoodType(vRow);
        }

        public static List<FoodType> GetAllFoodTypes()
        {
            DataTable foodTypeTable = DatabaseManager.GetDataFromView(TableName, "");
            List<FoodType> foodTypes = new List<FoodType>();

            foreach(DataRow vRow in foodTypeTable.Rows)
            {
                foodTypes.Add(extractFoodType(vRow));
            }

            return foodTypes;
        }

        public static void Save(FoodType FT)
        {
            DatabaseManager.InsertRowToTable(TableName, FT.ToDictionaryWithoutID());
        }

        public static void Update(FoodType FT)
        {
            string whereClause = "ID = " + FT.getID().ToString();
            DatabaseManager.UpdateTableValues(TableName, FT.ToDictionaryWithoutID(), whereClause);
        }

        public static void Delete(FoodType FT)
        {
            string whereClause = "ID = " + FT.getID().ToString();
            DatabaseManager.DeleteTableValues(TableName, whereClause, false);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataSources.Global;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace Orders_Register.DataSources
{
    static class FoodTypes
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

        }

        public static List<FoodType> GetAllFoodTypes()
        {
            MySqlDataReader dataReader = DatabaseManager.GetDataFromView(TableName, "");
            DataTable foodTypeTable = dataReader.GetSchemaTable();

            List<FoodType> foodTypes = new List<FoodType>();

            foreach(DataRow vRow in foodTypeTable.Rows)
            {
                foodTypes.Add(new FoodType(Convert.ToDateTime(vRow["Created"]), Convert.ToDateTime(vRow["Updated"]), Convert.ToInt32(vRow["ID"]), vRow["Name"].ToString()));
            }

            return foodTypes;
        }
    }
}

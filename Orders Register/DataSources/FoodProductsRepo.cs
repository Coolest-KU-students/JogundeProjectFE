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
    static class FoodProductsRepo
    {
        private static string TableName = "tbl_FoodProducts";

        public class FoodProduct : Global.GlobalParentTable
        {
            int ID;
            string Name;
            int TypeID;
            double Price;

            public FoodProduct(DateTime Created, DateTime Updated, int ID, string Name, int TypeID, double Price)
            {
                this.Created = Created;
                this.Updated = Updated;
                this.ID = ID;
                this.Name = Name;
                this.TypeID = TypeID;
                this.Price = Price;
            }
            public FoodProduct(DateTime Created, DateTime Updated, int ID, string Name, FoodTypesRepo.FoodType foodType, double Price)
            {
                this.Created = Created;
                this.Updated = Updated;
                this.ID = ID;
                this.Name = Name;
                this.TypeID = foodType.getID();
                this.Price = Price;
            }
            public FoodProduct(string Name, int TypeID, double Price)
            {

                this.Name = Name;
                this.TypeID = TypeID;
                this.Price = Price;
            }
            public FoodProduct(string Name, FoodTypesRepo.FoodType foodType, double Price)
            {

                this.Name = Name;
                this.TypeID = foodType.getID();
                this.Price = Price;
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

            public void setTypeID(int TypeID)
            {
                this.TypeID = TypeID;
            }

            public int getTypeID()
            {
                return TypeID;
            }

            public double getPrice()
            {
                return Price;
            }

            public void setPrice(double Price)
            {
                this.Price = Price;
            }

            public Dictionary<string, string> ToDictionary()
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                result.Add("ID", getID().ToString());
                result.Add("Name", getName());
                result.Add("TypeID", getTypeID().ToString());
                result.Add("Price", getPrice().ToString());

                return result;
            }
            public Dictionary<string, string> ToDictionaryWithoutID()
            {
                Dictionary<string, string> result = ToDictionary();
                result.Remove("ID");
                return result;
            }
        }

        public static FoodProduct extractFoodProduct(DataRow vRow)
        {
            return new FoodProduct(Convert.ToDateTime(vRow["Created"]), Convert.ToDateTime(vRow["Updated"]), Convert.ToInt32(vRow["ID"]), vRow["Name"].ToString(), Convert.ToInt32(vRow["TypeID"]), Convert.ToDouble(vRow["Price"]));
        }

        public static FoodProduct getByID(int ID)
        {
            DataRow vRow = DatabaseManager.GetDataFromView(TableName, "ID = '" + ID + "'").Rows[0];
            return extractFoodProduct(vRow);
        }

        public static List<FoodProduct> getByTypeID(int TypeID)
        {
            DataTable dataTable = DatabaseManager.GetDataFromView(TableName, "TypeID = '" + TypeID + "'");
            List<FoodProduct> foodProductsList = new List<FoodProduct>();

            foreach(DataRow vRow in dataTable.Rows)
            {
                foodProductsList.Add(extractFoodProduct(vRow));
            }

            return foodProductsList;
        }

        public static List<FoodProduct> GetAllFoodProducts()
        {
            DataTable foodTypeTable = DatabaseManager.GetDataFromView(TableName, "");
            List<FoodProduct> foodTypes = new List<FoodProduct>();

            foreach (DataRow vRow in foodTypeTable.Rows)
            {
                foodTypes.Add(extractFoodProduct(vRow));
            }

            return foodTypes;
        }

        public static void Save(FoodProduct FT)
        {
            DatabaseManager.InsertRowToTable(TableName, FT.ToDictionaryWithoutID());
        }

        public static void Update(FoodProduct FT)
        {
            string whereClause = "ID = " + FT.getID().ToString();

            DatabaseManager.UpdateTableValues(TableName, FT.ToDictionaryWithoutID(), whereClause);
        }

        public static void Delete(FoodProduct FT)
        {
            string whereClause = "ID = " + FT.getID().ToString();
            DatabaseManager.DeleteTableValues(TableName, whereClause, false);
        }
    }
}

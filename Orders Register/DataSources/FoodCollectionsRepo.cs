using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orders_Register.DataSources.Global;
using Orders_Register.DataSources;
using System.Data;

namespace Orders_Register.DataSources
{
    static class FoodCollectionsRepo
    {
        private static string TableName = "tbl_FoodCollections";


        public class FoodCollection : GlobalParentTable
        {
            int ID;
            int OrderID;
            int Amount;

            public FoodCollection(DateTime Created, DateTime Updated, int ID, int OrderID, int Amount)
            {
                this.Created = Created;
                this.Updated = Updated;
                this.ID = ID;
                this.OrderID = OrderID;
                this.Amount = Amount;
            }

            public FoodCollection(int OrderID, int Amount)
            {
                this.OrderID = OrderID;
                this.Amount = Amount;
            }

            public int getID() { return ID; }
            public int getOrderID() { return OrderID; }
            public int getAmount() { return Amount; }
            public void setAmount(int Amount) { this.Amount = Amount;}

            public Dictionary<string, string> ToDictionary()
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                result.Add("ID", getID().ToString());
                result.Add("OrderID", getOrderID().ToString());
                result.Add("Amount", getAmount().ToString());

                return result;
            }
            public Dictionary<string, string> ToDictionaryWithoutID()
            {
                Dictionary<string, string> result = ToDictionary();
                result.Remove("ID");
                return result;
            }

            public List<FoodProductsRepo.FoodProduct> getAllProductsFromTheCollection()
            {
                return ProductsCollectionsRepo.getByProductID(getID());
            }
        }
    
        public static FoodCollection extractFoodCollection(DataRow vRow)
        {
            return new FoodCollection(Convert.ToDateTime(vRow["Created"]), Convert.ToDateTime(vRow["Updated"]), Convert.ToInt32(vRow["ID"]), Convert.ToInt32(vRow["OrderID"]), Convert.ToInt32(vRow["Amount"]));
        }

        public static FoodCollection getByID(int ID)
        {
            
            DataRow vRow = DatabaseManager.GetDataFromView(TableName, "ID = '" + ID + "'").Rows[0];
            return extractFoodCollection(vRow);
        }

        public static List<FoodCollection> getByOrderID(int OrderID)
        {
            DataTable dataTable = DatabaseManager.GetDataFromView(TableName, "OrderID = '" + OrderID + "'");
            List<FoodCollection> FoodCollectionsList = new List<FoodCollection>();

            foreach(DataRow vRow in dataTable.Rows)
            {
                FoodCollectionsList.Add(extractFoodCollection(vRow));
            }
            
            return FoodCollectionsList;
        }

        public static List<FoodCollection> GetAllFoodCollections()
        {
            DataTable foodTypeTable = DatabaseManager.GetDataFromView(TableName, "");
            List<FoodCollection> foodTypes = new List<FoodCollection>();

            foreach (DataRow vRow in foodTypeTable.Rows)
            {
                foodTypes.Add(extractFoodCollection(vRow));
            }

            return foodTypes;
        }

        public static void Save(FoodCollection FT)
        {
            DatabaseManager.InsertRowToTable(TableName, FT.ToDictionaryWithoutID());
        }

        public static void Update(FoodCollection FT)
        {
            string whereClause = "ID = " + FT.getID().ToString();

            DatabaseManager.UpdateTableValues(TableName, FT.ToDictionaryWithoutID(), whereClause);
        }

        public static void Delete(FoodCollection FT)
        {
            string whereClause = "ID = " + FT.getID().ToString();
            DatabaseManager.DeleteTableValues(TableName, whereClause, false);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Orders_Register.DataSources;
using Orders_Register.DataSources.Global;

namespace Orders_Register.DataSources
{
    static class ProductsCollectionsRepo
    {
        private static string TableName = "tbl_productscollections";

        public class ProductsCollection : GlobalParentTable 
        {
            int ID;
            FoodCollectionsRepo.FoodCollection foodCollection;
            FoodProductsRepo.FoodProduct foodProduct;

            public ProductsCollection(DateTime Created, DateTime Updated, int ID, int collectionID, int productID)
            {
                this.Created = Created;
                this.Updated = Updated;
                this.ID = ID;
                this.foodCollection = FoodCollectionsRepo.getByID(collectionID);
                this.foodProduct = FoodProductsRepo.getByID(productID);
            }

            public ProductsCollection(FoodCollectionsRepo.FoodCollection foodCollection, FoodProductsRepo.FoodProduct foodProduct)
            {
                this.foodCollection = foodCollection;
                this.foodProduct = foodProduct;
            }
        
            public int getID() { return ID; }
            public FoodCollectionsRepo.FoodCollection GetFoodCollection() { return foodCollection; }
            public void setFoodCollection(FoodCollectionsRepo.FoodCollection foodCollection) { this.foodCollection = foodCollection; }
            public FoodProductsRepo.FoodProduct GetFoodProduct() { return foodProduct; }
            public void setFoodProduct(FoodProductsRepo.FoodProduct foodProduct) { this.foodProduct = foodProduct; }

            public Dictionary<string, string> ToDictionary()
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                result.Add("ID", getID().ToString());
                result.Add("collectionID", GetFoodCollection().getID().ToString());
                result.Add("productID", GetFoodProduct().getID().ToString());

                return result;
            }

            public Dictionary<string, string> ToDictionaryWithoutID()
            {
                Dictionary<string, string> result = ToDictionary();
                result.Remove("ID");
                return result;
            }

        }

        public static ProductsCollection extractProductsCollection(DataRow vRow)
        {
            return new ProductsCollection(Convert.ToDateTime(vRow["Created"]), Convert.ToDateTime(vRow["Updated"]), Convert.ToInt32(vRow["ID"]), Convert.ToInt32(vRow["collectionID"]), Convert.ToInt32(vRow["productID"].ToString()));
        }

        public static ProductsCollection getByID(int ID)
        {
            DataRow vRow = DatabaseManager.GetDataFromView(TableName, "ID = '" + ID + "'").Rows[0];
            return extractProductsCollection(vRow);
        }
        public static List<FoodCollectionsRepo.FoodCollection> getByProductID(int productID)
        {
            DataTable dataTable = DatabaseManager.GetDataFromView(TableName, "productID = '" + productID + "'");
            List<FoodCollectionsRepo.FoodCollection> FoodCollectionsList = new List<FoodCollectionsRepo.FoodCollection>();

            foreach (DataRow vRow in dataTable.Rows)
            {
                FoodCollectionsList.Add(extractProductsCollection(vRow).GetFoodCollection());
            }

            return FoodCollectionsList;
        }

        public static List<FoodProductsRepo.FoodProduct> getByCollectionID(int collectionID)
        {
            DataTable dataTable = DatabaseManager.GetDataFromView(TableName, "collectionID = '" + collectionID + "'");
            List<FoodProductsRepo.FoodProduct> FoodCollectionsList = new List<FoodProductsRepo.FoodProduct>();

            foreach (DataRow vRow in dataTable.Rows)
            {
                FoodCollectionsList.Add(extractProductsCollection(vRow).GetFoodProduct());
            }

            return FoodCollectionsList;
        }

        public static List<ProductsCollection> GetAllProductsCollections()
        {
            DataTable ProductsCollectionTable = DatabaseManager.GetDataFromView(TableName, "");
            List<ProductsCollection> ProductsCollection = new List<ProductsCollection>();

            foreach (DataRow vRow in ProductsCollectionTable.Rows)
            {
                ProductsCollection.Add(extractProductsCollection(vRow));
            }

            return ProductsCollection;
        }

        public static void Save(ProductsCollection O)
        {
            DatabaseManager.InsertRowToTable(TableName, O.ToDictionaryWithoutID());
        }

        public static void Update(ProductsCollection O)
        {
            string whereClause = "ID = " + O.getID().ToString();
            DatabaseManager.UpdateTableValues(TableName, O.ToDictionaryWithoutID(), whereClause);
        }

        public static void Delete(ProductsCollection O)
        {
            string whereClause = "ID = " + O.getID().ToString();
            DatabaseManager.DeleteTableValues(TableName, whereClause, false);
        }
    }
}

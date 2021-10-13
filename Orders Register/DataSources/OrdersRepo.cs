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
    static class OrdersRepo
    {
        private static string TableName = "tbl_orders";

        public class Orders : GlobalParentTable
        {
            int ID;
            string name;
            string address;
            string phoneNumber;
            string paymentMethod;
            bool status;
        
            public Orders(DateTime Created, DateTime Updated, int ID, string name, string address, string phoneNumber, string paymentMethod, bool status)
            {
                this.Created = Created;
                this.Updated = Updated;
                this.ID = ID;
                this.name = name;
                this.address = address;
                this.phoneNumber = phoneNumber;
                this.paymentMethod = paymentMethod;
                this.status = status;
            }
            
            public Orders(string name, string address, string phoneNumber, string paymentMethod)
            {
                this.name = name;
                this.address = address;
                this.phoneNumber = phoneNumber;
                this.paymentMethod = paymentMethod;
            }

            public int getID() { return ID; }

            public string getName() { return name; }

            public void setName(string name) { this.name = name; }

            public string getAddress() { return address; }

            public void setAddress(string address) { this.address = address; }

            public string getPhoneNumber() { return phoneNumber; }

            public void setPhoneNumber(string phoneNumber) { this.phoneNumber = phoneNumber; }

            public string getPaymentMethod() { return paymentMethod; }

            public void setPaymentMethod(string paymentMethod) { this.paymentMethod = paymentMethod; }

            public bool getStatus() { return status; }

            public void setStatus(bool status) { this.status = status; }

            public Dictionary<string, string> ToDictionary()
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                result.Add("ID", getID().ToString());
                result.Add("Name", getName());
                result.Add("Address", getAddress());
                result.Add("PhoneNumber", getPhoneNumber());
                result.Add("PaymentMethod", getPaymentMethod());
                result.Add("Status", getStatus().ToString());

                return result;
            }
            public Dictionary<string, string> ToDictionaryWithoutID()
            {
                Dictionary<string, string> result = ToDictionary();
                result.Remove("ID");
                return result;
            }

            public List<FoodCollectionsRepo.FoodCollection> getAllFoodCollectionsFromOrder()
            {
                return FoodCollectionsRepo.getByOrderID(getID());
            }
        }

        public static Orders extractFoodType(DataRow vRow)
        {
            return new Orders(Convert.ToDateTime(vRow["Created"]), Convert.ToDateTime(vRow["Updated"]), Convert.ToInt32(vRow["ID"]), vRow["Name"].ToString(), vRow["Address"].ToString(), vRow["PhoneNumber"].ToString(), vRow["PaymentMethod"].ToString(), Convert.ToBoolean(vRow["Status"]));
        }

        public static Orders getByID(int ID)
        {
            DataRow vRow = DatabaseManager.GetDataFromView(TableName, "ID = '" + ID + "'").Rows[0];
            return extractFoodType(vRow);
        }

        public static List<Orders> GetAllFoodTypes()
        {
            DataTable ordersTable = DatabaseManager.GetDataFromView(TableName, "");
            List<Orders> orders = new List<Orders>();

            foreach (DataRow vRow in ordersTable.Rows)
            {
                orders.Add(extractFoodType(vRow));
            }

            return orders;
        }

        public static void Save(Orders O)
        {
            DatabaseManager.InsertRowToTable(TableName, O.ToDictionaryWithoutID());
        }

        public static void Update(Orders O)
        {
            string whereClause = "ID = " + O.getID().ToString();
            DatabaseManager.UpdateTableValues(TableName, O.ToDictionaryWithoutID(), whereClause);
        }

        public static void Delete(Orders O)
        {
            string whereClause = "ID = " + O.getID().ToString();
            DatabaseManager.DeleteTableValues(TableName, whereClause, false);
        }
    }
}

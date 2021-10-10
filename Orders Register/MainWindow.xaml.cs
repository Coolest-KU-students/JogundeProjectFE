using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orders_Register
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            Orders.Orders orders = new Orders.Orders();
            orders.Show();
        }

        private void NewOrderButton_Click(object sender, RoutedEventArgs e)
        {
            New_Order.NewOrder newOrder = new New_Order.NewOrder();
            newOrder.Show();
        }

        private void NewOrderForCompanyButton_Click(object sender, RoutedEventArgs e)
        {
            New_Order_for_a_Company.NewOrderForCompany newOrderForComapny = new New_Order_for_a_Company.NewOrderForCompany();
            newOrderForComapny.Show();
        }

        private void OrderDraftsCompaniesButton_Click(object sender, RoutedEventArgs e)
        {
            Order_Drafts_for_Companies.OrderDraftsCompanies orderDraftsCompanies = new Order_Drafts_for_Companies.OrderDraftsCompanies();
            orderDraftsCompanies.Show();
        }

        private void CompaniesRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Companies_Register.CompaniesRegister companiesRegister = new Companies_Register.CompaniesRegister();
            companiesRegister.Show();
        }

        private void FoodRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Food_Register.FoodRegister foodRegister = new Food_Register.FoodRegister();
            foodRegister.Show();
        }
    }
}

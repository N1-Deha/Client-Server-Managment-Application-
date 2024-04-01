using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace WpfTest

{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        string str = null;
        public MainView()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }   
        }

        private string GetConnectionString()
        {
            
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["AdventureWorks2019"];
            if (settings != null)
                return settings.ConnectionString;
            throw new Exception("Connection string for AdventureWorks2019 not found.");
        }
        private void LoadData()
        {
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT pp.BusinessEntityID as CustomerID,LastName,MiddleName,FirstName,PersonType , ss.Name as 'Store Name'\r\nfrom Person.Person as pp\r\n " +
                        "join Sales.Customer as S on pp.BusinessEntityID=s.CustomerID JOIN Sales.Store as SS on s.StoreID=ss.BusinessEntityID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    MyDataGrid.ItemsSource = dt.DefaultView;    

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                LoadData(); 
                return;
            }

            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    try
                    {
                        con.Open();
                        string query = $"SELECT BusinessEntityID as CustomerID,LastName,MiddleName,FirstName,PersonType\r\nfrom Person.Person \r\n join Sales.Customer as S on Person.Person.BusinessEntityID=s.CustomerID " +
                            $" WHERE LastName LIKE '{txtSearch.Text}%'" +
                            $" ORDER BY LastName ";
                        SqlCommand cmd = new SqlCommand(query, con);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        MyDataGrid.ItemsSource = dt.DefaultView;
                        str = txtSearch.Text;

                    }
                    
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                    }
                    
                    if (txtSearch.Text.Contains(" "))
                    {
                        int spaceIndex = txtSearch.Text.IndexOf(" ");
                        string lastName = txtSearch.Text.Substring(0, spaceIndex);
                        string firstName = txtSearch.Text.Substring(spaceIndex + 1);
                        string query = $"SELECT BusinessEntityID as CustomerID,LastName , MiddleName,FirstName,PersonType\r\nfrom Person.Person \r\n join Sales.Customer as S on Person.Person.BusinessEntityID = s.CustomerID " +
                            $" WHERE LastName LIKE '{lastName}%' AND FirstName Like '{firstName}%'" +
                            $" ORDER BY LastName ";
                        SqlCommand cmd = new SqlCommand(query, con);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        MyDataGrid.ItemsSource = dt.DefaultView;
                    }
                }
                con.Close();
                bool toInt = int.TryParse(txtSearch.Text, out int result);
                if(toInt)
                {
                    con.Open();
                    string query = $"SELECT BusinessEntityID as CustomerID,FirstName , MiddleName,LastName,PersonType\r\nfrom Person.Person \r\n join Sales.Customer as S on Person.Person.BusinessEntityID = s.CustomerID" +
                        $" WHERE BusinessEntityID LIKE '{txtSearch.Text}%'" +
                        $" ORDER BY BusinessEntityID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    MyDataGrid.ItemsSource = dt.DefaultView;
                    str = txtSearch.Text;
                }
                con.Close();
                
                
            }
        }

        private void addCustomer_Click(object sender, RoutedEventArgs e)
        {
            AddCustomer a = new AddCustomer();
            a.ShowDialog();
            
        }

    }

}

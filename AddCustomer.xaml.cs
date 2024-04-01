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
using System.Data.Common;

namespace WpfTest

{

    public partial class AddCustomer : Window
    {
        public AddCustomer()
        {
            InitializeComponent();
            LoadPersonTypeData();
            LoadData();
            LoadAddressData();
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
        private void LoadPersonTypeData()
        {
            
            SqlCommand command;
            SqlDataReader dataReader;
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT DISTINCT PersonType FROM Person.Person ";
                    command = new SqlCommand(query, con);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        cmbPersonType.Items.Add(dataReader.GetValue(0));
                    }
                    dataReader.Close();
                    command.Dispose();
                    con.Close();
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }
        private void LoadAddressData()
        {

            SqlCommand command;
            SqlDataReader dataReader;
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT [Name] FROM [AdventureWorks2019].[Person].[AddressType]";
                    command = new SqlCommand(query, con);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        cmbAdressType.Items.Add(dataReader.GetValue(0));
                    }
                    dataReader.Close();
                    command.Dispose();
                    con.Close();
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mnmzBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
                        

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
                         string query = $"SELECT [BusinessEntityID],[Name] FROM [AdventureWorks2019].[Sales].[Store]" +
                             $" WHERE Name LIKE '{txtSearch.Text}%'" +
                             $" ORDER BY Name ";
                         SqlCommand cmd = new SqlCommand(query, con);
                         SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                         DataTable dt = new DataTable();
                         adapter.Fill(dt);
                        storeGrid.ItemsSource = dt.DefaultView;


                     }
                     catch (Exception ex)
                     {
                         MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                     }
                 }
                 con.Close();
                bool toInt = int.TryParse(txtSearch.Text, out int result);
                if (toInt)
                {
                    try
                    {
                        con.Open();
                        string query = $"SELECT [BusinessEntityID],[Name] FROM [AdventureWorks2019].[Sales].[Store]" +
                            $" WHERE BusinessEntityID LIKE '{txtSearch.Text}%'" +
                            $" ORDER BY BusinessEntityID ";
                        SqlCommand cmd = new SqlCommand(query, con);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        storeGrid.ItemsSource = dt.DefaultView;


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                    }
                }
                con.Close();


            }
         }
        private void LoadData()
        {
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT TOP (1000) [BusinessEntityID],[Name] FROM [AdventureWorks2019].[Sales].[Store]";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    storeGrid.ItemsSource = dt.DefaultView;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }

            private void ExecTrigger()
            {
                string connectionString = GetConnectionString();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {  
                conn.Open();
                

                    // 1.  create a command object identifying the stored procedure
                SqlCommand cmd = new SqlCommand("InsertingCustomer", conn);

                    // 2. set the command object so it knows to execute a stored procedure
                 cmd.CommandType = CommandType.StoredProcedure;

                int businessEntityId = 0;

                businessEntityId = gridDataReturn(businessEntityId);


                cmd.Parameters.Add(new SqlParameter("@FirstName",txtFirstName.Text));
                 cmd.Parameters.Add(new SqlParameter("@LastName", txtLastName.Text));
                 cmd.Parameters.Add(new SqlParameter("@PersonType", cmbPersonType.SelectedItem));
                 cmd.Parameters.Add(new SqlParameter("@AddressLine1", txtAddressLine1.Text));
                 cmd.Parameters.Add(new SqlParameter("@AddressLine2", txtAddressLine2.Text));
                 cmd.Parameters.Add(new SqlParameter("@AddressType", 2/*Convert.ToInt32(cmbAdressType.SelectedItem.ToString()))*/));
                 cmd.Parameters.Add(new SqlParameter("@PostalCode", Convert.ToInt32(txtPostal.Text.ToString())));
                 cmd.Parameters.Add(new SqlParameter("@country", txtCountry.Text));
                 cmd.Parameters.Add(new SqlParameter("@StateProvince", txtState.Text));
                 cmd.Parameters.Add(new SqlParameter("@City", txtCity.Text));
                 cmd.Parameters.Add(new SqlParameter("@StoreID", businessEntityId));

                // execute the command
                //SqlDataReader rdr = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();

            }
            }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ExecTrigger();
                MessageBox.Show("Customer Successfully Added!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtAddressLine1.Text = string.Empty;
            txtAddressLine2.Text = string.Empty;
            txtPostal.Text = string.Empty;
            txtCountry.Text = string.Empty;
            txtState.Text = string.Empty;
            txtCity.Text = string.Empty;
            cmbPersonType.SelectedIndex = -1;
            cmbAdressType.SelectedIndex = -1;
            storeGrid.UnselectAll();

        }

        private int gridDataReturn(int businessEntityId)    
        {
            businessEntityId = 0;
            if (storeGrid.SelectedItem != null)
            {
                // Cast the SelectedItem to a DataRowView
                DataRowView rowView = (DataRowView)storeGrid.SelectedItem;

                // Then access your data by column name, assuming the column is of type int
               businessEntityId = (int)rowView["BusinessEntityID"];



                // Use the businessEntityId in your code...
            }
            return businessEntityId;
        }

        private void txtFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
    }






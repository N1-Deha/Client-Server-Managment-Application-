using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace WpfTest
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton==MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState=WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void enterBtn_Click(object sender, RoutedEventArgs e)
        {
            MainView main = new MainView();
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string password = txtPassword.Password.ToString();

                try
                {
                    con.Open();
                    string query = $"select count(*) as Result from dbo.adminUsers where username ='{txtUsername.Text.Trim()}' and password = '{password.Trim()}'";
                    
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    int result = (int)cmd.ExecuteScalar();
                    if (result>0) 
                    {
                        main.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Error!\nWrong Username Or Password");
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }
        private string GetConnectionString()
        {

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["AdventureWorks2019"];
            if (settings != null)
                return settings.ConnectionString;
            throw new Exception("Connection string for AdventureWorks2019 not found.");
        }


    }
}
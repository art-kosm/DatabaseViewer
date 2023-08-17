using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string m_connectionString = string.Empty;

        public MainWindow()
        {
            while (true)
            {
                LoginDialog dialog = new();
                if (!dialog.ShowDialog().GetValueOrDefault(false))
                {
                    Close();
                    return;
                }

                m_connectionString = dialog.GetCredentials().ConnectionString;
                using SqlConnection connection = new(m_connectionString);
                bool error = false;
                try
                {
                    connection.Open();
                }
                catch (SqlException ex)
                {
                    error = true;
                    MessageBox.Show(ex.Message);
                }

                if (!error)
                {
                    break;
                }
            }

            InitializeComponent();
        }


        void ExecuteQuery(string query)
        {
            using SqlConnection connection = new(m_connectionString);
            connection.Open();

            SqlCommand command = new(query, connection);

            SqlDataAdapter adapter = new(command);
            DataTable dt = new();
            try
            {
                adapter.Fill(dt);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            grid.ItemsSource = dt.DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ExecuteQuery(queryEdit.Text);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void LoadQueryTemplate(object sender, SelectionChangedEventArgs e)
        {
            if (queryTemplateComboBox.SelectedIndex == 0)
            {
                return;
            }

            queryEdit.Text = queryTemplateComboBox.SelectedItem.ToString();
            queryTemplateComboBox.SelectedIndex = 0;
        }
    }
}

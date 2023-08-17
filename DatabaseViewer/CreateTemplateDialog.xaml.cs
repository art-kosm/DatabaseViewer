using System.Windows;

namespace DatabaseViewer
{
    /// <summary>
    /// Interaction logic for CreateTemplateDialog.xaml
    /// </summary>
    public partial class CreateTemplateDialog : Window
    {
        public CreateTemplateDialog()
        {
            InitializeComponent();
        }

        public ICredentialsTemplate GetCredentials()
        {
            return ToCredentials();
        }

        private ICredentialsTemplate ToCredentials()
        {
            return SelectedCredentialsType() == CredentialsType.Windows_Authentication ?
                new WindowsAuthenticationCredentials
                {
                    TemplateName = templateNameEdit.Text,
                    Server = serverEdit.Text,
                    Database = databaseEdit.Text,
                } :
                new SQLServerAuthenticationCredentials
                {
                    TemplateName = templateNameEdit.Text,
                    Server = serverEdit.Text,
                    Database = databaseEdit.Text,
                    Login = loginEdit.Text,
                    Password = passwordEdit.Text
                };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MinHeight = Height;
            MaxHeight = Height;
        }

        private void Accept(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Reject(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void UpdateCredentialsType(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Visibility visible = SelectedCredentialsType() == CredentialsType.Windows_Authentication ? 
                Visibility.Collapsed : 
                Visibility.Visible;
            loginTextBlock.Visibility = visible;
            loginEdit.Visibility = visible;
            passwordTextBlock.Visibility = visible;
            passwordEdit.Visibility = visible;
        }

        private CredentialsType SelectedCredentialsType()
        {
            return credentialsTypeComboBox.SelectedIndex == 0 ? 
                CredentialsType.Windows_Authentication :
                CredentialsType.SQL_Server_Authentication;
        }
    }
}

using System.IO;
using System.Windows;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Collections.Generic;
using System.Windows.Controls;

namespace DatabaseViewer
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        const string SettingsPath = "settings";
        const string LastUsedCredentialsPath = @$"{SettingsPath}\last_used.yml";
        const string CredentialsTemplatesPath = @$"{SettingsPath}\credentials_templates.yml";

        private readonly List<ICredentialsTemplate> m_availableTemplates;
        private bool m_ignoreTextChanged = false;
        private string m_templateName = string.Empty;

        public static string[] ReadAllLines(TextReader reader)
        {
            List<string> lines = new();
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }

            return lines.ToArray();
        }

        public LoginDialog()
        {
            InitializeComponent();
            if (File.Exists(LastUsedCredentialsPath))
            {
                using StreamReader reader = new(LastUsedCredentialsPath);
                LoadCredentials(GetDeserializer()
                                .Deserialize<ICredentialsTemplate>(reader.ReadToEnd()));
            }

            if (File.Exists(CredentialsTemplatesPath))
            {
                using StreamReader reader = new(CredentialsTemplatesPath);
                m_availableTemplates = GetDeserializer()
                                       .Deserialize<List<ICredentialsTemplate>>(reader.ReadToEnd());
            }
            else
            {
                m_availableTemplates = new();
            }                

            LoadAvailableTemplates();
        }

        public ICredentialsTemplate GetCredentials()
        {
            return ToCredentials(m_templateName);
        }

        private static IDeserializer GetDeserializer()
        {
            return new DeserializerBuilder()
                .WithTagMapping("!Windows_Authentication", typeof(WindowsAuthenticationCredentials))
                .WithTagMapping("!SQL_Server_Authentication", typeof(SQLServerAuthenticationCredentials))
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
        }

        private static ISerializer GetSerializer()
        {
            return new SerializerBuilder()
                .WithTagMapping("!Windows_Authentication", typeof(WindowsAuthenticationCredentials))
                .WithTagMapping("!SQL_Server_Authentication", typeof(SQLServerAuthenticationCredentials))
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
        }

        private void LoadAvailableTemplates()
        {
            loadTemplateAction.Items.Clear();
            foreach (ICredentialsTemplate? credentials in m_availableTemplates)
            {
                MenuItem menuItem = new();
                loadTemplateAction.Items.Add(menuItem);

                TextBlock menuHeader = new()
                {
                    Text = credentials.TemplateName
                };
                menuItem.Header = menuHeader;

                //menuItem.ToolTip = "your detailed tooltip here";
                menuItem.Click += (_, _) => LoadCredentials(credentials);
            }
        }

        private void LoadCredentials(ICredentialsTemplate credentials)
        {
            credentialsTypeComboBox.SelectedIndex = credentials.Type == CredentialsType.Windows_Authentication ? 0 : 1;
            UpdateTemplateName(credentials.TemplateName);

            m_ignoreTextChanged = true;
            serverEdit.Text = credentials.Server;
            databaseEdit.Text = credentials.Database;

            SQLServerAuthenticationCredentials? sqlCredentials = credentials as SQLServerAuthenticationCredentials?;
            if (sqlCredentials is not null)
            {
                loginEdit.Text = sqlCredentials?.Login;
                passwordEdit.Text = sqlCredentials?.Password;
            }

            m_ignoreTextChanged = false;
        }

        private void UpdateTemplateName(string templateName)
        {
            m_templateName = templateName;
            templateNameBlock.Text = "Шаблон: " + m_templateName;
        }

        private ICredentialsTemplate ToCredentials(string templateName)
        {
            return SelectedCredentialsType() == CredentialsType.Windows_Authentication ?
                new WindowsAuthenticationCredentials
                {
                    TemplateName = templateName,
                    Server = serverEdit.Text,
                    Database = databaseEdit.Text,
                } :
                new SQLServerAuthenticationCredentials
                {
                    TemplateName = templateName,
                    Server = serverEdit.Text,
                    Database = databaseEdit.Text,
                    Login = loginEdit.Text,
                    Password = passwordEdit.Text
                };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MinWidth = Width;
            //MinHeight = Height;
            //MaxHeight = Height;
        }

        private void AddTemplate(ICredentialsTemplate credentials)
        {
            m_availableTemplates.Add(credentials);
            LoadAvailableTemplates();
            UpdateTemplatesFile();
        }

        private static void WriteYaml(object? which, string where)
        {
            Directory.CreateDirectory(SettingsPath);
            string yaml = GetSerializer()
                          .Serialize(which);
            using StreamWriter writer = new(where);
            writer.Write(yaml);
        }

        private void UpdateSavedCredentials()
        {
            WriteYaml(GetCredentials(), LastUsedCredentialsPath);
        }

        private void UpdateTemplatesFile()
        {
            WriteYaml(m_availableTemplates, CredentialsTemplatesPath);
        }

        private void CreateTemplate(object sender, RoutedEventArgs e)
        {
            CreateTemplateDialog dialog = new();
            if (!dialog.ShowDialog().GetValueOrDefault(false))
            {
                return;
            }

            AddTemplate(dialog.GetCredentials());
        }

        private void SaveAsTemplate(object sender, RoutedEventArgs e)
        {
            SaveAsTemplateDialog dialog = new();
            if (!dialog.ShowDialog().GetValueOrDefault(false))
            {
                return;
            }

            AddTemplate(ToCredentials(dialog.GetTemplateName()));
        }

        private void Accept(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            UpdateSavedCredentials();
            Close();
        }

        private void Reject(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ResetTemplateName(object sender, TextChangedEventArgs e)
        {
            if (m_ignoreTextChanged)
            {
                return;
            }

            UpdateTemplateName(string.Empty);
        }

        private void UpdateCredentialsType(object sender, SelectionChangedEventArgs e)
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

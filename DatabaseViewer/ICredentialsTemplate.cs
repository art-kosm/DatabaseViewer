using YamlDotNet.Serialization;

namespace DatabaseViewer
{
    public interface ICredentialsTemplate
    {
        public string TemplateName { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string ConnectionString { get; }
        //public string[] Strings { get; }
        public CredentialsType Type { get; }
    }

    public enum CredentialsType
    {
        Windows_Authentication,
        SQL_Server_Authentication
    }

    public struct WindowsAuthenticationCredentials : ICredentialsTemplate
    {
        [YamlMember(Alias = "template name", ApplyNamingConventions = false)]
        public string TemplateName { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        [YamlIgnore]
        public readonly string ConnectionString => $@"Server={Server};
                                                      Database={Database};
                                                      Trusted_Connection=True;
                                                      TrustServerCertificate=True;
                                                      Connect Timeout=5";
        [YamlIgnore]
        public readonly CredentialsType Type => CredentialsType.Windows_Authentication;
    }

    public struct SQLServerAuthenticationCredentials : ICredentialsTemplate
    {
        [YamlMember(Alias = "template name", ApplyNamingConventions = false)]
        public string TemplateName { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        [YamlIgnore]
        public readonly string ConnectionString => $@"SERVER={Server};
                                                      DATABASE={Database};
                                                      User Id={Login};
                                                      PASSWORD={Password};
                                                      TrustServerCertificate=True;
                                                      Connect Timeout=5";
        [YamlIgnore]
        public readonly CredentialsType Type => CredentialsType.SQL_Server_Authentication;
    }
}

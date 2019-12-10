namespace Galaxy.Auth.Core.Models.Settings
{
    public class SmtpServerSettings
    {
        public string Host { get;set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
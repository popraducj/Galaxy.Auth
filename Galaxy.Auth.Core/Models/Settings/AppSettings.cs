namespace Galaxy.Auth.Core.Models.Settings
{
    public class AppSettings
    {
        public string EncryptionKey { get; set; }
        public SmtpServerSettings SmtpServer { get; set; }
        public string FromEmail { get; set; }
        public JwtSettings Jwt { get; set; }
    }
}
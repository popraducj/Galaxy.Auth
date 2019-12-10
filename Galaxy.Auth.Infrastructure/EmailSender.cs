using MailKit.Net.Smtp;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Interfaces;
using Galaxy.Auth.Core.Models.Settings;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Galaxy.Auth.Infrastructure
{
    public class EmailSender : IEmailSender
    {
        private readonly AppSettings _appSettings;

        public EmailSender(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public void SendEmail(string address, string body, string subject)
        {
            var message = new MimeMessage();
            message.To.Add(InternetAddress.Parse(address));
            message.From.Add(InternetAddress.Parse(_appSettings.FromEmail));
        
            message.Subject =subject;
            //We will say we are sending HTML. But there are options for plaintext etc. 
            message.Body = new TextPart("html")
            {
                Text = body
            };
        
            using (var emailClient = new SmtpClient())
            {
                //The last parameter here is to use SSL 
                emailClient.Connect(_appSettings.SmtpServer.Host, _appSettings.SmtpServer.Port, false);
        
                //Remove any OAuth functionality as we won't be using it. 
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
        
                emailClient.Authenticate(_appSettings.SmtpServer.Username, _appSettings.SmtpServer.Password);
                emailClient.Send(message);
                emailClient.Disconnect(true);
            }
        }
    }
}
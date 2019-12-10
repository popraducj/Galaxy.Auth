using System.Threading.Tasks;

namespace Galaxy.Auth.Core.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(string address, string body, string subject);
    }
}
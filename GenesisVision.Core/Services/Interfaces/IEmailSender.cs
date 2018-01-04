using System.Threading.Tasks;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface IEmailSender
    {
        void SendEmailAsync(string email, string subject, string textMessage, string htmlMessage);
    }
}

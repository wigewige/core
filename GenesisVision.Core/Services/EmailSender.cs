using GenesisVision.Core.Helpers;
using GenesisVision.Core.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace GenesisVision.Core.Services
{
    public class EmailSender : IEmailSender
    {
        public void SendEmailAsync(string email, string subject, string textMessage, string htmlMessage)
        {
            var client = new SendGridClient(Constants.SendGridApiKey);

            var from = new EmailAddress(Constants.SendGridFromEmail, Constants.SendGridFromName);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, textMessage, htmlMessage);

            Task.Run(() => client.SendEmailAsync(msg));
        }
    }
}

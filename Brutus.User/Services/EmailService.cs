using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace Brutus.User.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;

        public EmailService(EmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public async Task SendEmailAsync(string receiver, string subject, string content)
        {
            var mimeMessage = CreateMimeMessage(receiver, subject, content);

            using SmtpClient smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
            await smtpClient.AuthenticateAsync(_emailConfiguration.UserName, _emailConfiguration.Password);
            await smtpClient.SendAsync(mimeMessage);
            await smtpClient.DisconnectAsync(true);
        }

        private MimeMessage CreateMimeMessage(string receiver, string subject, string content)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(InternetAddress.Parse(_emailConfiguration.Sender));
            mimeMessage.To.Add(InternetAddress.Parse(receiver));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = content };
            return mimeMessage;
        }
    }
}
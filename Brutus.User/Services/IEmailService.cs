using System.Threading.Tasks;

namespace Brutus.User.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string receiver, string subject, string content);
    }
    
    public class EmailConfiguration 
    {
        public string Sender { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
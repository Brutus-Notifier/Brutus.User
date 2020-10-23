using System.Threading.Tasks;

namespace Brutus.User.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string body);
    }
}
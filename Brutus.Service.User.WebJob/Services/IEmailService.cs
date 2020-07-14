using System.Threading.Tasks;

namespace Brutus.Service.User.WebJob.Services
{
    public interface IEmailService
    {
        Task Send(string emailAdress, string message);
    }
}
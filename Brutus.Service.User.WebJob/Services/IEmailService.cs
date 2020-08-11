using System.Threading.Tasks;

namespace Brutus.Service.User.WebJob.Services
{
    public interface IEmailService
    {
        Task Send(string emailAdress, string message);
    }

    public class FakeEmailService : IEmailService
    {
        public Task Send(string emailAdress, string message) => Task.CompletedTask;
    }
}
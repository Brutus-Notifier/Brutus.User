using System;
using System.Threading.Tasks;

namespace Brutus.User.Services
{
    public class StubEmailService : IEmailService
    {
        public Task SendEmailAsync(string receiver, string subject, string content)
        {
            Console.WriteLine($"Email sent to: {receiver}. With body: {content}");
            return Task.CompletedTask;
        }
    }
}
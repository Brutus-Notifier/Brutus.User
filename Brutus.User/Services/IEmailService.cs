using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Brutus.User.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string body);
    }

    public class StubEmailService : IEmailService
    {
        private readonly ILogger<StubEmailService> _logger;

        public StubEmailService(ILogger<StubEmailService> logger)
        {
            _logger = logger;
        }
        public Task SendEmailAsync(string email, string body)
        {
            Console.WriteLine($"Email sent to: {email}. With body: {body}");
            // _logger.LogDebug($"Email sent to: {email}. With body: {body}");
            return Task.CompletedTask;
        }
    }
}
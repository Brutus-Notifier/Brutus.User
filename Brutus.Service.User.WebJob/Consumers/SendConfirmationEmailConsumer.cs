using System;
using System.Threading.Tasks;
using Brutus.Service.User.Contracts.Commands;
using Brutus.Service.User.Contracts.Events;
using Brutus.Service.User.WebJob.Services;
using MassTransit;

namespace Brutus.Service.User.WebJob.Consumers
{
    public class SendConfirmationEmailConsumer: IConsumer<SendUserConfirmationEmail>
    {
        private readonly IEmailService _emailService;

        public SendConfirmationEmailConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }
        
        public async Task Consume(ConsumeContext<SendUserConfirmationEmail> context)
        {
            Guid confirmationCode = Guid.NewGuid();
            var message = $"welcome! please use this code [{confirmationCode.ToString()}] to confirm your email!";
            
            await _emailService.Send(context.Message.Email, message);

            await context.Publish(new UserConfirmationEmailSent(context.Message.UserId, confirmationCode));
        }
    }
}
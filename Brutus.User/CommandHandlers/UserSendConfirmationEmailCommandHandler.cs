using System;
using System.Threading.Tasks;
using Brutus.Core;
using Brutus.User.Services;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class UserSendConfirmationEmailCommandHandler: ICommandHandler<Commands.V1.UserSendEmailConfirmation>
    {
        // private readonly IRepository<Domain.User> _repository;
        private readonly IEmailService _emailService;

        public UserSendConfirmationEmailCommandHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<Commands.V1.UserSendEmailConfirmation> context)
        {
            if(string.IsNullOrWhiteSpace(context.Message.Email))
                throw new ArgumentNullException(nameof(context.Message.Email));
            
            if(context.Message.UserId == Guid.Empty)
                throw new ArgumentNullException(nameof(context.Message.UserId));

            var body = $"Hello, please confirm your email by this link";
            await _emailService.SendEmailAsync(context.Message.Email, body);
            await context.Publish(new Events.V1.UserEmailConfirmationSent(context.Message.UserId, context.Message.Email));
        }
    }
}
using System.Threading.Tasks;
using Brutus.Core;
using DomainCommands = Brutus.User.Domain.Commands;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class UserChangeEmailCommandHandler:ICommandHandler<DomainCommands.V1.UserChangeEmail>
    {
        private readonly IRepository<Domain.User> _repository;
        public UserChangeEmailCommandHandler(IRepository<Domain.User> repository)
        {
            _repository = repository;
        }
        
        public async Task Consume(ConsumeContext<DomainCommands.V1.UserChangeEmail> context)
        {
            var user = await _repository.FindAsync(context.Message.UserId);
            user.ChangeEmail(context.Message.Email);
            var events = await _repository.UpdateAsync(user);

            foreach (var @event in events) await context.Publish(@event);
        }
    }
}
using System.Threading.Tasks;
using Brutus.Core;
using Brutus.User.Domain;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class ConfirmEmailCommandHandler:ICommandHandler<Commands.V1.ConfirmUserEmail>
    {
        private readonly IRepository<Domain.User> _repository;

        public ConfirmEmailCommandHandler(IRepository<Domain.User> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<Commands.V1.ConfirmUserEmail> context)
        {
            var user = await _repository.FindAsync(context.Message.UserId);
            user.ConfirmEmail(context.Message.Email);
            var events = await _repository.UpdateAsync(user);
            
            foreach (var @event in events) await context.Publish(@event);
        }
    }
}
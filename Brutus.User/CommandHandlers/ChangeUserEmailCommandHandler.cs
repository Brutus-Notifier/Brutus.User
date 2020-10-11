using System.Threading.Tasks;
using Brutus.Core;
using Brutus.User.Domain;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class ChangeUserEmailCommandHandler:ICommandHandler<Commands.V1.ChangeUserEmail>
    {
        private readonly IRepository<Domain.User> _repository;
        public ChangeUserEmailCommandHandler(IRepository<Domain.User> repository)
        {
            _repository = repository;
        }
        
        public async Task Consume(ConsumeContext<Commands.V1.ChangeUserEmail> context)
        {
            var user = await _repository.FindAsync(context.Message.UserId);
            user.ChangeEmail(context.Message.Email);
            var events = await _repository.UpdateAsync(user);

            foreach (var @event in events)
            {
                await context.Publish(@event);
            }
        }
    }
}
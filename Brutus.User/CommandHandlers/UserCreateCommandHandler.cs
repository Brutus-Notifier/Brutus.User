using System.Threading.Tasks;
using Brutus.Core;
using DomainCommands = Brutus.User.Domain.Commands;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class UserCreateCommandHandler: ICommandHandler<DomainCommands.V1.UserCreate>
    {
        private readonly IRepository<Domain.User> _repository;

        public UserCreateCommandHandler(IRepository<Domain.User> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<DomainCommands.V1.UserCreate> context)
        {
            var user = new Domain.User(context.Message.UserId, context.Message.FirstName, context.Message.LastName, context.Message.Email);
            var events = await _repository.AddAsync(user);
            
            foreach (var @event in events) await context.Publish(@event);
        }
    }
}
using System.Threading.Tasks;
using Brutus.Core;
using Brutus.User.Domain;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class CreateUserCommandHandler: ICommandHandler<Commands.V1.CreateUser>
    {
        private readonly IRepository<Domain.User> _repository;

        public CreateUserCommandHandler(IRepository<Domain.User> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<Commands.V1.CreateUser> context)
        {
            Domain.User user = new Domain.User(context.Message.UserId, context.Message.FirstName, context.Message.LastName, context.Message.Email);
            var events = await _repository.AddAsync(user);
            
            foreach (var @event in events)
            {
                await context.Publish(@event);
            }
        }
    }
}
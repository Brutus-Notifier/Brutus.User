using System.Threading.Tasks;
using Brutus.Core;
using Brutus.User.Domain;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class CreateUserCommandHandler: IConsumer<Commands.V1.CreateUser>
    {
        private readonly IRepository<Domain.User> _repository;
        public CreateUserCommandHandler(IRepository<Domain.User> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<Commands.V1.CreateUser> context)
        {
            // Domain.User user = new Domain.User(context.Message.UserId);
            // await _repository.Add(user);
        }
    }
}
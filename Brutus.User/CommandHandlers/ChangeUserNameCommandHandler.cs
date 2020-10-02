using System.Threading.Tasks;
using Brutus.Core;
using Brutus.User.Domain;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class ChangeUserNameCommandHandler: ICommandHandler<Commands.V1.ChangeUserName>
    {
        private readonly IRepository<Domain.User> _repository;
        public ChangeUserNameCommandHandler(IRepository<Domain.User> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<Commands.V1.ChangeUserName> context)
        {
            Domain.User aggregate = await _repository.Find(context.Message.UserId);
            aggregate.ChangeName(context.Message.FirstName, context.Message.LastName);
            await _repository.Update(aggregate);
        }
    }
}
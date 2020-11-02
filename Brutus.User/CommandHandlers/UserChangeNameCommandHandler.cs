using System.Threading.Tasks;
using Brutus.Core;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class UserChangeNameCommandHandler: ICommandHandler<Commands.V1.UserChangeName>
    {
        private readonly IAggregateRepository<Domain.User> _aggregateRepository;
        public UserChangeNameCommandHandler(IAggregateRepository<Domain.User> aggregateRepository)
        {
            _aggregateRepository = aggregateRepository;
        }

        public async Task Consume(ConsumeContext<Commands.V1.UserChangeName> context)
            => await this.HandleUpdate(context, _aggregateRepository, context.Message.UserId, x => x.ChangeName(context.Message.FirstName, context.Message.LastName));
    }
}
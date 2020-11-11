using System.Threading.Tasks;
using Brutus.Core;
using Marten;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class UserActivateCommandHandler:ICommandHandler<Commands.V1.FinishUserRegistration>
    {
        private readonly IAggregateRepository<Domain.User> _aggregateRepository;

        public UserActivateCommandHandler(IAggregateRepository<Domain.User> aggregateRepository)
        {
            _aggregateRepository = aggregateRepository;
        }

        public async Task Consume(ConsumeContext<Commands.V1.FinishUserRegistration> context)
        {
            await this.HandleUpdate(context, _aggregateRepository, context.Message.UserId, x => x.Activate());
            await context.RespondAsync(new Events.V1.RegistrationUserEmailConfirmed(context.Message.UserId));
        }
    }
}
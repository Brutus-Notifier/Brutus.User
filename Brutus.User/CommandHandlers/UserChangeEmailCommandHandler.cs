using System.Threading.Tasks;
using Brutus.Core;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class UserChangeEmailCommandHandler:ICommandHandler<Commands.V1.UserChangeEmail>
    {
        private readonly IAggregateRepository<Domain.User> _aggregateRepository;
        public UserChangeEmailCommandHandler(IAggregateRepository<Domain.User> aggregateRepository)
        {
            _aggregateRepository = aggregateRepository;
        }
        
        public async Task Consume(ConsumeContext<Commands.V1.UserChangeEmail> context) 
            => await this.HandleUpdate(context, _aggregateRepository, context.Message.UserId, x => x.ChangeEmail(context.Message.Email));
    }
}
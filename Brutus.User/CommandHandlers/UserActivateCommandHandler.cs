using System.Threading.Tasks;
using Brutus.Core;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class UserActivateCommandHandler:ICommandHandler<Commands.V1.FinishUserRegistration>
    {
        private readonly IRepository<Domain.User> _repository;

        public UserActivateCommandHandler(IRepository<Domain.User> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<Commands.V1.FinishUserRegistration> context)
        {
            await this.HandleUpdate(context, _repository, context.Message.UserId,
                x => x.Activate());
            await context.RespondAsync(new Events.V1.RegistrationUserFinished(context.Message.UserId));
        }
    }
}
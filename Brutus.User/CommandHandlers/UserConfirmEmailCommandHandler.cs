using System.Threading.Tasks;
using Brutus.Core;
using DomainCommands = Brutus.User.Domain.Commands;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class UserConfirmEmailCommandHandler:ICommandHandler<DomainCommands.V1.UserConfirmEmail>
    {
        private readonly IRepository<Domain.User> _repository;

        public UserConfirmEmailCommandHandler(IRepository<Domain.User> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<DomainCommands.V1.UserConfirmEmail> context)
            => await this.HandleUpdate(context, _repository, context.Message.UserId, x => x.ConfirmEmail(context.Message.Email));
    }
}
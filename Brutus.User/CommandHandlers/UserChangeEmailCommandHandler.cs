using System.Threading.Tasks;
using Brutus.Core;
using DomainCommands = Brutus.User.Domain.Commands;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class UserChangeEmailCommandHandler:ICommandHandler<DomainCommands.V1.UserChangeEmail>
    {
        private readonly IRepository<Domain.User> _repository;
        public UserChangeEmailCommandHandler(IRepository<Domain.User> repository)
        {
            _repository = repository;
        }
        
        public async Task Consume(ConsumeContext<DomainCommands.V1.UserChangeEmail> context) 
            => await this.HandleUpdate(context, _repository, context.Message.UserId, x => x.ChangeEmail(context.Message.Email));
    }
}
using System.Threading.Tasks;
using Brutus.Core;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class UserChangeNameCommandHandler: ICommandHandler<Commands.V1.UserChangeName>
    {
        private readonly IRepository<Domain.User> _repository;
        public UserChangeNameCommandHandler(IRepository<Domain.User> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<Commands.V1.UserChangeName> context)
            => await this.HandleUpdate(context, _repository, context.Message.UserId, x => x.ChangeName(context.Message.FirstName, context.Message.LastName));
    }
}
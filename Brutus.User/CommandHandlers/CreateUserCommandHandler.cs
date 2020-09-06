using System.Threading;
using System.Threading.Tasks;
using Brutus.Core;
using Brutus.User.Domain;
using MediatR;

namespace Brutus.User.CommandHandlers
{
    public class CreateUserCommandHandler: ICommandHandler<Commands.V1.CreateUser>
    {
        private IRepository<Domain.User> _repository;
        public CreateUserCommandHandler(IRepository<Domain.User> repository)
        {
            _repository = repository;
        }
        public async Task<Unit> Handle(Commands.V1.CreateUser request, CancellationToken cancellationToken)
        {
            Domain.User user = new Domain.User(request.UserId);
            
            await _repository.Add(user);
            return Unit.Value;
        }
    }
}
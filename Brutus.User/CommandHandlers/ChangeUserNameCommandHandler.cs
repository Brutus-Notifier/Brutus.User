using System.Threading;
using System.Threading.Tasks;
using Brutus.Core;
using Brutus.User.Domain;
using Marten;
using MediatR;

namespace Brutus.User.CommandHandlers
{
    public class ChangeUserNameCommandHandler: ICommandHandler<Commands.V1.ChangeUserName>
    {
        private IRepository<Domain.User> _repository;
        public ChangeUserNameCommandHandler(IRepository<Domain.User> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(Commands.V1.ChangeUserName request, CancellationToken cancellationToken)
        {
            Domain.User aggregate = await _repository.Find(request.UserId);
            aggregate.ChangeName(request.UserName);
            await _repository.Update(aggregate);
            return Unit.Value;
        }
    }
}
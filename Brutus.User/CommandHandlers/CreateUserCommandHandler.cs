using System;
using System.Threading;
using System.Threading.Tasks;
using Brutus.Core;
using Brutus.User.Domain;
using MediatR;

namespace Brutus.User.CommandHandlers
{
    public class CreateUserCommandHandler: ICommandHandler<Commands.V1.CreateUser>
    {
        public Task<Unit> Handle(Commands.V1.CreateUser request, CancellationToken cancellationToken)
        {
            Domain.User user = new Domain.User(request.UserId);
            Console.WriteLine("HANDLED");
            
            return Task.FromResult(Unit.Value);
        }
    }
}
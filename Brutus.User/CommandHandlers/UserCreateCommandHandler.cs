using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brutus.Core;
using Marten.Events;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class UserCreateCommandHandler: ICommandHandler<Commands.V1.UserCreate>
    {
        private readonly IRepository<Domain.User> _repository;

        public UserCreateCommandHandler(IRepository<Domain.User> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<Commands.V1.UserCreate> context)
        {
            var user = new Domain.User(context.Message.UserId, context.Message.FirstName, context.Message.LastName, context.Message.Email);
            IEnumerable<object> events;
            try
            {
                events = await _repository.AddAsync(user);
            }
            catch (ExistingStreamIdCollisionException)
            {
                throw new DomainException($"{nameof(User)} with Id {user.Id} already exists!");
            }
            
            await Task.WhenAll(events.Select(@event => context.Publish(@event)));

            await context.RespondAsync(new Events.V1.RegistrationUserStarted(user.Id));
        }
    }
}
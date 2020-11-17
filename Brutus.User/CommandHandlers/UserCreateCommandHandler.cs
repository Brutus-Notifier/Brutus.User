using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brutus.Core;
using Brutus.User.Sagas;
using Marten;
using Marten.Events;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class UserCreateCommandHandler: ICommandHandler<Commands.V1.UserCreate>
    {
        private readonly IAggregateRepository<Domain.User> _aggregateRepository;
        private readonly IDocumentSession _session;

        public UserCreateCommandHandler(IAggregateRepository<Domain.User> aggregateRepository, IDocumentSession session)
        {
            _aggregateRepository = aggregateRepository;
            _session = session;
        }

        public async Task Consume(ConsumeContext<Commands.V1.UserCreate> context)
        {
            if (_session.Query<UserRegistrationState>().Any(x => x.Email == context.Message.Email))
            {
                throw new Exceptions.DataValidationException(nameof(Domain.User), nameof(Commands.V1.UserCreate.Email), $"{nameof(User)} with email {context.Message.Email} already started registration!");
            }

            if (_session.Query<Domain.User>().Any(x => x.Email == context.Message.Email))
            {
                throw new Exceptions.DataValidationException(nameof(Domain.User), nameof(Commands.V1.UserCreate.Email), $"{nameof(User)} with email {context.Message.Email} already exists!");
            }
            
            var user = new Domain.User(context.Message.UserId, context.Message.FirstName, context.Message.LastName, context.Message.Email);
            IEnumerable<object> events;
            try
            {
                events = await _aggregateRepository.AddAsync(user);
            }
            catch (ExistingStreamIdCollisionException)
            {
                throw new Exceptions.DataValidationException(nameof(Domain.User), nameof(Commands.V1.UserCreate.UserId), $"{nameof(User)} with Id {user.Id} already exists!");
            }

            await Task.WhenAll(events.Select(@event => context.Publish(@event)));

            await context.RespondAsync(new Events.V1.RegistrationUserStarted(user.Id));
        }
    }
}
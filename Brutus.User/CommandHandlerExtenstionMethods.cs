using System;
using System.Linq;
using System.Threading.Tasks;
using Brutus.Core;
using MassTransit;

namespace Brutus.User
{
    public static class CommandHandlerExtenstionMethods
    {
        public static async Task HandleUpdate<T,Y>(this ICommandHandler<T> commandHandler, ConsumeContext<T> context, IAggregateRepository<Y> aggregateRepository, Guid aggregateId, Action<Y> operation) 
            where T: class, ICommand
            where Y: Aggregate
        {
            var aggregate = await aggregateRepository.FindAsync(aggregateId);
            if (aggregate == null)
                throw new InvalidOperationException($"{typeof(Y).Name} with Id ${aggregateId} not found");

            operation(aggregate);
            var events = await aggregateRepository.UpdateAsync(aggregate);

            await Task.WhenAll(events.Select(@event => context.Publish(@event)));
        }
    }
}
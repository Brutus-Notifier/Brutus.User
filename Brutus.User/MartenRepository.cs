using System;
using System.Threading;
using System.Threading.Tasks;
using Brutus.Core;
using Marten;

namespace Brutus.User
{
    public class MartenRepository<T> : IRepository<T> where T : class, IAggregate, new()
    {
        private readonly IDocumentSession _session;

        public MartenRepository(IDocumentSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }
        
        public async Task<T> Find(Guid id)
        {
            return await _session.Events.AggregateStreamAsync<T>(id, token: new CancellationToken());
        }

        public async Task Add(T aggregate)
        {
            _session.Events.StartStream<T>(aggregate.Id);
            var events = aggregate.DequeueEvents();
            _session.Events.Append(aggregate.Id, events);
            await _session.SaveChangesAsync();
        }

        public async Task Update(T aggregate)
        {
            _session.Events.Append(aggregate.Id, aggregate.DequeueEvents());
            await _session.SaveChangesAsync();
        }
    }
}
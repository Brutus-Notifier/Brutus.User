using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Brutus.Core;
using Marten;

namespace Brutus.User
{
    public class MartenAggregateRepository<T> : IAggregateRepository<T> where T : Aggregate
    {
        private readonly IDocumentSession _session;

        public MartenAggregateRepository(IDocumentSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }
        
        public async Task<T> FindAsync(Guid id)
        {
            //reason -> https://github.com/JasperFx/marten/issues/1532
            if (await _session.Events.FetchStreamStateAsync(id) == null) return null;
            
            return await _session.Events.AggregateStreamAsync<T>(id, token: new CancellationToken());
        }

        public async Task<ICollection<object>> AddAsync(T aggregate)
        {
            _session.Events.StartStream<T>(aggregate.Id);
            var events = aggregate.DequeueEvents();
            _session.Events.Append(aggregate.Id, events);
            await _session.SaveChangesAsync();

            return events;
        }

        public async Task<ICollection<object>> UpdateAsync(T aggregate)
        {
            var events = aggregate.DequeueEvents();
            _session.Events.Append(aggregate.Id, events);
            await _session.SaveChangesAsync();

            return events;
        }
    }
}
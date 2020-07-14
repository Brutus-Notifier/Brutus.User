using System.Collections.Generic;
using System.Linq;

namespace Brutus.Framework
{
    public abstract class AggregateRoot<TId> : IInternalEventHandler where TId : Value<TId>
    {
        public int Version { get; private set; } = -1;
        public TId Id { get; protected set; }
        private readonly List<object> _changes;

        protected AggregateRoot() => _changes = new List<object>();

        public IEnumerable<object> GetChanges() => _changes.AsEnumerable();
        public void CleanChanges() => _changes.Clear();

        public void Load(IEnumerable<object> @history)
        {
            foreach (var e in history)
            {
                When(e);
                Version++;
            }
        }

        protected abstract void EnsureValidState();
        protected abstract void When(object @event);

        protected void Apply(object @event)
        {
            When(@event);
            EnsureValidState();
            _changes.Add(@event);
        }

        protected void ApplyToEntity(IInternalEventHandler entity, object @event) => entity.Handle(@event);
        void IInternalEventHandler.Handle(object @event) => When(@event);
    }
}
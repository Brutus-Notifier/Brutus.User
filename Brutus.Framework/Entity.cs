using System;

namespace Brutus.Framework
{
    public abstract class Entity<T> : IInternalEventHandler where T : Value<T>
    {
        protected internal Entity(){}
        public T Id { get; protected set; }
        private readonly Action<object> _applier;
        protected Entity(Action<object> applier) => _applier = applier;

        protected void Apply(object @event)
        {
            When(@event);
            _applier(@event);
        }

        protected abstract void When(object @event);
        void IInternalEventHandler.Handle(object @event) => When(@event);
    }
}
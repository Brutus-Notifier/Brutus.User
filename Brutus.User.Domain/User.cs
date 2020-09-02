using System;
using System.Collections.Generic;
using System.Linq;
using Brutus.Core;

namespace Brutus.User.Domain
{
    public class User: IAggregate
    {
        private readonly IList<object> _events = new List<object>();

        public int Version { get; set; }
        public Guid Id { get; set; }
        public string Name { get; private set; }

        public User() { }
        
        public User(Guid id)
        {
            Events.V1.UserCreated @event = new Events.V1.UserCreated { UserId = id };
            Enqueue(@event);
            Apply(@event);
        }
        
        public ICollection<object> DequeueEvents()
        {
            var events = _events.ToList();
            _events.Clear();
            return events;
        }

        public void Enqueue(object @event)
        {
            _events.Add(@event);
        }

        public void ChangeName(string name)
        {
            Events.V1.UserNameChanged @event = new Events.V1.UserNameChanged {UserId = Id, UserName = name};
            Enqueue(@event);
            Apply(@event);
        }

        public void Apply(Events.V1.UserCreated @event)
        {
            Id = @event.UserId;
        }

        public void Apply(Events.V1.UserNameChanged @event)
        {
            Name = @event.UserName;
        }
    }
}
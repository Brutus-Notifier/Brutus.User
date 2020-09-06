using System;
using System.Collections.Generic;

namespace Brutus.Core
{
    public interface IAggregate
    {
        Guid Id { get; set;  }
        int Version { get; set; }
        ICollection<object> DequeueEvents();
        void Enqueue(object @event);
    }
}
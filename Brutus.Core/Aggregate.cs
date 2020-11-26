using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Brutus.Core
{
    public abstract class Aggregate: Entity
    {
        private readonly IList<object> _events = new List<object>();
        
        public ICollection<object> DequeueEvents()
        {
            var events = _events.ToList();
            _events.Clear();
            return events;
        }

        private void Enqueue(object @event)
        {
            _events.Add(@event);
        }

        protected void Process(object @event)
        {
            Type thisType = GetType();
            if (thisType == null) throw new NotSupportedException($"Current this type is null!");
            
            MethodInfo methodInfo = thisType.GetMethod("Apply",  BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new [] { @event.GetType()}, null );
            if (methodInfo == null) throw new NotSupportedException($"Missing handler for event {@event.GetType().Name}");
            
            try
            {
                methodInfo.Invoke(this, new[] { @event } );
            }
            catch(TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            Enqueue(@event);
        }
    }
}
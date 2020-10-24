using System;
using System.Threading.Tasks;
using Automatonymous;
using MassTransit.Testing;

namespace Brutus.User.Tests
{
    public abstract class TestBaseSaga<T,Y> : TestBaseConsumer 
        where T: class, SagaStateMachineInstance
        where Y: MassTransitStateMachine<T>, new ()
    {
        protected readonly StateMachineSagaTestHarness<T, Y> HarnessSaga;
        protected readonly Y StateMachine;
        protected readonly TimeSpan Timeout;

        protected TestBaseSaga()
        {
            Timeout = TimeSpan.FromSeconds(3);
            StateMachine = new Y();
            HarnessSaga = Harness.StateMachineSaga<T, Y>(StateMachine);
        }
        
        protected async Task<T> FindMachineInstance(Guid correlationId, State state)
        {
            var instanceId = await HarnessSaga.Exists(correlationId, state);
            return instanceId.HasValue ? HarnessSaga.Sagas.Contains(instanceId.Value) : null;
        }

        protected async Task Publish(object @event, TimeSpan? delay = null)
        {
            await Harness.Bus.Publish(@event);
            if (delay.HasValue) await Task.Delay(delay.Value.Milliseconds);
        }
    }
}
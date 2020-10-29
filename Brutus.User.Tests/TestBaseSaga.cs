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
        protected readonly TimeSpan Delay;

        protected TestBaseSaga()
        {
            Timeout = TimeSpan.FromMilliseconds(500);
            Delay = TimeSpan.FromMilliseconds(100);
            StateMachine = new Y();
            HarnessSaga = Harness.StateMachineSaga<T, Y>(StateMachine);
        }
        
        protected async Task<T> FindMachineInstance(Guid correlationId, State state, TimeSpan? waitTimeout = null)
        {
            var instanceId = await HarnessSaga.Exists(correlationId, state, waitTimeout ?? Timeout);
            if (instanceId.HasValue) return HarnessSaga.Sagas.Contains(instanceId.Value);
            
            return HarnessSaga.Created.ContainsInState(correlationId, StateMachine, state);
        }

        protected async Task Publish(object @event, TimeSpan? delay = null)
        {
            await Harness.Bus.Publish(@event);
            await Task.Delay(delay?.Milliseconds ?? Delay.Milliseconds);
        }
    }
}
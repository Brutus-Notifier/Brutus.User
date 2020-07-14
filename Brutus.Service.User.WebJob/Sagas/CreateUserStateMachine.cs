using System;
using Automatonymous;
using Brutus.Service.User.Contracts.Events;
using Brutus.Service.User.Domain.Events;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using MassTransit.Saga;

namespace Brutus.Service.User.WebJob.Sagas
{
    public class CreateUserStateMachine : MassTransitStateMachine<CreateUserState>
    {
        public State Submitted { get; private set; }
        public State ConfirmationEmailSent { get; private set; }
        public State Finished { get; private set; }
        
        public Event<UserCreated> UserCreatedEvent { get; private set; }
        public Event<UserActivated> UserActivatedEvent { get; private set; }
        public Event<UserConfirmationEmailSent> UserConfirmationEmailSentEvent { get; private set; }
        public Event<UserEmailConfirmed> UserEmailConfirmedEvent { get; private set; }

        public CreateUserStateMachine()
        {
            Event(() => UserCreatedEvent, x=> x.CorrelateById(m => m.Message.UserId));
            Event(() => UserConfirmationEmailSentEvent, x => x.CorrelateById(m => m.Message.UserId));
            Event(() => UserActivatedEvent, x => x.CorrelateById(m=> m.Message.UserId));
            InstanceState( x => x.CurrentState);
            
            Initially(When(UserCreatedEvent)
                .TransitionTo(Submitted));
            
            During(Submitted, 
                Ignore(UserCreatedEvent),
                When(UserConfirmationEmailSentEvent)
                    .Then(context =>
                    {
                        context.Instance.EmailConfirmationCode = context.Data.ConfirmationCode;
                    })
                    .TransitionTo(ConfirmationEmailSent));
            
            During(ConfirmationEmailSent,
                Ignore(UserCreatedEvent),
                Ignore(UserConfirmationEmailSentEvent),
                When(UserEmailConfirmedEvent)
                    .Then(context =>
                    {
                        // TODO: Get the code from the state
                        // and compare with the code from the message
                        // if OK, then send the ActivateUser command
                        // and wait for the response which will proxy further as reponse
                        // otherwise - send confirmation failed event as response
                    }));
            
            During(Finished,
                Ignore(UserCreatedEvent),
                Ignore(UserConfirmationEmailSentEvent));
        }
    }

    public class CreateUserState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public Guid EmailConfirmationCode { get; set; }
        
        public int Version { get; set; }
    }

    public class RegisterUserStateMachineDefinition : SagaDefinition<CreateUserState>
    {
        public RegisterUserStateMachineDefinition()
        {
            ConcurrentMessageLimit = 16;
        }
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<CreateUserState> sagaConfigurator)
        {
            endpointConfigurator.UseMessageRetry(x=> x.Intervals(500, 5000, 10000));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
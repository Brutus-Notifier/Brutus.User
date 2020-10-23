using System;
using Automatonymous;
using MassTransit;
using DomainEvents = Brutus.User.Domain.Events;
using ServiceEvents = Brutus.User.Events;

namespace Brutus.User.Sagas
{
    public class UserRegistrationState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        
        public string Email { get; set; }
    }
        
    public class UserRegistrationSaga : MassTransitStateMachine<UserRegistrationState>
    {
        public State Submitted { get; private set; }
        public State ConfirmationSent { get; private set; }
        
        public Event<DomainEvents.V1.UserCreated> UserCreated { get; private set; }
        public Event<ServiceEvents.V1.UserEmailConfirmationSent> EmailConfirmationSent { get; private set; }
        public Event<DomainEvents.V1.UserEmailConfirmed> EmailConfirmed { get; private set; }
        
        public UserRegistrationSaga()
        {
            InstanceState(x => x.CurrentState, Submitted, ConfirmationSent);
            
            Event(() => UserCreated, x => x.CorrelateById(context => context.Message.UserId));
            Event(() => EmailConfirmationSent, x => x.CorrelateById(context => context.Message.UserId));
            Event(() => EmailConfirmed, x => x.CorrelateById(context => context.Message.UserId));
            
            Initially(
                When(UserCreated)
                    .PublishAsync(context => context.Init<Commands.V1.UserSendEmailConfirmation>(new
                    {
                        UserId = context.Instance.CorrelationId, 
                        Email = context.Data.Email
                    }))
                    .TransitionTo(Submitted));

            During(Submitted,
                When(EmailConfirmationSent)
                    .Then(x => x.Instance.Email = x.Data.Email)
                    .TransitionTo(ConfirmationSent));

            During(ConfirmationSent,
                When(EmailConfirmed)
                    .TransitionTo(Final));
        }
    }
}
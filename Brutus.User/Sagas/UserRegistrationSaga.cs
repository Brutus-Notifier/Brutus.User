using System;
using Automatonymous;
using Marten.Schema;
using DomainEvents = Brutus.User.Domain.Events;
using ServiceEvents = Brutus.User.Events;

namespace Brutus.User.Sagas
{
    [UseOptimisticConcurrency]
    public class UserRegistrationState : SagaStateMachineInstance
    {
        [Identity]
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public string Email { get; set; }
    }
        
    public class UserRegistrationSaga : MassTransitStateMachine<UserRegistrationState>
    {
        public State Submitted { get; private set; }
        public State InvitationCreated { get; private set; }
        public State ConfirmationSent { get; private set; }
        
        public Event<DomainEvents.V1.UserCreated> UserCreated { get; private set; }
        public Event<ServiceEvents.V1.UserEmailConfirmationSent> EmailConfirmationSent { get; private set; }
        public Event<DomainEvents.V1.UserActivated> UserActivated { get; private set; }
        public Event<ServiceEvents.V1.UserInvitationCreated> UserInvitationCreated { get; private set; }
        
        public UserRegistrationSaga()
        {
            InstanceState(x => x.CurrentState, Submitted, InvitationCreated, ConfirmationSent);
            
            Event(() => UserCreated, x => x.CorrelateById(context => context.Message.UserId));
            Event(() => EmailConfirmationSent, x => x.CorrelateById(context => context.Message.UserId));
            Event(() => UserActivated, x => x.CorrelateById(context => context.Message.UserId));
            Event(() => UserInvitationCreated, x => x.CorrelateById(context => context.Message.UserId));

            Initially(
                When(UserCreated)
                    .Then(x =>
                    {
                        x.Instance.Email = x.Data.Email;
                    })
                    .TransitionTo(Submitted)
            );
            
            WhenEnter(Submitted, 
                context => context.Then(cx => cx.Publish(new Commands.V1.CreateUserInvitation
                {
                    UserId = cx.Instance.CorrelationId,
                    Email = cx.Instance.Email
                }))
            );

            During(Submitted,
                When(UserInvitationCreated)
                    .Publish(cx => new Commands.V1.UserSendEmailConfirmation
                    {
                        UserId = cx.Instance.CorrelationId,
                        Email = cx.Instance.Email,
                        InvitationId = cx.Data.InvitationId
                    })
                    .TransitionTo(InvitationCreated));

            During(InvitationCreated, When(EmailConfirmationSent).TransitionTo(ConfirmationSent));
            During(ConfirmationSent, When(UserActivated).Finalize());
            
            SetCompletedWhenFinalized();
        }
    }
}
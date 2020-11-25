using System;
using Automatonymous;
using Brutus.User.Exceptions;
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
        public Guid InvitationId { get; set; }
    }
        
    public class UserRegistrationSaga : MassTransitStateMachine<UserRegistrationState>
    {
        public State Submitted { get; private set; }
        public State ConfirmationSent { get; private set; }
        
        public Event<DomainEvents.V1.UserCreated> UserCreated { get; private set; }
        public Event<ServiceEvents.V1.UserEmailConfirmationSent> EmailConfirmationSent { get; private set; }
        public Event<DomainEvents.V1.UserActivated> UserActivated { get; private set; }
        public Event<ServiceEvents.V1.UserInvitationCreated> UserInvitationCreated { get; private set; }
        
        public Event<Commands.V1.ConfirmUserRegistrationEmail> ConfirmUserRegistrationEmail { get; private set; }
        
        public UserRegistrationSaga()
        {
            InstanceState(x => x.CurrentState, Submitted, ConfirmationSent);
            
            Event(() => UserCreated, x => x.CorrelateById(context => context.Message.UserId));
            Event(() => UserInvitationCreated, x => x.CorrelateById(context => context.Message.UserId));
            Event(() => EmailConfirmationSent, x => x.CorrelateById(context => context.Message.UserId));
            Event(() => UserActivated, x => x.CorrelateById(context => context.Message.UserId));
            Event(() => ConfirmUserRegistrationEmail, x =>
            {
                x.CorrelateBy<Guid>(regSaga => regSaga.InvitationId, context => context.Message.InvitationId);
                x.OnMissingInstance(m =>
                    m.ExecuteAsync(x => throw new DataValidationException(nameof(Domain.User), "InvitationId", "Incorrect InvitationId"))
                );
            });
            // return x.RespondAsync<Fault<Commands.V1.ConfirmUserRegistrationEmail>>(new {x.OrderId});

            Initially(
                When(UserCreated)
                    .Then(x =>
                    {
                        x.Instance.Email = x.Data.Email;
                        x.Instance.InvitationId = Guid.NewGuid();
                    })
                    .TransitionTo(Submitted)
            );
            
            WhenEnter(Submitted, 
                context => context.Then(cx => cx.Publish(new Commands.V1.UserSendEmailConfirmation
                {
                    UserId = cx.Instance.CorrelationId,
                    Email = cx.Instance.Email,
                    InvitationId = cx.Instance.InvitationId
                }))
            );

            During(Submitted, When(EmailConfirmationSent).TransitionTo(ConfirmationSent));
            
            During(ConfirmationSent,
                When(ConfirmUserRegistrationEmail)
                    .Publish(ctx => new Commands.V1.ActivateUser { UserId =  ctx.Instance.CorrelationId })
                    .Respond(ctx => new Events.V1.RegistrationUserEmailConfirmed(ctx.Instance.CorrelationId)),
                When(UserActivated)
                    .Finalize());
            
            SetCompletedWhenFinalized();
        }
    }
}
using System;
using System.Text.RegularExpressions;
using Automatonymous;
using Brutus.Core;

namespace Brutus.User.Domain
{
    public class User: Aggregate
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Status { get; private set; }

        private User() { }
        
        public User(Guid id, string firstName, string lastName, string email)
        {
            Apply(new Events.V1.UserCreated(userId:id, firstName: firstName, lastName: lastName, email: email ));
        }

        public void ChangeName(string firstName, string lastName)
        {
            Apply(new Events.V1.UserNameChanged(userId: Id, firstName: firstName, lastName: lastName));
        }
        
        public void ChangeEmail(string userEmail)
        {
            Apply(new Events.V1.UserEmailChanged(userId: this.Id, email: userEmail));
        }
        
        public void ConfirmEmail(string email)
        {
            Apply(new Events.V1.UserEmailConfirmed(userId: this.Id, email));
        }
        
        public void Activate()
        {
            Apply(new Events.V1.UserActivated(Id, Email, FirstName, LastName));
        }

        #region When
        private void When(Events.V1.UserCreated @event)
        {
            Id = @event.UserId;
            When(new Events.V1.UserNameChanged(userId: this.Id, firstName:  @event.FirstName, lastName: @event.LastName));
            When(new Events.V1.UserEmailChanged(userId: this.Id, email: @event.Email));
            Status = UserStatus.Pending;
        }
        
        private void When(Events.V1.UserNameChanged @event)
        {
            CheckNullOrEmpty(@event.FirstName, nameof(@event.FirstName));
            CheckMaxLength(100, @event.FirstName, nameof(@event.FirstName));
            FirstName = @event.FirstName.Trim();
            
            CheckNullOrEmpty(@event.LastName, nameof(@event.LastName));
            CheckMaxLength(100, @event.LastName, nameof(@event.LastName));
            LastName = @event.LastName.Trim();
        }

        private void When(Events.V1.UserEmailChanged @event)
        {
            CheckNullOrEmpty(@event.Email, nameof(@event.Email));
            CheckMaxLength(50, @event.Email, nameof(@event.Email));
            var trimmedEmail = @event.Email.Trim();
            
            if(!Regex.IsMatch(trimmedEmail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                throw new ArgumentException($"Email {trimmedEmail} is invalid");

            Email = trimmedEmail;
        }
        
        private void When(Events.V1.UserEmailConfirmed @event)
        {
            CheckNullOrEmpty(@event.Email, nameof(@event.Email));

            if(Email != @event.Email)
                throw new ArgumentException($"Incorrect Email. User doesn't have an email {@event.Email}");

            if (Status == UserStatus.Active)
                throw new AggregateException($"User already has confirmed {@event.Email} email");

            Status = UserStatus.Active;
        }

        private void When(Events.V1.UserActivated @event)
        {
            if (Status == UserStatus.Active)
                throw new AggregateException("User could not be activated as it is already in Active status");

            Status = UserStatus.Active;
        }
        #endregion

        public static class UserStatus
        {
            public const string Pending = "Pending";
            public const string Active = "Active";
        }
    }
}
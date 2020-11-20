using System;
using Brutus.Core;

namespace Brutus.User.Domain
{
    public class User: Aggregate
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        
        public string Password { get; private set; }
        public string Status { get; private set; }

        private User() { }
        
        public User(Guid id, string password, string email) => Process(new Events.V1.UserCreated(userId:id, password: password, email: email ));
        public void ChangeName(string firstName, string lastName) => Process(new Events.V1.UserNameChanged(userId: Id, firstName: firstName, lastName: lastName));
        public void ChangeEmail(string userEmail) => Process(new Events.V1.UserEmailChanged(userId: Id, email: userEmail));
        public void ConfirmEmail(string email) => Process(new Events.V1.UserEmailConfirmed(userId: this.Id, email));
        public void Activate() => Process(new Events.V1.UserActivated(Id));

        #region Applies
        private void Apply(Events.V1.UserCreated @event)
        {
            Id = @event.UserId;
            
            SetUserPassword(@event.Password);
            SetUserEmail(@event.Email);
            Status = UserStatus.Pending;
        }

        private void Apply(Events.V1.UserEmailChanged @event) => SetUserEmail(@event.Email);

        private void Apply(Events.V1.UserNameChanged @event)
        {
            CheckNullOrEmpty(@event.FirstName, nameof(@event.FirstName));
            CheckMaxLength(100, @event.FirstName, nameof(@event.FirstName));
            FirstName = @event.FirstName.Trim();

            CheckNullOrEmpty(@event.LastName, nameof(@event.LastName));
            CheckMaxLength(100, @event.LastName, nameof(@event.LastName));
            LastName = @event.LastName.Trim();
        }
        
        private void SetUserEmail(string email)
        {
            email = email?.Trim();

            CheckNullOrEmpty(email, "Email");
            CheckMaxLength(50, email, "Email");
            CheckIsMatch(Constants.EmailTemplate, email, "Email");

            Email = email;
        }
        
        private void SetUserPassword(string password)
        {
            CheckNullOrEmpty(password, "Password");
            CheckIsMatch(Constants.PasswordTemplate, password, "Password");

            Password = password;
        }

        private void Apply(Events.V1.UserEmailConfirmed @event)
        {
            CheckNullOrEmpty(@event.Email, nameof(@event.Email));

            if(Email != @event.Email)
                throw new DomainException(nameof(User), $"Incorrect Email. User doesn't have an email {@event.Email}");

            if (Status == UserStatus.Active)
                throw new DomainException(nameof(User), $"User already has confirmed {@event.Email} email");

            Status = UserStatus.Active;
        }

        private void Apply(Events.V1.UserActivated @event)
        {
            if (Status == UserStatus.Active)
                throw new DomainException(nameof(User), "User could not be activated as it is already in Active status");

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
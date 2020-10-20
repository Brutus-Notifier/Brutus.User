using System;
using System.Text.RegularExpressions;
using Brutus.Core;

namespace Brutus.User.Domain
{
    public class User: Aggregate
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Status { get; set; }

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

        #region When
        private void When(Events.V1.UserCreated @event)
        {
            Id = @event.UserId;
            When(new Events.V1.UserNameChanged(userId: this.Id, firstName:  @event.FirstName, lastName: @event.LastName));
            When(new Events.V1.UserEmailChanged(userId: this.Id, email: @event.Email));
            Status = UserStatus.Awaiting;
        }
        
        private void When(Events.V1.UserNameChanged @event)
        {
            if (string.IsNullOrWhiteSpace(@event.FirstName))
                throw new ArgumentException($"{nameof(@event.FirstName)} could not be null or empty");
            if (@event.FirstName.Length > 100)
                throw new ArgumentException($"{nameof(@event.FirstName)} could not be longer the 100 characters");
            
            FirstName = @event.FirstName.Trim();
            
            if (string.IsNullOrWhiteSpace(@event.LastName))
                throw new ArgumentException($"{nameof(@event.LastName)} could not be null or empty");
            if (@event.LastName.Length > 100)
                throw new ArgumentException($"{nameof(@event.LastName)} could not be longer the 100 characters");
            LastName = @event.LastName.Trim();
        }

        private void When(Events.V1.UserEmailChanged @event)
        {
            if (string.IsNullOrWhiteSpace(@event.Email))
                throw  new ArgumentException($"{nameof(@event.Email)} could not be null or empty");
            if (@event.Email.Length > 50)
                throw new ArgumentException($"{nameof(@event.Email)} could not be longer then 50 characters");

            var trimmedEmail = @event.Email.Trim();
            
            if(!Regex.IsMatch(trimmedEmail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                throw new ArgumentException($"Email {trimmedEmail} is invalid");

            Email = trimmedEmail;
        }
        #endregion

        public static class UserStatus
        {
            public const string Awaiting = "AWAITING";
        }
    }
}
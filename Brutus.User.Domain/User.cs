using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Brutus.Core;

namespace Brutus.User.Domain
{
    public class User: IAggregate
    {
        private readonly IList<object> _events = new List<object>();

        public int Version { get; set; }
        public Guid Id { get; set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }

        public string Status { get; set; }

        public User() { }
        
        public User(Guid id, string firstName, string lastName, string email)
        {
            Apply(new Events.V1.UserCreated { UserId = id, FirstName = firstName, LastName = lastName, Email = email });
        }
        
        public ICollection<object> DequeueEvents()
        {
            var events = _events.ToList();
            _events.Clear();
            return events;
        }

        public void Enqueue(object @event)
        {
            _events.Add(@event);
        }

        public void ChangeName(string firstName, string lastName)
        {
            Apply(new Events.V1.UserNameChanged {UserId = Id, FirstName = firstName, LastName = lastName});
        }

        public void Apply(object @event)
        {
            switch (@event)
            {
                case Events.V1.UserCreated e: When(e); break;
                case Events.V1.UserNameChanged e: When(e); break;
                case Events.V1.UserEmailChanged e:  When(e); break;
                default: throw new NotSupportedException($"Event {@event} is not supported by User");
            }
            Enqueue(@event);
        }

        private void When(Events.V1.UserCreated @event)
        {
            Id = @event.UserId;
            When(new Events.V1.UserNameChanged{UserId = this.Id, FirstName =  @event.FirstName, LastName = @event.LastName});
            When(new Events.V1.UserEmailChanged(){UserId = this.Id, Email = @event.Email});
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

        public static class UserStatus
        {
            public const string Awaiting = "AWAITING";
        }

        public void ChangeEmail(string userEmail)
        {
            Apply(new Events.V1.UserEmailChanged(){ UserId = this.Id, Email = userEmail});
        }
    }
}
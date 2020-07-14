using System;
using System.IO;
using Brutus.Framework;
using Brutus.Service.User.Domain.Events;

namespace Brutus.Service.User.Domain
{
    public class User : AggregateRoot<UserId>
    {
        public UserEmail Email { get; private set; }
        public UserPassword Password { get; private set; }
        public UserStateEnum State { get; private set; }

        public User(UserId id, UserEmail email, UserPassword password)
        {
            Apply(new UserCreated(id, email, password));
        }

        public void Activate()
        {
            if (State != UserStateEnum.Pending)
                throw new InvalidOperationException($"Only users in 'Pending' state could be activated!");
            
            Apply(new UserActivated(Id));
        }
        
        protected override void When(object @event)
        {
            switch (@event)
            {
                case UserCreated e:
                    Email  = new UserEmail(e.Email);
                    Id = new UserId(e.UserId);
                    Password = new UserPassword(e.PasswordHash);
                    State = UserStateEnum.Pending;
                    break;
                
                case UserActivated e:
                    State = UserStateEnum.Active;
                    break;
            }
        }
        
        protected override void EnsureValidState()
        {
            var valid = Id != null;

            switch (State)
            {
                case UserStateEnum.Active: 
                    valid = Email != null 
                            && Password != null 
                            && valid; 
                    break;
            }
            
            if (!valid)
                throw new InvalidDataException($"Post-checks failed in state ${ Enum.GetName(typeof(UserStateEnum), State)}");
        }
    }

    public enum UserStateEnum
    {
        Pending = 1,
        Active = 2,
        Blocked = 3,
        Removed = 4
    }
}
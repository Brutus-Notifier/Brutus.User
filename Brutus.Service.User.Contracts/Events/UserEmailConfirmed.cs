using System;

namespace Brutus.Service.User.Contracts.Events
{
    public class UserEmailConfirmed
    {
        public Guid UserId { get; private set; }

        public UserEmailConfirmed(Guid userId)
        {
            UserId = userId;
        }
    }
}
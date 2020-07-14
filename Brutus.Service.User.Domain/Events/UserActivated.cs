using System;

namespace Brutus.Service.User.Domain.Events
{
    public class UserActivated
    {
        public Guid UserId { get; private set; }

        public UserActivated(Guid userId)
        {
            UserId = userId;
        }
    }
}
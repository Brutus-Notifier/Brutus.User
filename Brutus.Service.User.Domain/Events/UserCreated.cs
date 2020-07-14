using System;

namespace Brutus.Service.User.Domain.Events
{
    public class UserCreated
    {
        public Guid UserId { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }

        public UserCreated(Guid userId, string email, string passwordHash)
        {
            UserId = userId;
            Email = email;
            PasswordHash = passwordHash;
        }
    }
}
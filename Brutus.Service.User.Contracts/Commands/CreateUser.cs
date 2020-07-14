using System;

namespace Brutus.Service.User.Contracts.Commands
{
    public class CreateUser
    {
        public CreateUser(Guid userId, string email, string password)
        {
            UserId = userId;
            Email = email;
            Password = password;
        }

        public Guid UserId { get; }
        public string Email { get; }
        public string Password { get; }
    }
}
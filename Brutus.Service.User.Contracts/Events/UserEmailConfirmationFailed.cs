using System;

namespace Brutus.Service.User.Contracts.Events
{
    public class UserEmailConfirmationFailed
    {
        public Guid UserId { get; private set; }
        public string Message { get; private set; }

        public UserEmailConfirmationFailed(Guid userId, string message)
        {
            UserId = userId;
            Message = message;
        }
    }
}
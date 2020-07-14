using System;

namespace Brutus.Service.User.Contracts.Events
{
    public class UserConfirmationEmailSent
    {
        public Guid UserId { get; private set; }
        public Guid ConfirmationCode { get; private set; }

        public UserConfirmationEmailSent(Guid userId, Guid confirmationCode)
        {
            UserId = userId;
            ConfirmationCode = confirmationCode;
        }
    }
}
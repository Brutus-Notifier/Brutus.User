using System;

namespace Brutus.Service.User.Contracts.Commands
{
    public class ConfirmUserEmail
    {
        public Guid UserId { get; private set; }
        public Guid ConfirmationCode { get; private set; }

        public ConfirmUserEmail(Guid userId, Guid confirmationCode)
        {
            UserId = userId;
            ConfirmationCode = confirmationCode;
        }
    }
}
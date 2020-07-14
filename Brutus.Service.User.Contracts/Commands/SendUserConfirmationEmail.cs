using System;

namespace Brutus.Service.User.Contracts.Commands
{
    public class SendUserConfirmationEmail
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
    }
}
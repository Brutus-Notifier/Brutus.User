using System;
using Brutus.Core;

namespace Brutus.User
{
    public static class Commands
    {
        public static class V1
        {
            public class UserSendEmailConfirmation: ICommand
            {
                public Guid UserId { get; set; }
                public string Email { get; set; }
            }
        }
    }
}
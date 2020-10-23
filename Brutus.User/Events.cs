using System;

namespace Brutus.User
{
    public static class Events
    {
        public static class V1
        {
            public class UserEmailConfirmationSent
            {
                public UserEmailConfirmationSent(Guid userId, string email)
                {
                    UserId = userId;
                    Email = email;
                }

                public Guid UserId { get; }
                public string Email { get; }
            }
        }
    }
}
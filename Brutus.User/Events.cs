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

            public class RegistrationUserStarted
            {
                public RegistrationUserStarted(Guid userId)
                {
                    UserId = userId;
                }

                public Guid UserId { get; private set; }
            }
            
            public class RegistrationUserFinished
            {
                public RegistrationUserFinished(Guid userId)
                {
                    UserId = userId;
                }

                public Guid UserId { get; private set; }
            }
        }
    }
}
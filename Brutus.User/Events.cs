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

                public Guid UserId { get; }
            }
            
            public class RegistrationUserEmailConfirmed
            {
                public RegistrationUserEmailConfirmed(Guid userId)
                {
                    UserId = userId;
                }

                public Guid UserId { get; }
            }

            public class RegistrationUserFinished
            {
                public RegistrationUserFinished(Guid userId, string email, string firstName, string lastName)
                {
                    UserId = userId;
                    FirstName = firstName;
                    LastName = lastName;
                    Email = email;
                }
                
                public Guid UserId { get; }
                public string Email { get; }
                public string FirstName { get; }
                public string LastName { get; }
            }
        }
    }
}
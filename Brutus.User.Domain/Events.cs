using System;

namespace Brutus.User.Domain
{
    public static class Events
    {
        public static class V1
        {
            public class UserCreated
            {
                public UserCreated(Guid userId, string password, string email)
                {
                    UserId = userId;
                    Password = password;
                    Email = email;
                }

                public Guid UserId { get; }
                public string Password { get; }
                public string Email { get; }
            }

            public class UserNameChanged
            {
                public UserNameChanged(Guid userId, string firstName, string lastName)
                {
                    UserId = userId;
                    FirstName = firstName;
                    LastName = lastName;
                }

                public Guid UserId { get; }
                public string FirstName { get; }
                public string LastName { get; }
            }

            public class UserEmailChanged
            {
                public UserEmailChanged(Guid userId, string email)
                {
                    UserId = userId;
                    Email = email;
                }

                public Guid UserId { get; }
                public string Email { get;}
            }

            public class UserEmailConfirmed
            {
                public UserEmailConfirmed(Guid userId, string email)
                {
                    UserId = userId;
                    Email = email;
                }
                
                public Guid UserId { get; }
                public string Email { get; }
            }
            
            public class UserActivated
            {
                public UserActivated(Guid userId)
                {
                    UserId = userId;
                }

                public Guid UserId { get; }
            }
        }
    }
}
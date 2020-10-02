using System;

namespace Brutus.User.Domain
{
    public static class Events
    {
        public static class V1
        {
            public class UserCreated
            {
                public Guid UserId { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public string Email { get; set; }
            }

            public class UserNameChanged
            {
                public Guid UserId { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
            }

            public class UserEmailChanged
            {
                public Guid UserId { get; set; }
                public string Email { get; set; }
            }
        }
    }
}
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
            }

            public class UserNameChanged
            {
                public Guid UserId { get; set; }
                public string UserName { get; set; }
            }
        }
    }
}
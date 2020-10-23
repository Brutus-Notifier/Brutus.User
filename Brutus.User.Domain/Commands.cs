using System;
using Brutus.Core;

namespace Brutus.User.Domain
{
    public static class Commands
    {
        public static class V1
        {
            public class UserChangeName: ICommand
            {
                public Guid UserId { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
            }
            
            public class UserCreate : ICommand
            {
                public Guid UserId { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public string Email { get; set; }
            }
            
            public class UserChangeEmail : ICommand
            {
                public Guid UserId { get; set; }
                public string Email { get; set; }
            }

            public class UserConfirmEmail : ICommand
            {
                public Guid UserId { get; set; }
                public string Email { get; set; }
            }
        }
    }
}
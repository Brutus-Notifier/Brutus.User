using System;
using Brutus.Core;

namespace Brutus.User.Domain
{
    public static class Commands
    {
        public static class V1
        {
            public class ChangeUserName: ICommand
            {
                public Guid UserId { get; set; }
                public string UserName { get; set; }
            }
            
            public class CreateUser : ICommand
            {
                public Guid UserId { get; set; }
            }
        }
    }
}
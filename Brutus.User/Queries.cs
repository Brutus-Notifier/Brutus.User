using System;
using System.Threading.Tasks;

namespace Brutus.User
{
    public static class Queries
    {
        public class GetUserById
        {
            public Guid UserId { get; set; }
        }

        public class GetAllUsers
        {
            public int Page { get; set; }
            public int PageSize { get; set;  }
        }
    }
}
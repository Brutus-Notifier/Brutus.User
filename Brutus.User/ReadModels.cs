using System;

namespace Brutus.User
{
    public static class ReadModels
    {
        public class User
        {
            public User(Guid id, string firstName, string lastName, string email, string status)
            {
                Id = id;
                FirstName = firstName;
                LastName = lastName;
                Email = email;
                Status = status;
            }

            public Guid Id { get; }
            public string FirstName { get; }
            public string LastName { get; }
            public string Email { get; }
            public string Status { get; }
        }
    }
}
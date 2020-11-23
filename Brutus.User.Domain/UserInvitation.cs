using System;
using Brutus.Core;

namespace Brutus.User.Domain
{
    public class UserInvitation : Entity
    {
        public UserInvitation(Guid id, Guid userId, string email)
        {
            UserId = userId;
            Id = id;
            Email = email;
            CreatedAt = DateTime.UtcNow;
        }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
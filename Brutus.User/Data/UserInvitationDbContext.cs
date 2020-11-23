using Brutus.User.Domain;
using Microsoft.EntityFrameworkCore;

namespace Brutus.User.Data
{
    public class UserInvitationDbContext:DbContext
    {
        public UserInvitationDbContext(DbContextOptions<UserInvitationDbContext> options) : base(options) { }
        
        public virtual DbSet<UserInvitation> UserInvitations { get; set; }
    }
}
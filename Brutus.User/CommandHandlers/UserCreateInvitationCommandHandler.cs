using System;
using System.Threading.Tasks;
using Brutus.Core;
using Brutus.User.Data;
using Brutus.User.Domain;
using MassTransit;

namespace Brutus.User.CommandHandlers
{
    public class UserCreateInvitationCommandHandler: ICommandHandler<Commands.V1.CreateUserInvitation>
    {
        private readonly UserInvitationDbContext _dbContext;

        public UserCreateInvitationCommandHandler(UserInvitationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<Commands.V1.CreateUserInvitation> context)
        {
            var userInvitation = new UserInvitation(Guid.NewGuid(), context.Message.UserId, context.Message.Email);
            await _dbContext.UserInvitations.AddAsync(userInvitation);
            await _dbContext.SaveChangesAsync();

            await context.Publish(new Events.V1.UserInvitationCreated(userInvitation.Id, userInvitation.UserId));
        }
    }
}
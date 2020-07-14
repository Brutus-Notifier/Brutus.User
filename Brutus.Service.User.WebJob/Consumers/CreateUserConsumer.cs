using System.Threading.Tasks;
using Brutus.Service.User.Contracts.Commands;
using Brutus.Service.User.Domain;
using MassTransit;

namespace Brutus.Service.User.WebJob.Consumers
{
    public class CreateUserConsumer : IConsumer<CreateUser>
    {
        private readonly IPasswordObfuscator _obfuscator;

        public CreateUserConsumer(IPasswordObfuscator obfuscator)
        {
            _obfuscator = obfuscator;
        }

        public async Task Consume(ConsumeContext<CreateUser> context)
        {
            var user = new Domain.User(
                id: new UserId(context.Message.UserId),
                email: new UserEmail(context.Message.Email),
                password: UserPassword.Parse(context.Message.Password, _obfuscator));

            await context.Publish(user.GetChanges());
        }
    }
}
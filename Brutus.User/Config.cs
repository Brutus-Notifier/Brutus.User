using Brutus.User.CommandHandlers;
using Brutus.User.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Brutus.User
{
    public static class Config
    {
        public static void AddBrutusUserService(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<Commands.V1.CreateUser, Unit>, CreateUserCommandHandler>();
        }
    }
}
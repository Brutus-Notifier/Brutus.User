using Brutus.Core;
using Brutus.User.CommandHandlers;
using Brutus.User.Domain;
using Marten;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Brutus.User
{
    public static class Config
    {
        public static void AddBrutusUserService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IRepository<Domain.User>, MartenRepository<Domain.User>>();
            services.AddScoped<IRequestHandler<Commands.V1.CreateUser, Unit>, CreateUserCommandHandler>();
            services.AddScoped<IRequestHandler<Commands.V1.ChangeUserName, Unit>, ChangeUserNameCommandHandler>();
            services.AddMarten(settings =>
            {
                var conStr = configuration.GetConnectionString("Marten");
                
                settings.Connection(conStr);
                settings.AutoCreateSchemaObjects = AutoCreate.All;
            });
        }
    }
}
using Brutus.Core;
using Brutus.User.CommandHandlers;
using Brutus.User.Domain;
using Marten;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Brutus.User
{
    public static class Config
    {
        public static void AddBrutusUserService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IRepository<Domain.User>, MartenRepository<Domain.User>>();
            services.AddMarten(settings =>
            {
                var conStr = configuration.GetConnectionString("Marten");
                
                settings.Connection(conStr);
                settings.AutoCreateSchemaObjects = AutoCreate.All;
            });

            services.AddMassTransit(settings =>
            {
                settings.AddConsumersFromNamespaceContaining<CreateUserCommandHandler>();
                settings.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
                settings.AddRequestClient<Commands.V1.CreateUser>();
            });
            services.AddMassTransitHostedService();
        }
    }
}
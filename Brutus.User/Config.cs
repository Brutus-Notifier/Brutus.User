using Brutus.Core;
using Brutus.User.CommandHandlers;
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
                settings.AddConsumersFromNamespaceContaining<UserCreateCommandHandler>();
                settings.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
                settings.AddRequestClient<Brutus.User.Domain.Commands.V1.UserCreate>();
            });
            services.AddMassTransitHostedService();
        }
    }
}
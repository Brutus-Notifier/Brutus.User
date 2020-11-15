using Brutus.Core;
using Brutus.User.CommandHandlers;
using Brutus.User.Sagas;
using Brutus.User.Services;
using Marten;
using Marten.Schema;
using Marten.Services.Events;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Brutus.User
{
    public static class Config
    {
        public static void AddBrutusUserService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAggregateRepository<Domain.User>, MartenAggregateRepository<Domain.User>>();
            services.AddScoped<IEmailService, StubEmailService>();
            services.AddScoped<QueryHandler>();
            
            services.AddMarten(settings =>
            {
                var conStr = configuration.GetConnectionString("Marten");
                
                settings.Connection(conStr);
                settings.AutoCreateSchemaObjects = AutoCreate.All;
                settings.Events.UseAggregatorLookup(AggregationLookupStrategy.UsePrivateApply);
                settings.Events.InlineProjections.AggregateStreamsWith<Domain.User>();
                settings.Schema.For<Domain.User>().UniqueIndex(UniqueIndexType.DuplicatedField, x => x.Email);
                settings.UseDefaultSerialization(nonPublicMembersStorage: NonPublicMembersStorage.NonPublicSetters);
            });

            services.AddMassTransit(settings =>
            {
                var conStr = configuration.GetConnectionString("Marten");

                settings.AddConsumersFromNamespaceContaining<UserCreateCommandHandler>();
                settings.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
                settings.AddRequestClient<Commands.V1.FinishUserRegistration>();
                settings.AddRequestClient<Commands.V1.UserCreate>();
                settings.AddRequestClient<Commands.V1.UserChangeName>();
                settings.AddSagaStateMachine<UserRegistrationSaga, UserRegistrationState>()
                    .MartenRepository(conStr);
            });
            services.AddMassTransitHostedService();
        }
    }
}
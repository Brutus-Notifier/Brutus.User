using System;
using Brutus.Core;
using Brutus.User.CommandHandlers;
using Brutus.User.Projections;
using Brutus.User.Sagas;
using Brutus.User.Services;
using Marten;
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
            services.AddScoped<IRepository<Domain.User>, MartenRepository<Domain.User>>();
            services.AddScoped<IEmailService, StubEmailService>();
            
            services.AddMarten(settings =>
            {
                var conStr = configuration.GetConnectionString("Marten");
                
                settings.Connection(conStr);
                settings.AutoCreateSchemaObjects = AutoCreate.All;
                settings.Events.UseAggregatorLookup(AggregationLookupStrategy.UsePrivateApply);
                // settings.Events.InlineProjections.Add<UserRegisteredProjection>();
                
                settings.Events.ProjectView<RegisteredUser, Guid>()
                    .ProjectEvent<Events.V1.RegistrationUserFinished>(@event => @event.UserId, (view, @event) =>
                    {
                        view.Id = @event.UserId;
                        view.Email = @event.Email;
                        view.FirstName = @event.FirstName;
                        view.LastName = @event.LastName;
                        view.IsActive = true;
                    });
                    
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
                settings.AddSagaStateMachine<UserRegistrationSaga, UserRegistrationState>()
                    .MartenRepository(conStr);
            });
            services.AddMassTransitHostedService();
        }
    }
}
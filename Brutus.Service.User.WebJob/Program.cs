using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Brutus.Service.User.WebJob.Consumers;
using Brutus.Service.User.WebJob.Sagas;
using Brutus.Service.User.WebJob.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Brutus.Service.User.WebJob
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", true);
                    config.AddEnvironmentVariables();

                    if (args != null) config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IEmailService, FakeEmailService>();
                    
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumers(typeof(CreateUserConsumer).Assembly);
                        
                        x.AddSagaStateMachine<CreateUserStateMachine, CreateUserState>(typeof(RegisterUserStateMachineDefinition))
                            .RedisRepository("127.0.0.1");
                        
                        x.AddBus(ConfigureBus);
                    });
                    services.AddHostedService<MassTransitConsoleHostedService>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });
            
            if (isService)
                await builder.UseWindowsService().Build().RunAsync();
            else
                await builder.RunConsoleAsync();
        }

        static IBusControl ConfigureBus(IBusRegistrationContext context)
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                 cfg.ConfigureEndpoints(context);
            });
        }
    }
}

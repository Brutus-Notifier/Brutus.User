using System;
using System.Threading.Tasks;
using DomainCommands = Brutus.User.Domain.Commands;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Brutus.User.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private IBus _bus;
        
        public UserController(ILogger<UserController> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        [HttpPost("create")]
        public async Task CreateUser(DomainCommands.V1.UserCreate command)
        {
            await _bus.Publish(command);
        }

        [HttpPost("confirm-email")]
        public async Task ConfirmEmail(DomainCommands.V1.UserConfirmEmail command){
            throw new NotImplementedException();
        }
    }
}

using System.Threading.Tasks;
using Brutus.User.Domain;
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
        public async Task CreateUser(Commands.V1.CreateUser command)
        {
            await _bus.Publish(command);
        }

        [HttpPut]
        [Route("change-name")]
        public async Task ChangeName(Commands.V1.ChangeUserName command)
        {
            await _bus.Publish(command);
        }
    }
}

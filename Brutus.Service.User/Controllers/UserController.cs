using System;
using System.Threading.Tasks;
using Brutus.Service.User.Contracts.Commands;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Brutus.Service.User.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IBus _bus;

        public UserController(ILogger<UserController> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }
        
        [HttpPost]
        public async Task<ActionResult> Register(string email, string password)
        {
            await _bus.Publish(new CreateUser(Guid.NewGuid(), email, password));
            return NoContent();
        }
    }
}
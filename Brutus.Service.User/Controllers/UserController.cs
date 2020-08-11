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
        private readonly IRequestClient<ConfirmUserEmail> _confirmEmailRequest;

        public UserController(ILogger<UserController> logger, IBus bus, IRequestClient<ConfirmUserEmail> confirmEmailRequest)
        {
            _logger = logger;
            _bus = bus;
            _confirmEmailRequest = confirmEmailRequest;
        }
        
        [HttpPost]
        public async Task<ActionResult> Register(string email, string password)
        {
            await _bus.Publish(new CreateUser(Guid.NewGuid(), email, password));
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> ConfirmEmail(string userId, string confirmationCode)
        {
            var (status, notFound) = await _confirmEmailRequest.GetResponse<OrderStatus, OrderNotFound>(new {OrderId = id});

            if (status.IsCompletedSuccessfully)
            {
                var response = await status;
                return Ok(response.Message);
            }
            else
            {
                var response = await notFound;
                return NotFound(response.Message);
            }
        }
    }
}
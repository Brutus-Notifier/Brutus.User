using System;
using System.Threading.Tasks;
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
        private readonly IRequestClient<Commands.V1.FinishUserRegistration> _finishRegistrationClient;
        private readonly IRequestClient<Commands.V1.UserCreate> _createUserClient;

        public UserController(ILogger<UserController> logger, IBus bus, 
            IRequestClient<Commands.V1.FinishUserRegistration> finishRegistrationClient,
            IRequestClient<Commands.V1.UserCreate> createUserClient)
        {
            _logger = logger;
            _bus = bus;
            _finishRegistrationClient = finishRegistrationClient;
            _createUserClient = createUserClient;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(Commands.V1.UserCreate command)
        {
            try
            {
                await _createUserClient.GetResponse<Events.V1.RegistrationUserStarted>(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(Commands.V1.FinishUserRegistration command)
        {
            try
            {
                await _finishRegistrationClient.GetResponse<Events.V1.RegistrationUserFinished>(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

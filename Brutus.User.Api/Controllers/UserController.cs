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
        private readonly IRequestClient<Commands.V1.FinishUserRegistration> _finishRegistrationClient;
        private readonly IRequestClient<Commands.V1.UserCreate> _createUserClient;

        public UserController(ILogger<UserController> logger,
            IRequestClient<Commands.V1.FinishUserRegistration> finishRegistrationClient,
            IRequestClient<Commands.V1.UserCreate> createUserClient)
        {
            _logger = logger;
            _finishRegistrationClient = finishRegistrationClient;
            _createUserClient = createUserClient;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(Commands.V1.UserCreate command)
        {
            await _createUserClient.GetResponse<Events.V1.RegistrationUserStarted>(command);
            return NoContent();
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(Commands.V1.FinishUserRegistration command)
        {
            await _finishRegistrationClient.GetResponse<Events.V1.RegistrationUserFinished>(command);
            return NoContent();
        }
    }
}

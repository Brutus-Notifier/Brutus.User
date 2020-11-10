﻿using System.Threading.Tasks;
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
        private readonly IClientFactory _clientFactory;

        public UserController(ILogger<UserController> logger, IBus bus)
        {
            _logger = logger;
            _clientFactory = bus.CreateClientFactory();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(Commands.V1.UserCreate command)
        {
            var createUserClient = _clientFactory.CreateRequestClient<Commands.V1.UserCreate>();
            
            await createUserClient.GetResponse<Events.V1.RegistrationUserStarted>(command);
            return NoContent();
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(Commands.V1.FinishUserRegistration command)
        {
            var finishRegistrationClient = _clientFactory.CreateRequestClient<Commands.V1.FinishUserRegistration>();
            
            await finishRegistrationClient.GetResponse<Events.V1.RegistrationUserEmailConfirmed>(command);
            return NoContent();
        }

        [HttpPut("change-name")]
        public async Task<IActionResult> ChangeName(Commands.V1.UserChangeName command)
        {
            var changeUserNameClient = _clientFactory.CreateRequestClient<Commands.V1.UserChangeName>();
            
            await changeUserNameClient.GetResponse<Events.V1.SuccessResponse>(command);
            return NoContent();
        }
    }
}

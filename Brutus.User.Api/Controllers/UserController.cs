using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brutus.User.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Brutus.User.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMediator _mediator;

        public UserController(ILogger<UserController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task Post(Commands.V1.CreateUser command)
        {
            await _mediator.Send(command);
        }
    }
}

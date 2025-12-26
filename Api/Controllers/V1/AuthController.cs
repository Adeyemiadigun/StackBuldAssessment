using Application.Commands.Auth.Login;
using Application.Commands.Auth.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> Register(CreateUserDto user)
        {
            var id = await _mediator.Send(new RegisterUserCommand(user));
            return Ok(new { UserId = id });
        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var result = await _mediator.Send(new LoginCommand(request));
            return Ok(result);
        }
    }

}

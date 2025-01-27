using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.ConfigurationModels;
using IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.DTOs;

namespace Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IServiceManager _service;
        private readonly IConfiguration _configuration;

        //private readonly ISerilogManager _logger;

        public AuthenticationController(IServiceManager service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
            //_logger = serilogManagerFactory.CreateLogger<AuthenticationController>();
        }

        [HttpPost("signin-local")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SigninForLocal(
            [FromBody] AuthenticationLocalDto loginLocalDto
        )
        {
            bool checkedForce = _service.BlockBruteForceService.CheckBlockForceStatus(loginLocalDto.Email);
            if (!checkedForce)
            {
                return Unauthorized();
            }

            var authResponse = await _service.AuthenticationService.LoginLocalAsync(loginLocalDto);

            return Ok(authResponse);
        }

        [HttpPost("verify-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status401Unauthorized)]
        public IActionResult VerifyAccessToken(VerifyAccessTokenDto request)
        {
            var isVerifyJwtToken = _service.AuthenticationService.VerifyAccessToken(request.AccessToken);

            if (!isVerifyJwtToken)
            {
                return Unauthorized();
            }

            var response = new
            {
                Message = "Token is valid",
            };

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] AuthResponseDto request)
        {
            if (
                string.IsNullOrEmpty(request.AccessToken)
                || string.IsNullOrEmpty(request.RefreshToken)
            )
            {
                return Unauthorized();
            }

            var authResponse = await _service.AuthenticationService.VerifyRefreshTokenAsync(request);

            if (authResponse == null)
            {
                return Unauthorized();
            }

            return Ok(authResponse);
        }

        [HttpPost("sing-up")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SingUp([FromBody] SingUpDto singUpDto)
        {
            var authResponse = await _service.AuthenticationService.SingUpAsync(singUpDto);

            return Ok(authResponse);
        }

        [HttpPost("sign-out")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public IActionResult Signout()
        {
            _service.PlanWiseSessionService.ClearSession();

            return Ok();
        }
    }
}

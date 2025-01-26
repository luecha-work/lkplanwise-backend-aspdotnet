using Entities.ConfigurationModels;
using IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController: ControllerBase
    {
        private readonly IServiceManager _service;
        private readonly IConfiguration _configuration;
        //private readonly ISerilogManager _logger;

        public AuthenticationController(
            IServiceManager service,
            IConfiguration configuration
        )
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
            //bool checkedForce = _authServiceManager.BlockBruteforceService.CheckeBlockforceStatus(
            //    loginLocalDto.Email
            //);

            //if (!checkedForce)
            //{
            //    return Unauthorized();
            //}

            var authResponse = await _service.AuthenticationService.LoginLocalAsync(
                loginLocalDto
            );

            return Ok(authResponse);
        }

        //[HttpPost("Refresh-Token")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> RefreshToken([FromBody] AuthResponseDto request)
        //{
        //    if (
        //        string.IsNullOrEmpty(request.AccessToken)
        //        || string.IsNullOrEmpty(request.RefreshToken)
        //    )
        //    {
        //        return Unauthorized();
        //    }

        //    var authResponse =
        //        await _authServiceManager.AuthenticationService.VerifyRefreshTokenAsync(request);

        //    if (authResponse == null)
        //    {
        //        return Unauthorized();
        //    }

        //    return Ok(authResponse);
        //}

        [HttpPost("sing-up")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SingUp([FromBody] SingUpDto singUpDto)
        {
            var authResponse = await _service.AuthenticationService.SingUpAsync(singUpDto);

            return Ok(authResponse);
        }
    }
}

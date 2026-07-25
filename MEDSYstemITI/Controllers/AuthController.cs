using Application.DTOs.Auth;
using Application.DTOs.User;
using Application.Services.Abstraction.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MEDSYstemITI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Logs in with either username or email + password.</summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<RefreshTokenRequestDto>> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }


        [HttpPost("Refresh-Token")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return Ok(result);
        }

        //[Authorize]



        [AllowAnonymous]
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequestDto request)
        {
            var response = await _authService.ForgetPasswordAsync(request);

            if (!response.IsSuccess)
                return BadRequest(response.Message);


            return Ok(response);
        }

        [HttpPost("new-password")]
        public async Task<IActionResult> NewPassword([FromBody] NewPasswordRequestDto request)
        {
            var response = await _authService.ResetPasswordAsync(request);
            if (!response.isSuccess)
                return BadRequest(response.message);
            return Ok(response);
        }
    }
}

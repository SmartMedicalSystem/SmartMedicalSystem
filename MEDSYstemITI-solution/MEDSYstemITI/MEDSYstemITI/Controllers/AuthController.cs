using Application.DTOs.Auth;
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

        /// <summary>Registers a new account with the given role (Admin, Doctor, DepartmentManager, LabTechnician).</summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<RefreshTokenRequestDto>> Register([FromBody] RegisterRequestDTO request)
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }

        /// <summary>Logs in with either username or email + password.</summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<RefreshTokenRequestDto>> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("Refresh-Token")]
        public async Task<ActionResult<RefreshTokenRequestDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return Ok(result);
        }

        [Authorize]

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword( [FromBody] ChangePasswordRequestDto request)
        {
            var response = await _authService.ChangePasswordAsync(request);

            if (!response.IsSuccess)
                return BadRequest(response.Message);

            return Ok(response);
        }

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
            var response = await _authService.NewPasswordAsync(request);
            if (!response.IsSuccess)
                return BadRequest(response.Message);
            return Ok(response);
        }
    }
}

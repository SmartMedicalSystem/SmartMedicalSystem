using Application.DTOs.Auth;
using Application.DTOs.Register;
using Application.Interfaces.Services;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //[AllowAnonymous]
    [ApiController]
    public class Account : ControllerBase
    {
        private readonly IAuthService _authService;
        /// <summary>
        /// Constructor for the Account controller, which takes an instance of IAuthService as a parameter.
        /// </summary>
        /// <param name="authService"></param>
        public Account(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        ///<summary>
        /// Registers a new user.
        /// check username and email and phone number if exist or not
        /// needs just Admin role to register new user 
        /// just Add Doctor or Lab technician.
        /// must be unauthenticated user to register
        /// Need UnAuthenticated user to register
        /// </summary>

        [HttpPost("api/account/register")]
        // unathenticated 

        public async Task<IActionResult> Register(RegisterRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result.AccessToken);
            }
            else 
                return Unauthorized();
          
        }
      
     

        /// <summary> 
        /// Login endpoint for user authentication.
        /// Needs unauthenticated user to login and get the token to access the system.
        /// Login with email or username and password.
        /// </summary>
        [HttpPost("api/account/login")]       

        public async Task<IActionResult> Login(LoginRequestDto loginDTO)
        {
            var result = await _authService.LoginAsync(loginDTO);
            if (result == "Login successful")
            {
                return Ok(result);
            }
            else
            {
                return Unauthorized(result);
            }



        }
}
}


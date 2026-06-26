using API.Attributes;
using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Azure;
using Domain.Enums;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //[AllowAnonymous]
    [ApiController]
    [AllowAnonymous]
    [Route("api/account")]

    public class Auth : ControllerBase
    {
        private readonly IAuthService _authService;
        /// <summary>
        /// Constructor for the Account controller, which takes an instance of IAuthService as a parameter.
        /// </summary>
        /// <param name="authService"></param>
        public Auth(IAuthService authService)
        {
            _authService = authService;
        }


        ///<summary>
        /// Registers a new user.
        /// check username and email and phone number if exist or not
        /// needs just Admin role to register new user 
        /// just Add Doctor or Lab technician.
        /// must be unauthenticated user to register
        /// Need UnAuthenticated user to register
        /// </summary>
        [HttpPost("register")]
        // unathenticated 

        public async Task<IActionResult> Register(RegisterRequestDTO model)
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
        /// <returns> 
        /// auth  response of token 
        /// </returns>
        [HttpPost("login")]       

        public async Task<IActionResult> Login(LoginRequestDto loginDTO)
        {
            var Response = await _authService.LoginAsync(loginDTO);

            if (Response.IsSuccess)
            {
                return Ok(Response);

            }
            else
                return Unauthorized(Response.Message);
        }
    }
}



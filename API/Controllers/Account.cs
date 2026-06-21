using Application.DTOs.Register;
using Application.Interfaces.Services;
using Application.Services;
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
        private readonly IRegisterService _registerService;

        public Account(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        [HttpGet]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _registerService.RegisterAsync(model);
            return Ok(new { Message = "User registered successfully" });
        }
        [HttpGet]
        public async Task<bool> IsEmailAvailable(string email)
        {
            return !await _registerService.IsEmailExistsAsync(email);



        }
}
}

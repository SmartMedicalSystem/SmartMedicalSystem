using Application.DTOs.LabTechProfile;
using Application.DTOs.User;
using Application.Services;
using Application.Services.Abstraction;
using Application.Services.Auth;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol.Protocol;
using StackExchange.Redis;
using System.Security.Claims;

namespace MEDSYstemITI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class DoctorController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public DoctorController(IProfileService profileService)
        {
            _profileService = profileService;
        }



        /// <summary>
        /// The logged-in lab technician's own profile page.
        /// No extra permission check beyond [Authorize] - a technician can always read their own data.
        /// </summary>
        [HttpGet("me/{id:int}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<LabTechProfileReadDto>> GetMyProfile(int id)
        {
            var result = await _profileService.GetByIdAsync(id.ToString());
            return Ok(result);
        }

        /// <summary>Self-service: Save Changes button on Personal + Contact Information sections.</summary>
        [Authorize(Roles = "Doctor")]
        [HttpPut("me/{id:int}")]
        public async Task<ActionResult<LabTechProfileReadDto>> UpdateMyProfile(int id, [FromForm] LabTechProfileUpdateDto dto)
        {
            var result = await _profileService
                .UpdatePublicInfoAsync(id, dto);

            return Ok(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPut("me/user-info/{id:int}")]
        public async Task<IActionResult> UpdateUserInfo(int id,
          [FromBody] UserUpdateDto dto)
        {
            await _profileService
                .UpdateUserInfoAsync(id, dto);

            return NoContent();
        }

    }
}
using Application.DTOs.LabTechProfile;
using Application.DTOs.User;
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
    public class LabTechProfileController : ControllerBase
    {
        private readonly ILabTechProfileService _labTechProfileService;

        public LabTechProfileController(ILabTechProfileService labTechProfileService)
        {
            _labTechProfileService = labTechProfileService;
        }



        /// <summary>
        /// The logged-in lab technician's own profile page.
        /// No extra permission check beyond [Authorize] - a technician can always read their own data.
        /// </summary>
        [HttpGet("me/{id:int}")]
        [Authorize(Roles ="LabTechnician")]
        public async Task<ActionResult<LabTechProfileReadDto>> GetMyProfile(int id)
        {
            var result = await _labTechProfileService.GetByIdAsync(id.ToString());
            return Ok(result);
        }

        /// <summary>Self-service: Save Changes button on Personal + Contact Information sections.</summary>
        //[Authorize(Roles = "LabTechnician")]
        [HttpPut("me/{id:int}")]
        public async Task<ActionResult<LabTechProfileReadDto>> UpdateMyProfile(int id,[FromForm] LabTechProfileUpdateDto dto)
        {
            var result = await _labTechProfileService
                .UpdatePublicInfoAsync(id, dto);

            return Ok(result);
        }

        [HttpPut("me/user-info/{id:int}")]
        public async Task<IActionResult> UpdateUserInfo(int id,
          [FromBody] UserUpdateDto dto)
        {
            await _labTechProfileService
                .UpdateUserInfoAsync(id,dto);

            return NoContent();
        }

    }
}
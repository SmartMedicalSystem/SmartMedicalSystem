using Application.DTOs.LabTechProfile;
using Application.Services.Abstraction;
using Application.Services.Auth;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MEDSYstemITI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
        [HttpGet("me/ {id}")]
        public async Task<ActionResult<LabTechProfileReadDto>> GetMyProfile([FromBody] string id)
        {
            var result = await _labTechProfileService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>Self-service: Save Changes button on Personal + Contact Information sections.</summary>
        [HttpPut("me")]
        public async Task<ActionResult<LabTechProfileReadDto>> UpdateMyProfile( [FromBody] LabTechProfileUpdateDto dto)
        {
            var result = await _labTechProfileService.UpdatePublicInfoAsync(int.Parse(dto.id), dto);
            return Ok(result);
        }

        
    }
}
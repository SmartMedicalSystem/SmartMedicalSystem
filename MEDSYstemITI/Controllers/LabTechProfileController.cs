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

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(idClaim!);
        }

        /// <summary>
        /// The logged-in lab technician's own profile page.
        /// No extra permission check beyond [Authorize] - a technician can always read their own data.
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult<LabTechProfileReadDto>> GetMyProfile()
        {
            var result = await _labTechProfileService.GetByUserIdAsync(GetCurrentUserId());
            return Ok(result);
        }

        /// <summary>Self-service: Save Changes button on Personal + Contact Information sections.</summary>
        [HttpPut("me")]
        public async Task<ActionResult<LabTechProfileReadDto>> UpdateMyProfile([FromBody] LabTechProfileUpdateDto dto)
        {
            var result = await _labTechProfileService.UpdateOwnProfileAsync(GetCurrentUserId(), dto);
            return Ok(result);
        }

        /// <summary>Self-service: camera-icon avatar upload.</summary>
        [HttpPut("me/picture")]
        public async Task<ActionResult<LabTechProfileReadDto>> UpdateMyPicture([FromBody] LabTechProfilePictureDto dto)
        {
            var result = await _labTechProfileService.UpdateProfilePictureAsync(GetCurrentUserId(), dto);
            return Ok(result);
        }

        /// <summary>Admin: onboard a new lab technician profile.</summary>
        [HttpPost]
        [HasPermission(Permissions.CreateLabTechnician)]
        public async Task<ActionResult<LabTechProfileReadDto>> Create([FromBody] LabTechProfileCreateDto dto)
        {
            var result = await _labTechProfileService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Admin: view a specific technician's profile.</summary>
        [HttpGet("{id:int}")]
        [HasPermission(Permissions.ReadLabTechnician)]
        public async Task<ActionResult<LabTechProfileReadDto>> GetById(int id)
        {
            var result = await _labTechProfileService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>Admin: paginated list of all lab technicians.</summary>
        [HttpGet]
        [HasPermission(Permissions.ReadLabTechnician)]
        public async Task<ActionResult<PaginatedResult<LabTechProfileReadDto>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _labTechProfileService.GetAllPaginatedAsync(new PaginationParams(pageNumber, pageSize));
            return Ok(result);
        }

        /// <summary>Admin: updates the grayed-out Professional Information section.</summary>
        [HttpPut("{id:int}/professional")]
        [HasPermission(Permissions.UpdateLabTechnician)]
        public async Task<ActionResult<LabTechProfileReadDto>> UpdateProfessionalInfo(int id, [FromBody] LabTechProfileAdminUpdateDto dto)
        {
            var result = await _labTechProfileService.UpdateProfessionalInfoAsync(id, dto);
            return Ok(result);
        }

        /// <summary>Admin: activates / deactivates / sets on-leave (the "Active Status" badge).</summary>
        [HttpPut("{id:int}/status")]
        [HasPermission(Permissions.UpdateLabTechnician)]
        public async Task<ActionResult<LabTechProfileReadDto>> UpdateStatus(int id, [FromBody] LabTechProfileStatusDto dto)
        {
            var result = await _labTechProfileService.UpdateStatusAsync(id, dto);
            return Ok(result);
        }
    }
}
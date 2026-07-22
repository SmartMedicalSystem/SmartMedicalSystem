using Application.DTOs.AdminDashboard;
using Application.Services.Abstraction;
using Application.Services.Auth;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MEDSYstemITI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminDashboardController(
            IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        [HttpGet]
        [HasPermission(Permissions.ReadDashboard)]
        public async Task<ActionResult<AdminDashboardDto>> GetDashboardStats()
        {
            var result =
                await _adminDashboardService.GetDashboardStatsAsync();

            return Ok(result);
        }
    }
}

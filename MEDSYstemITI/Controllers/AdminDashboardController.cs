using Application.DTOs.AdminDashboard;
using Application.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MEDSYstemITI.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminDashboardController(
            IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        [HttpGet]
        public async Task<ActionResult<AdminDashboardDto>> GetDashboardStats()
        {
            var result =
                await _adminDashboardService.GetDashboardStatsAsync();

            return Ok(result);
        }
    }
}

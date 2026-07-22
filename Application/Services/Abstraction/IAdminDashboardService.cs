using Application.DTOs.AdminDashboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Abstraction
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetDashboardStatsAsync();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{
    /// <summary>
    /// all reads permissions
    /// all adds permission
    /// all updates permissions
    /// 
    /// 
    /// </summary>
    public enum Permissions
    {
        // Lab Reports
        ReadLabReport = 1,
        CreateLabReport,
        UpdateLabReport,
        DeleteLabReport,

        // AI Reports
        ReadAiReport,
        CreateAiReport,
        UpdateAiReport,
        DeleteAiReport,

        // Doctors
        ReadDoctor,
        CreateDoctor,
        UpdateDoctor,
        DeleteDoctor,

        // Lab Technicians
        ReadLabTechnician,
        CreateLabTechnician,
        UpdateLabTechnician,
        DeleteLabTechnician,

        // Departments
        ReadDepartment,
        CreateDepartment,
        UpdateDepartment,
        DeleteDepartment,

        // Dashboard
        ReadDashboard,

        // Requests
        RequestLabTest
    }
}
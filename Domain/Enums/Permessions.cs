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
        RequestLabTest,

        // Patients
        ReadPatient,
        CreatePatient,
        UpdatePatient,
        DeletePatient,

        // Sessions
        ReadSession,
        CreateSession,
        UpdateSession,
        DeleteSession,

        // Lab Test catalog (LabTest master data + LabTest/TestElement links)
        ReadLabTest,
        CreateLabTest,
        UpdateLabTest,
        DeleteLabTest,

        // Test Elements (reference ranges catalog)
        ReadTestElement,
        CreateTestElement,
        UpdateTestElement,
        DeleteTestElement,

        // Notifications
        ReadNotification,
        CreateNotification
    }
}
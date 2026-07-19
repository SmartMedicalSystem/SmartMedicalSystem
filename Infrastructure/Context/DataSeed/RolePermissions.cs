using Domain.Enums;

namespace Infrastructure.DataSeed;

public static class RolePermissions
{
    public static readonly Dictionary<Roles, Permissions[]> PermissionsByRole =
        new()
        {
            {
                Roles.Admin,
                Enum.GetValues<Permissions>()
            },

            {
                Roles.DepartmentManager,
                new[]
                {
                    Permissions.ReadDoctor,
                    Permissions.CreateDoctor,
                    Permissions.UpdateDoctor,
                    Permissions.DeleteDoctor,

                    Permissions.ReadLabTechnician,
                    Permissions.CreateLabTechnician,
                    Permissions.UpdateLabTechnician,
                    Permissions.DeleteLabTechnician,

                    Permissions.ReadDepartment,
                    Permissions.CreateDepartment,
                    Permissions.UpdateDepartment,
                    Permissions.DeleteDepartment,

                    Permissions.ReadDashboard,

                    Permissions.ReadLabReport,
                    Permissions.ReadAiReport,

                    Permissions.ReadPatient,
                    Permissions.ReadSession,

                    Permissions.ReadLabTest,
                    Permissions.CreateLabTest,
                    Permissions.UpdateLabTest,
                    Permissions.DeleteLabTest,

                    Permissions.ReadTestElement,
                    Permissions.CreateTestElement,
                    Permissions.UpdateTestElement,
                    Permissions.DeleteTestElement,

                    Permissions.ReadNotification,
                    Permissions.CreateNotification
                }
            },

            {
                Roles.Doctor,
                new[]
                {
                    Permissions.ReadLabReport,
                    Permissions.CreateLabReport,
                    Permissions.UpdateLabReport,

                    Permissions.ReadAiReport,

                    Permissions.ReadDashboard,

                    Permissions.RequestLabTest,

                    Permissions.ReadPatient,
                    Permissions.CreatePatient,
                    Permissions.UpdatePatient,

                    Permissions.ReadSession,
                    Permissions.CreateSession,
                    Permissions.UpdateSession,

                    Permissions.ReadLabTest,
                    Permissions.ReadTestElement,

                    Permissions.ReadNotification,
                    Permissions.CreateNotification
                }
            },

            {
                Roles.LabTechnician,
                new[]
                {
                    Permissions.ReadLabReport,
                    Permissions.UpdateLabReport,

                    Permissions.ReadAiReport,
                    Permissions.CreateAiReport,
                    Permissions.UpdateAiReport,

                    Permissions.ReadDashboard,

                    Permissions.ReadPatient,
                    Permissions.ReadSession,

                    Permissions.ReadLabTest,
                    Permissions.ReadTestElement,

                    Permissions.ReadNotification,
                    Permissions.CreateNotification
                }
            }
        };
}
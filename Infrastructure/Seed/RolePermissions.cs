using Domain.Enums;

namespace Infrastructure.Seed;

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
                    Permissions.ReadAiReport
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

                    Permissions.RequestLabTest
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

                    Permissions.ReadDashboard
                }
            }
        };
}
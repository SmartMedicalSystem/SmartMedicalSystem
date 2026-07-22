using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.AdminDashboard
{
    public class AdminDashboardDto
    {
        public int TotalPatients { get; set; }

        public int ActiveDoctors { get; set; }

        public int Departments { get; set; }

        public int Laboratories { get; set; }

        public int LabTechnicians { get; set; }
    }
}

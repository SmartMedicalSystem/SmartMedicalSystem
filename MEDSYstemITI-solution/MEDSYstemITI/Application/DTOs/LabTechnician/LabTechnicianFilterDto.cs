using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.LabTechnician
{
    public class LabTechnicianFilterDto : PaginationParams
    {
        public string? Search { get; set; }

        public string? Laboratory { get; set; }

        public EmploymentStatus? EmploymentStatus { get; set; }

        public WorkShift? WorkShift { get; set; }

        public DateOnly? JoiningDate { get; set; }
    }
}

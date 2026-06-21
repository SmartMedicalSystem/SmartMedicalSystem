using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Department
{
    public class UpdateDepartmentDto
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; } = null!;
    }
}

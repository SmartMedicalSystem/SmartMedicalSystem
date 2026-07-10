using System;

namespace Application.DTOs.Patient
{
    public class PatientCreateDto                          
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
    }
}

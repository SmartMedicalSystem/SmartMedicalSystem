using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Patient
{
    public class UpdatePatientDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
    }
}

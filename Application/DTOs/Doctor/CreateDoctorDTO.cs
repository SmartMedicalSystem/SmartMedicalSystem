namespace Application.DTOs.Doctor
{
    public class CreateDoctorDTO
    {
        public string Name { get; set; }

        public string Specialization { get; set; }

        public string Contact { get; set; }

        public int DepartmentId { get; set; }
    }
}

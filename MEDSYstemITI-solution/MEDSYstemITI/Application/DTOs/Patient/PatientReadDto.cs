namespace Application.DTOs.Patient
{
    public class PatientReadDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Age { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}

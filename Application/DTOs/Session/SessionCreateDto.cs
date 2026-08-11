namespace Application.DTOs.Session
{
    public class SessionCreateDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int DeptId { get; set; }
        public DateTime SessionDate { get; set; }
        public string? Notes { get; set; }
    }
}

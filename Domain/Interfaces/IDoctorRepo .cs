using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDoctorRepo:IGenericRepo<Doctor>
    {
        Task<List<Doctor>> GetDoctorsByDepartmentAsync(int departmentId);

        Task<Doctor> GetDoctorByPhoneAsync(string contact);

        Task<Doctor> GetDoctorWithDepartmentAsync(int id);

    }
}

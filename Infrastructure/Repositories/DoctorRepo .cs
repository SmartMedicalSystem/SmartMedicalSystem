using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DoctorRepo : GenericRepo<Doctor>,IDoctorRepo
    {
       public DoctorRepo(ApplicationDbContext context) : base(context) { }

        public async Task<Doctor> GetDoctorByPhoneAsync(string contact)
        {
            var doctor= await context.Doctors.FirstOrDefaultAsync(d=>d.Contact== contact);
            return doctor;
        }

        public async Task<List<Doctor>> GetDoctorsByDepartmentAsync(int departmentId)
        {
            var res = await context.Doctors
                .Where(d => d.DepartmentId == departmentId && !d.IsDeleted)
                .ToListAsync();

            if (!res.Any())
                throw new Exception($"No doctors found at department {departmentId}");

            return res;
        }
        public async Task<Doctor?> GetDoctorWithDepartmentAsync(int id)
        {
            return await context.Doctors
               // .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }
    }
}

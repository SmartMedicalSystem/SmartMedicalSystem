using Application.DTOs.Doctor;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;


namespace Application.Services
{
    public class DoctorService : IDoctorService
    {
        IUnitOfWork UnitOfWork;
        public DoctorService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public async Task<GetDoctorDetailsDTO>
       CreateDoctorAsync(CreateDoctorDTO dto)
        {
            var doctor = new Doctor(
                dto.Name,
                dto.Specialization,
                dto.Contact,
                dto.DepartmentId);

            var doctorExsit = await UnitOfWork.Doctors.GetDoctorByPhoneAsync(dto.Contact);
            if(doctorExsit != null) {
                throw new Exception("Phone number already exists");
            }

            //المفروض هنا كمان اتاكد ان القسم موجود

            await UnitOfWork.Doctors.AddAsync(doctor);

            await UnitOfWork.SaveChangesAsync();

            return new GetDoctorDetailsDTO
            {
                Name = doctor.Name,
                Contact = doctor.Contact,
                Specialization = doctor.Specialization,
                CreatedAt = doctor.CreatedAt,
                DepartmentName = ""
            };
        }

        public async Task<bool> DeleteDoctorAsync(int id)
        {
            var doctor = await UnitOfWork.Doctors.GetByIdAsync(id);
            if (doctor == null)
                throw new Exception("Doctor not found");

            doctor.Delete(); 
            UnitOfWork.Doctors.Update(doctor);
             await UnitOfWork.SaveChangesAsync();

            return true;
        }


        public async Task<GetDoctorDetailsDTO?> GetDoctorDetailsAsync(int id)
        {
            var res = await UnitOfWork.Doctors.GetByIdAsync(id);
            if (res == null)
                throw new Exception("Doctor not found");
            return new GetDoctorDetailsDTO()
            {
                Name = res.Name,
                Contact = res.Contact,
                Specialization = res.Specialization,
                CreatedAt = res.CreatedAt,
                DepartmentName = "",           //محتاج القسم عشان اجيب الاسم عن طريق اي دي بتاع الدكتور
                UpdatedAt = res.UpdatedAt,
            };

        }

        public async Task<List<GetDoctorDetailsDTO>> GetDoctorsByDepartmentAsync(int deptId)
        {
            // هنا لازم اتاكد ان القسم موجود 

            var res = await UnitOfWork.Doctors.GetDoctorsByDepartmentAsync(deptId);
            if (res == null)
                throw new Exception("Doctor not found");
            return res.Select(d => new GetDoctorDetailsDTO()
            {
                Name = d.Name,
                Specialization = d.Specialization,
                Contact = d.Contact,
                CreatedAt = d.CreatedAt,
                DepartmentName = "",
                UpdatedAt = d.UpdatedAt,

            }).ToList();
        }

        public async Task UpdateDoctorAsync(int id, UpdateDoctorDTO dto)
        {

            var doctor = await UnitOfWork.Doctors.GetByIdAsync(id);
            if (doctor == null)
                throw new Exception($"Doctor with ID {id} not found");

            doctor.Update(
                dto.Name ?? doctor.Name,
                dto.Specialization ?? doctor.Specialization,
                dto.Contact ?? doctor.Contact,
                dto.DepartmentId==0 ? dto.DepartmentId :  doctor.DepartmentId
            );

            UnitOfWork.Doctors.Update(doctor);
            await UnitOfWork.SaveChangesAsync();
        }
    }
}

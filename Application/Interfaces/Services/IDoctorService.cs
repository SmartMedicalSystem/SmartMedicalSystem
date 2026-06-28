using Application.DTOs.Doctor;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IDoctorService
    {
        Task<GetDoctorDetailsDTO?> GetDoctorDetailsAsync(int id);

        Task<List<GetDoctorDetailsDTO>>
            GetDoctorsByDepartmentAsync(int deptId);

        Task<GetDoctorDetailsDTO> CreateDoctorAsync(CreateDoctorDTO dto);

        Task UpdateDoctorAsync(int id, UpdateDoctorDTO dto);

        Task<bool> DeleteDoctorAsync(int id);
    }
}

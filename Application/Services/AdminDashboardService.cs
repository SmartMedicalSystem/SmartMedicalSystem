using Application.DTOs.AdminDashboard;
using Application.Services.Abstraction;
using Domain.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminDashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminDashboardDto> GetDashboardStatsAsync()
        {
            var patients = await _unitOfWork.Patients.GetAllAsync();

            var doctors = await _unitOfWork.Doctors.GetAllAsync();

            var departments = await _unitOfWork.Departments.GetAllAsync();

            var laboratories = await _unitOfWork.Laboratories.GetAllAsync();

            var labTechnicians =
                await _unitOfWork.LabTechnicians.GetAllAsync();

            return new AdminDashboardDto
            {
                TotalPatients = patients.Count(),

                ActiveDoctors = doctors.Count(),

                Departments = departments.Count(),

                Laboratories = laboratories.Count(),

                LabTechnicians = labTechnicians.Count()
            };
        }
    }
}

using Application.Common;
using Application.DTOs.LabTechnician;
using Application.DTOs.LabTechProfile;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.Common;
using Domain.Entities.Person;
using Domain.Enums;
using Domain.Identity;
using Domain.IRepository;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LabTechProfileService : ILabTechProfileService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public LabTechProfileService(IUnitOfWork uow, IMapper mapper, IFileStorageService fileStorageService)
        {
            _uow = uow;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }


        public async Task<LabTechProfileReadDto> GetByIdAsync(string id)
        {
            var entity = await _uow.LabTechProfiles.GetByIdAsync(int.Parse(id))
                ?? throw new NotFoundException("LabTechProfile", id);
            return _mapper.Map<LabTechProfileReadDto>(entity);
        }

       
        public async Task<LabTechnicianReadDto> UpdatePublicInfoAsync(int id, LabTechProfileUpdateDto dto)
        {
            var entity = await _uow.LabTechnicians.GetByIdAsync(id)
                ?? throw new NotFoundException("LabTechnician", id);

            // Update fields similar to SSN-based update
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;

            entity.PhoneNumber = dto.PhoneNumber;
            entity.Email = dto.Email;
            entity.Address = dto.Address;

            if (dto.PhotoUrl != null)
            {
                var updatedPhotoUrl = await _fileStorageService.SaveImageAsync(dto.PhotoUrl);
                entity.PhotoUrl = updatedPhotoUrl ?? entity.PhotoUrl;
            }

            await _uow.LabTechnicians.UpdateAsync(entity);
            return _mapper.Map<LabTechnicianReadDto>(entity);
        }

        public async Task UpdateUserInfo() //add here update usesr dto 
        {
            //var user = await _uow.PersonGeneric.GetByUserIdAsync();
            //add mapping between user and new values
        }




    }
}

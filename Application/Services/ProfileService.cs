using Application.Common;
using Application.DTOs.LabTechnician;
using Application.DTOs.Profile;
using Application.DTOs.User;
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
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICurrentUserService _currentUserService;

        public ProfileService(IUnitOfWork uow, IMapper mapper, IFileStorageService fileStorageService,
            ICurrentUserService currentUserService)
        {
            _uow = uow;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _currentUserService = currentUserService;
        }


        public async Task<ProfileReadDto> GetByIdAsync(string id)
        {
            var entity = await _uow.Profiles.GetByIdAsync(int.Parse(id))
           ?? throw new NotFoundException("Profile", id);


            //check if the personId from token matches the entity's personId
            await CheckPersonFromToken(int.Parse(id));
            

            return _mapper.Map<ProfileReadDto>(entity);
        }

        public async Task<ProfileReadDto> UpdatePublicInfoAsync(int id, ProfileUpdateDto dto)
        {

            var entity = await _uow.PersonGeneric.GetByIdAsync(id)
                ?? throw new NotFoundException("LabTechnician", id);
            await CheckPersonFromToken(id);


            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                entity.FirstName = dto.FirstName;

            if (!string.IsNullOrWhiteSpace(dto.LastName))
                entity.LastName = dto.LastName;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                entity.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                entity.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Address))
                entity.Address = dto.Address;

            if (dto.PhotoUrl != null)
            {
                var updatedPhotoUrl =
                    await _fileStorageService.SaveImageAsync(dto.PhotoUrl);

                entity.PhotoUrl = updatedPhotoUrl ?? entity.PhotoUrl;
            }

            await _uow.PersonGeneric.UpdateAsync(entity);

            return _mapper.Map<ProfileReadDto>(entity);
        }



        public async Task<ProfileReadDto> UpdateUserInfoAsync(int id, UserUpdateDto dto)
        {
            var userId = _currentUserService.UserId;

            if (userId is null)
                throw new UnauthorizedAccessException(
                    "Current user is not authenticated.");

            var user = await _uow.UserGeneric
                .GetByUserIdAsync(userId.Value.ToString())
                ?? throw new NotFoundException(
                    "User",
                    userId.Value);

            if (!string.IsNullOrWhiteSpace(dto.UserName))
                user.UserName = dto.UserName;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                user.PhoneNumber = dto.PhoneNumber;

            var result = await _uow.UserGeneric.UpdateUserAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }
            return _mapper.Map<ProfileReadDto>(user);
        }

   

        private async Task CheckPersonFromToken(int personId)
        {
            var id = _currentUserService.BasePersonId;

            if(id != personId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to access this resource.");
            }
        }

     
    }
}

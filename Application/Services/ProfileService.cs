using Application.Common;
using Application.DTOs.Profile;
using Application.DTOs.User;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICurrentUserService _currentUserService;

        private readonly ILogger<ProfileService> _logger;

        public ProfileService(IUnitOfWork uow, IMapper mapper, IFileStorageService fileStorageService,
            ICurrentUserService currentUserService, ILogger<ProfileService> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _currentUserService = currentUserService;
            _logger = logger;
        }


        public async Task<ProfileReadDto> GetByIdAsync(string id)
        {
            var entity = await _uow.Profiles.GetByIdAsync(int.Parse(id))
    ?? throw new NotFoundException("Profile", id);

            Console.WriteLine($"Entity Status = {entity.Status}");

            var dto = _mapper.Map<ProfileReadDto>(entity);

            Console.WriteLine($"DTO Status = {dto.Status}");

            return dto;
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

            //check if the personId from token matches the entity's personId  // extra security layer to ensure that the user can only access their own profile
            await CheckPersonFromToken(id);


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

        public async Task<UserReadDto> ChangePasswordAsync(int id, ChangePasswordRequestDto request)
        {
            //check if the personId from token matches the entity's personId  // extra security layer to ensure that the user can only access their own profile
            await CheckPersonFromToken(id);


            var userId = _currentUserService.UserId.ToString();



            var result =
                await _uow.UserGeneric.ChangePasswordAsync(
                    userId,
                    request.CurrentPassword,
                    request.NewPassword);

            if (!result.Succeeded)
            {
                var errorDetails = result.Errors.Select(e => new Application.Common.Models.ErrorDetail { Message = e.Description });



                throw new ValidationException(
                    "Password validation failed.",
                    errorDetails);
            }

            var user = await _uow.UserGeneric.GetByUserIdAsync(userId);

            return _mapper.Map<UserReadDto>(user);
        }



        private async Task CheckPersonFromToken(int personId)
        {
            var id = _currentUserService.BasePersonId;

            if (id != personId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to access this resource.");
            }
        }


    }
}

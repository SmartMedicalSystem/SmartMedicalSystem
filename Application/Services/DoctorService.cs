using Application.Common;
using Application.DTOs.Auth;
using Application.DTOs.Doctor;
using Application.DTOs.User;
using Application.Services.Abstraction;
using Application.Services.Abstraction.Auth;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserService? _userService;
        private readonly Domain.IRepository.IPersonGenericRepo _personRepo;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;


        public DoctorService(IUnitOfWork uow, IUserService? userService, 
            Domain.IRepository.IPersonGenericRepo personRepo, IFileStorageService fileStorageService, IMapper mapper)
        {
            _uow = uow;
            _userService = userService;
            _personRepo = personRepo;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }

        // Backwards-compatible overload used by tests and simple constructions
        public DoctorService(IUnitOfWork uow, Domain.IRepository.IPersonGenericRepo personRepo, IMapper mapper)
            : this(uow, null, personRepo, null, mapper)
        {
        }

        // Compatibility overloads that accept entity id instead of SSN
        public async Task<DoctorReadDto> UpdateAsync(int id, DoctorUpdateDto dto)
        {
            var entity = await _uow.Doctors.GetByIdAsync(id)
                ?? throw new NotFoundException("Doctor", id);

            entity.UpdateProfile(dto.Name, dto.Specialization, dto.Contact, dto.Gender, dto.Email, dto.MobileNumber, dto.Address);
            await _uow.Doctors.UpdateAsync(entity);
            return _mapper.Map<DoctorReadDto>(entity);
        }

        // (int-based members implemented above)

        public async Task<DoctorReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.Doctors.GetByIdAsync(id)
                ?? throw new NotFoundException("Doctor", id);
            return _mapper.Map<DoctorReadDto>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _uow.Doctors.SoftDeleteAsync(id);
        }

        public async Task<DoctorReadDto> CreateAsync(DoctorCreateDto dto)
        {
            var department = await _uow.Departments
                .GetByIdAsync(dto.DepartmentId)
                ?? throw new NotFoundException(
                    "Department",
                    dto.DepartmentId);

            var photoUrl = await _fileStorageService.SaveImageAsync(dto.PhotoUrl);


            var entity = new Doctor
            {
                FirstName = dto.Name.Split(' ', 2)[0],

                LastName = dto.Name
                .Split(' ', 2)
                .ElementAtOrDefault(1) ?? string.Empty,

                Specialization = dto.Specialization,

                PhoneNumber = dto.MobileNumber,

                DateOfBirth = dto.DateOfBirth,

                Email = dto.Email,

                Address = dto.Address,

                Gender = dto.Gender,

                DepartmentId = dto.DepartmentId,

                EncryptedNationalId = dto.NationalId,
                PhotoUrl = photoUrl

            };
            var EncryptedNationalId = dto.NationalId;
            await _uow.PersonGeneric.AddPerson(EncryptedNationalId, entity);

            var user = await _userService.CreateUserAsync(
                new CreateUserRequestDto
                {
                    FirstName = entity.FirstName,

                    LastName = entity.LastName,

                    Email = entity.Email,

                    PhoneNumber = entity.PhoneNumber,

                    Username = dto.Email,

                    Password = dto.Password,

                    Role = Roles.Doctor.ToString(),

                    PersonId = entity.Id
                });


            return _mapper.Map<DoctorReadDto>(entity);
        }

        public async Task<DoctorReadDto> UpdateAsync(string ssn, DoctorUpdateDto dto)
        {
            var person = await _personRepo.FindBySSN(ssn)
                ?? throw new NotFoundException("Doctor", ssn);

            if (person is not Domain.Entities.Doctor entity)
                throw new NotFoundException("Doctor", ssn);

            entity.UpdateProfile(dto.Name, dto.Specialization, dto.Contact, dto.Gender, dto.Email, dto.MobileNumber, dto.Address);

            // Persist changes and update encrypted SSN if NationalId changed
            await _personRepo.UpdateSSNAsync(entity, dto.NationalId.ToString());

            return _mapper.Map<DoctorReadDto>(entity);
        }

        public async Task<DoctorReadDto> GetBySSNAsync(string ssn)
        {
            var entity = await _personRepo.FindBySSN(ssn) as Domain.Entities.Doctor
                ?? throw new NotFoundException("Doctor", ssn    );
            return _mapper.Map<DoctorReadDto>(entity);
        }

        public async Task<PaginatedResult<DoctorReadDto>> GetByDepartmentAsync(int departmentId, PaginationParams pagination)
        {
            var page = await _uow.Doctors.GetByDepartmentPaginatedAsync(departmentId, pagination);
            return PaginatedResult<DoctorReadDto>.Create(
                _mapper.Map<IEnumerable<DoctorReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task DeleteAsync(string ssn)
        {
            var person = await _personRepo.FindBySSN(ssn)
                ?? throw new NotFoundException("Doctor", ssn);

            await _uow.Doctors.SoftDeleteAsync(person.Id);
        }

        public async Task<PaginatedResult<DoctorReadDto>> GetAllAsync(PaginationParams pagination)
        {
            var page = await _uow.Doctors.GetAllActivePaginatedAsync(pagination);
            return PaginatedResult<DoctorReadDto>.Create(
                _mapper.Map<IEnumerable<DoctorReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task<PaginatedResult<DoctorForSelectDto>>
     GetAvailableForNewDepartmentAsync(
         PaginationParams pagination)
        {
            var page =
                await _uow.Doctors
                    .GetAvailableForNewDepartmentAsync(pagination);

            return PaginatedResult<DoctorForSelectDto>.Create(
                _mapper.Map<IEnumerable<DoctorForSelectDto>>(page.Items),
                page.TotalCount,
                pagination);
        }
    }
}

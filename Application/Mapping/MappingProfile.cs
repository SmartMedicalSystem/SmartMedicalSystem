using Application.DTOs.Doctor;
using Application.DTOs.Laboratory;
using Application.DTOs.LabTechnician;
using Application.DTOs.Patient;
using Application.DTOs.Profile;
using Application.DTOs.User;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Person;
using DeptDto = Application.DTOs.Department;
using DoctorDto = Application.DTOs.Doctor;
using DomainEntities = Domain.Entities;
using LabTechnicianDto = Application.DTOs.LabTechnician;
using LabTestDto = Application.DTOs.LabTest;
using LabTestElementDto = Application.DTOs.LabTestElement;
using NotificationDto = Application.DTOs.Notifiaction;
using PatientDto = Application.DTOs.Patient;
using PatientResultDto = Application.DTOs.PatientResult;
using PatientResultElementDto = Application.DTOs.PatientResultElement;
using RequestLabsDto = Application.DTOs.RequestLabs;
using SessionDto = Application.DTOs.Session;
using TestElementDto = Application.DTOs.TestElement;

namespace Application.Mapping
{
    /// <summary>
    /// Single AutoMapper profile for the whole system.
    ///
    /// IMPORTANT DESIGN NOTE: this profile only maps Entity -> ReadDto (the
    /// "read side"). Entities in this project are rich domain models: they
    /// have private setters and are only constructible/mutable through their
    /// own validated constructors and methods (see Domain/Entities). Letting
    /// AutoMapper build or mutate an entity straight from an incoming
    /// Create/Update DTO would bypass that validation entirely (it uses
    /// reflection to punch through private setters). So for writes, the
    /// *Service classes call `new Entity(...)` / `entity.UpdateX(...)`
    /// directly using the DTO's values - AutoMapper is intentionally not used
    /// for that direction. This keeps "where validation happens" unambiguous:
    /// always in the entity itself.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Convert DateOnly -> DateTime when mapping entity fields (e.g. JoiningDate)
            CreateMap<DateOnly, DateTime>().ConvertUsing(d => d.ToDateTime(new TimeOnly(0, 0)));

            CreateMap<DomainEntities.Department, DeptDto.DepartmentReadDto>();
            CreateMap<DeptDto.DepartmentCreateDto, DomainEntities.Department>();
            CreateMap<DeptDto.DepartmentUpdateDto, DomainEntities.Department>();

            CreateMap<DomainEntities.Doctor, DoctorDto.DoctorReadDto>()
    .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.Name : null))
    .ForMember(d => d.EncryptedNationalId, opt => opt.MapFrom(s => s.EncryptedNationalId))
    .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber));


            CreateMap<Doctor, DoctorForSelectDto>()
           .ForMember(
               dest => dest.Name,
               opt => opt.MapFrom(src => src.FirstName + " " + src.LastName)
           )
           .ForMember(
               dest => dest.DepartmentName,
               opt => opt.MapFrom(src =>
                   src.Department != null
                       ? src.Department.Name
                       : null)
           );

            CreateMap<DoctorDto.DoctorCreateDto, DomainEntities.Doctor>();
            CreateMap<DoctorDto.DoctorUpdateDto, DomainEntities.Doctor>();


            CreateMap<Patient, PatientReadDto>()
    .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.EncryptedNationalId))
    .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.PhoneNumber));
            CreateMap<PatientDto.PatientCreateDto, DomainEntities.Patient>();
            CreateMap<PatientDto.PatientUpdateDto, DomainEntities.Patient>();


            CreateMap<DomainEntities.LabTest, LabTestDto.LabTestReadDto>();
            CreateMap<LabTestDto.LabTestCreateDto, DomainEntities.LabTest>();
            CreateMap<LabTestDto.LabTestUpdateDto, DomainEntities.LabTest>();

            CreateMap<DomainEntities.TestElement, TestElementDto.TestElementReadDto>();
            CreateMap<TestElementDto.TestElementCreateDto, DomainEntities.TestElement>();
            CreateMap<TestElementDto.TestElementUpdateDto, DomainEntities.TestElement>();


            CreateMap<DomainEntities.Session, SessionDto.SessionReadDto>();
            CreateMap<SessionDto.SessionCreateDto, DomainEntities.Session>();
            CreateMap<SessionDto.SessionUpdateDto, DomainEntities.Session>();




            CreateMap<DomainEntities.RequestLabs, RequestLabsDto.RequestLabsReadDto>()
                .ForMember(d => d.LabTestIds, opt => opt.MapFrom(s => s.LabTests.Select(lt => lt.Id).ToList()));

            CreateMap<RequestLabsDto.RequestLabsCreateDto, DomainEntities.RequestLabs>();
            CreateMap<RequestLabsDto.RequestLabsUpdateStatusDto, DomainEntities.RequestLabs>();





            CreateMap<DomainEntities.Notification, NotificationDto.NotificationReadDto>();

            CreateMap<NotificationDto.NotificationReadDto, DomainEntities.Notification>();




            CreateMap<LabTechnicianDto.LabTechnicianCreateDto, DomainEntities.LabTechnician>();

            CreateMap<LabTechnician, TechnicianBriefDto>()
             .ForMember(
                 dest => dest.Name,
                 opt => opt.MapFrom(src =>
                     $"{src.FirstName} {src.LastName}".Trim()))
             .ForMember(
                 dest => dest.LaboratoryName,
                 opt => opt.MapFrom(src =>
                     src.Laboratory != null
                         ? src.Laboratory.Name
                         : null));

            CreateMap<LabTechnicianDto.LabTechnicianUpdateDto, DomainEntities.LabTechnician>();

            CreateMap<LabTechnician, LabTechnicianReadDto>()
    .ForMember(
        dest => dest.NationalId,
        opt => opt.MapFrom(src => src.EncryptedNationalId))
    .ForMember(
        dest => dest.AssignedLaboratory,
        opt => opt.MapFrom(src =>
            src.Laboratory != null
                ? src.Laboratory.Name
                : null));

            CreateMap<LabTechnicianDto.LabTechnicianCreateDto, DomainEntities.LabTechnician>();

            CreateMap<LabTechnician, TechnicianBriefDto>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src =>
                        $"{src.FirstName} {src.LastName}".Trim()))
                .ForMember(
                    dest => dest.LaboratoryName,
                    opt => opt.MapFrom(src =>
                        src.Laboratory != null
                            ? src.Laboratory.Name
                            : null));

            CreateMap<LabTechnicianDto.LabTechnicianUpdateDto, DomainEntities.LabTechnician>();


            CreateMap<DomainEntities.PatientResult, PatientResultDto.PatientResultReadDto>();
            CreateMap<PatientResultDto.PatientResultCreateDto, DomainEntities.PatientResult>();

            CreateMap<PatientResultDto.PatientResultUpdateDto, DomainEntities.PatientResult>();

            CreateMap<Domain.Identity.ApplicationUser, UserReadDto>();
            CreateMap<Domain.Identity.ApplicationUser, ProfileReadDto>()
                .ForMember(d => d.nationalId, opt => opt.MapFrom(s => s.Person != null ? s.Person.EncryptedNationalId : null))
                .ForMember(d => d.laboratoryId, opt => opt.MapFrom(s => (s.Person as DomainEntities.LabTechnician) != null ? ((DomainEntities.LabTechnician)s.Person).LaboratoryId : (int?)null))
                .ForMember(d => d.AssignedLaboratory, opt => opt.MapFrom(s =>
                    (s.Person as DomainEntities.LabTechnician) != null
                        ? (((DomainEntities.LabTechnician)s.Person).Laboratory != null
                            ? ((DomainEntities.LabTechnician)s.Person).Laboratory.Name
                            : null)
                        : null));

            CreateMap<BasePerson, ProfileReadDto>()
                .ForMember(d => d.nationalId, opt => opt.MapFrom(s => s.EncryptedNationalId))
                .ForMember(d => d.laboratoryId, opt => opt.MapFrom(s => (int?)null))
                .ForMember(d => d.AssignedLaboratory, opt => opt.MapFrom(s => (string?)null));

            CreateMap<Doctor, ProfileReadDto>()
                .ForMember(d => d.nationalId, opt => opt.MapFrom(s => s.EncryptedNationalId))
                .ForMember(d => d.laboratoryId, opt => opt.MapFrom(s => (int?)null))
                .ForMember(d => d.AssignedLaboratory, opt => opt.MapFrom(s => (string?)null));

            CreateMap<LabTechnician, ProfileReadDto>()
                .ForMember(d => d.nationalId, opt => opt.MapFrom(s => s.EncryptedNationalId))
                .ForMember(d => d.laboratoryId, opt => opt.MapFrom(s => s.LaboratoryId))
                .ForMember(d => d.AssignedLaboratory, opt => opt.MapFrom(s => s.Laboratory != null ? s.Laboratory.Name : null));


            CreateMap<DomainEntities.PatientResultElement, PatientResultElementDto.PatientResultElementReadDto>();


            CreateMap<DomainEntities.LabTestElement, LabTestElementDto.LabTestElementReadDto>();
            CreateMap<LabTestElementDto.LabTestElementCreateDto, DomainEntities.LabTestElement>();



            //laboratory Ahmed
            CreateMap<LabTest, TestDto>();
            CreateMap<Laboratory, LaboratoryReadDto>()
                .ForMember(d => d.HeadTechnicianName, o => o.MapFrom(s => s.HeadTechnician != null ? s.HeadTechnician.Name : null))
                .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department != null ? s.Department.Name : null))
                .ForMember(d => d.TestCount, o => o.MapFrom(s => s.LabTests.Count))
                .ForMember(d => d.TechnicianCount, o => o.MapFrom(s => s.LabTechnicians.Count));

            CreateMap<LaboratoryCreateDto, Laboratory>();
            CreateMap<LaboratoryUpdateDto, Laboratory>();
            CreateMap<Laboratory, LaboratoryForSelectDto>();


        }
    }
}

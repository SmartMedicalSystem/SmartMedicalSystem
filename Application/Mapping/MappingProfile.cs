using Application.DTOs.LabTechnician;
using Application.DTOs.Department;
using Application.DTOs.Doctor;
using Application.DTOs.Laboratory;
using Application.DTOs.LabTest;
using AutoMapper;
using Domain.Entities;
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
using LabTechProfileDto = Application.DTOs.LabTechProfile;

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
            CreateMap<DomainEntities.Department, DeptDto.DepartmentReadDto>();
            CreateMap<DeptDto.DepartmentCreateDto, DomainEntities.Department>();
            CreateMap<DeptDto.DepartmentUpdateDto, DomainEntities.Department>();

            CreateMap<DomainEntities.Doctor, DoctorDto.DoctorReadDto>()
                .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.Name : null));

            CreateMap<DoctorDto.DoctorCreateDto, DomainEntities.Doctor>();
            CreateMap<DoctorDto.DoctorUpdateDto, DomainEntities.Doctor>();

            CreateMap<DomainEntities.Patient, PatientDto.PatientReadDto>();
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



            CreateMap<DomainEntities.LabTechnician, LabTechnicianDto.LabTechnicianReadDto>();
            CreateMap<LabTechnicianDto.LabTechnicianCreateDto, DomainEntities.LabTechnician>();

            CreateMap<LabTechnicianDto.LabTechnicianUpdateDto, DomainEntities.LabTechnician>();


            CreateMap<LabTechnician, LabTechnicianReadDto>().ForMember(dest => dest.NationalId, 
                opt => opt.MapFrom(src => src.EncryptedNationalId));
            CreateMap<DomainEntities.LabTechProfile, LabTechProfileDto.LabTechProfileReadDto>();
            CreateMap<LabTechProfileDto.LabTechProfileCreateDto, DomainEntities.LabTechProfile>();

            CreateMap<DomainEntities.PatientResult, PatientResultDto.PatientResultReadDto>();
            CreateMap<PatientResultDto.PatientResultCreateDto, DomainEntities.PatientResult>();

            CreateMap<PatientResultDto.PatientResultUpdateDto, DomainEntities.PatientResult>();



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

using AutoMapper;
using System.Linq;

using DomainEntities = Domain.Entities;

using DeptDto = Application.DTOs.Department;
using DoctorDto = Application.DTOs.Doctor;
using PatientDto = Application.DTOs.Patient;
using LabTestDto = Application.DTOs.LabTest;
using TestElementDto = Application.DTOs.TestElement;
using SessionDto = Application.DTOs.Session;
using RequestLabsDto = Application.DTOs.RequestLabs;
using NotificationDto = Application.DTOs.Notification;
using LabTechnicianDto = Application.DTOs.LabTechnician;
using PatientResultDto = Application.DTOs.PatientResult;
using PatientResultElementDto = Application.DTOs.PatientResultElement;
using LabTestElementDto = Application.DTOs.LabTestElement;

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

            CreateMap<DomainEntities.Doctor, DoctorDto.DoctorReadDto>()
                .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.Name : null));

            CreateMap<DomainEntities.Patient, PatientDto.PatientReadDto>();

            CreateMap<DomainEntities.LabTest, LabTestDto.LabTestReadDto>();

            CreateMap<DomainEntities.TestElement, TestElementDto.TestElementReadDto>();

            CreateMap<DomainEntities.Session, SessionDto.SessionReadDto>();

            CreateMap<DomainEntities.RequestLabs, RequestLabsDto.RequestLabsReadDto>()
                .ForMember(d => d.LabTestIds, opt => opt.MapFrom(s => s.LabTests.Select(lt => lt.Id).ToList()));

            CreateMap<DomainEntities.Notification, NotificationDto.NotificationReadDto>();

            CreateMap<DomainEntities.LabTechnician, LabTechnicianDto.LabTechnicianReadDto>();

            CreateMap<DomainEntities.PatientResult, PatientResultDto.PatientResultReadDto>();

            CreateMap<DomainEntities.PatientResultElement, PatientResultElementDto.PatientResultElementReadDto>();

            CreateMap<DomainEntities.LabTestElement, LabTestElementDto.LabTestElementReadDto>();
        }
    }
}

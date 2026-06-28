using Application.DTOs.Patient;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Mappings
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, PatientDTO>().ReverseMap();

            CreateMap<CreatePatientDTO, Patient>().ReverseMap();

            CreateMap<UpdatePatientDTO, Patient>().ReverseMap();
        }
    }
}

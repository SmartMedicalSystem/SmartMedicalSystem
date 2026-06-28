using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IPatientRepo : IGenericRepo<Patient>
    {
        Task<Patient?> GetBySSNAsync(string ssn);

    }
}

using Application.DTOs.Register;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IRegisterService
    {
        public Task RegisterAsync(RegisterDTO model);
        public Task UnregisterAsync(RegisterDTO model);


    }
}

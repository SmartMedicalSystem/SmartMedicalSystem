using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Abstraction
{
    public interface IFileStorage
    {
        Task<string?> SaveImageAsync(IFormFile? file);
    }
}

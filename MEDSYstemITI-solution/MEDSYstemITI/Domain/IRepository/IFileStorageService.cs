using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.IRepository
{
    public interface IFileStorageService
    {
        Task<string?> SaveImageAsync(IFormFile? file);
    }
}

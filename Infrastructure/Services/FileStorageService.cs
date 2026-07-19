using Domain.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public FileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string?> SaveImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);

            await file.CopyToAsync(stream);

            return $"/uploads/{fileName}";
        }
    }
}

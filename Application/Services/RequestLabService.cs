using Application.DTOs.RequestLabsDtos;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class RequestLabService : IRequestLabsService
    {

        private readonly IRequestLabsRepository _repository;

        public RequestLabService(IRequestLabsRepository repository)
        {
            _repository = repository;
        }
        public async Task<RequestLabsDto> CreateAsync(CreateRequestLabsDto dto)
        {

            var requestLab = new RequestLabs
            {
                SessionId = dto.SessionId,
                LabTestId = dto.LabTestId,
                CreatedAt = DateTime.UtcNow,
                Status = LabRequestStatus.Pending
            };
            await _repository.AddAsync(requestLab);


            return new RequestLabsDto
            {
                Id = requestLab.Id,
                SessionId = requestLab.SessionId,
                LabTestId = requestLab.LabTestId,
                CreatedAt = requestLab.CreatedAt,
                Status = requestLab.Status
            };
        }

        public async Task DeleteAsync(int id)
        {
            var requestLab =
                   await _repository.GetByIdAsync(id);

            if (requestLab == null)
                throw new Exception("Request Lab not found");

            await _repository.DeleteAsync(requestLab);
        
        }

        public async Task<IEnumerable<RequestLabsDto>> GetAllAsync()
        {

            var requestLabs = await _repository.GetAllAsync();

            return requestLabs.Select(requestLab => new RequestLabsDto
            {
                Id = requestLab.Id,
                SessionId = requestLab.SessionId,
                LabTestId = requestLab.LabTestId,
                CreatedAt = requestLab.CreatedAt,
                Status = requestLab.Status
            });

        }

        public async Task<RequestLabsDto> GetByIdAsync(int id)
        {
            var requestLab = await _repository.GetByIdAsync(id);

            if (requestLab == null)
                return null;

            return new RequestLabsDto
            {
                Id = requestLab.Id,
                SessionId = requestLab.SessionId,
                LabTestId = requestLab.LabTestId,
                CreatedAt = requestLab.CreatedAt,
                Status = requestLab.Status
            };
        }

        public async Task UpdateStatusAsync(int id, UpdateRequestLabsStatusDto dto)
        {
            var requestLab =
                 await _repository.GetByIdAsync(id);

            if (requestLab == null)
                throw new Exception("Request Lab not found");

            requestLab.Status = dto.Status;

            await _repository.UpdateAsync(requestLab);
        }
    }
}

using Application.DTOs.RequestLabsDtos;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using static Application.Services.RequestLabService;

namespace Application.Services
{
    public class RequestLabService : IRequestLabsService
    {

            private readonly IUnitOfWork _unitOfWork;

            public RequestLabService(
                IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<RequestLabsDto>
                CreateAsync(CreateRequestLabsDto dto)
            {
                var request =
                    new RequestLabs(
                        dto.SessionId,
                        dto.LabTestId);

                await _unitOfWork
                    .RequestLabs
                    .AddAsync(request);

                await _unitOfWork
                    .SaveChangesAsync();

                return MapToDto(request);
            }

            public async Task<IEnumerable<RequestLabsDto>>
                GetAllAsync()
            {
                var requests =
                    await _unitOfWork
                        .RequestLabs
                        .GetAllAsync();

                return requests
                    .Select(MapToDto);
            }

            public async Task<RequestLabsDto?>
                GetByIdAsync(int id)
            {
                var request =
                    await _unitOfWork
                        .RequestLabs
                        .GetByIdAsync(id);

                if (request == null)
                    return null;

                return MapToDto(request);
            }

            public async Task<IEnumerable<RequestLabsDto>>
                GetBySessionIdAsync(int sessionId)
            {
                var requests =
                    await _unitOfWork
                        .RequestLabs
                        .GetBySessionIdAsync(sessionId);

                return requests
                    .Select(MapToDto);
            }

            public async Task<IEnumerable<RequestLabsDto>>
                GetPendingAsync()
            {
                var requests =
                    await _unitOfWork
                        .RequestLabs
                        .GetPendingRequestsAsync();

                return requests
                    .Select(MapToDto);
            }

            public async Task StartProcessingAsync(int id)
            {
                var request =
                    await _unitOfWork
                        .RequestLabs
                        .GetByIdAsync(id);

                if (request == null)
                    throw new Exception("Request not found");

                request.StartProcessing();

                _unitOfWork
                    .RequestLabs
                    .Update(request);

                await _unitOfWork
                    .SaveChangesAsync();
            }

            public async Task CompleteAsync(int id)
            {
                var request =
                    await _unitOfWork
                        .RequestLabs
                        .GetByIdAsync(id);

                if (request == null)
                    throw new Exception("Request not found");

                request.Complete();

                _unitOfWork
                    .RequestLabs
                    .Update(request);

                await _unitOfWork
                    .SaveChangesAsync();
            }

            public async Task CancelAsync(int id)
            {
                var request =
                    await _unitOfWork
                        .RequestLabs
                        .GetByIdAsync(id);

                if (request == null)
                    throw new Exception("Request not found");

                request.Cancel();

                _unitOfWork
                    .RequestLabs
                    .Update(request);

                await _unitOfWork
                    .SaveChangesAsync();
            }

            private static RequestLabsDto
                MapToDto(RequestLabs request)
            {
                return new RequestLabsDto
                {
                    Id = request.Id,
                    SessionId = request.SessionId,
                    LabTestId = request.LabTestId,
                    RequestedAt = request.RequestedAt,
                    Status = request.Status.ToString()
                };
            }
        }
    }


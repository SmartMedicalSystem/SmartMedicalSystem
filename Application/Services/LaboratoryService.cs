using Application.Common;
using Application.DTOs.Laboratory;
using Application.DTOs.LabTechnician;
using Application.DTOs.LabTest;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.Entities;
using Domain.IRepository;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class LaboratoryService : ILaboratoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public LaboratoryService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<LaboratoryReadDto> CreateAsync(
    LaboratoryCreateDto dto)
        {
            var isUnique =
                await _uow.Laboratories
                    .IsLaboratoryNameUniqueAsync(dto.Name);

            if (!isUnique)
            {
                throw new ArgumentException(
                    $"A laboratory named '{dto.Name}' already exists.");
            }

            var entity = new Laboratory(
                dto.Name,
                dto.Location,
                dto.Phone,
                dto.Status,
                dto.HeadTechnicianId,
                dto.DepartmentId,
                dto.Code,
                dto.Specialty
            );

            await _uow.Laboratories.AddAsync(entity);

            await _uow.SaveChangesAsync();

            // Assign Head Technician to this Laboratory
            if (dto.HeadTechnicianId.HasValue)
            {
                var technician =
                    await _uow.LabTechnicians
                        .GetByIdAsync(dto.HeadTechnicianId.Value);

                if (technician is null)
                {
                    throw new NotFoundException(
                        "Lab Technician",
                        dto.HeadTechnicianId.Value);
                }

                technician.LaboratoryId = entity.Id;

                await _uow.LabTechnicians
                    .UpdateAsync(technician);

                await _uow.SaveChangesAsync();
            }

            return _mapper.Map<LaboratoryReadDto>(entity);
        }
        public async Task<LaboratoryReadDto> UpdateAsync(int id, LaboratoryUpdateDto dto)
        {
            var entity = await _uow.Laboratories.GetLaboratoryWithDetailsAsync(id)
                ?? throw new NotFoundException("Laboratory", id);

            // Check unique name (excluding current)
            var isUnique = await _uow.Laboratories.IsLaboratoryNameUniqueAsync(dto.Name, id);
            if (!isUnique)
                throw new ArgumentException($"A laboratory named '{dto.Name}' already exists.");

            entity.UpdateDetails(
                dto.Name,
                dto.Location,
                dto.Phone,
                dto.Status,
                dto.HeadTechnicianId,
                dto.DepartmentId
            );


            await _uow.Laboratories.UpdateAsync(entity);
            await _uow.SaveChangesAsync();

            return _mapper.Map<LaboratoryReadDto>(entity);
        }

        public async Task<LaboratoryReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.Laboratories.GetLaboratoryWithDetailsAsync(id)
                ?? throw new NotFoundException("Laboratory", id);

            return _mapper.Map<LaboratoryReadDto>(entity);
        }

        public async Task<PaginatedResult<LaboratoryReadDto>> GetAllAsync(
      PaginationParams pagination,
      string? searchTerm = null,
      string? statusFilter = null)
        {
            var page = await _uow.Laboratories.GetAllPaginatedAsync(pagination, searchTerm, statusFilter);

            var dtos = page.Items.Select(entity => new LaboratoryReadDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Location = entity.Location,
                Phone = entity.Phone,
                Status = entity.Status,
                HeadTechnicianId = entity.HeadTechnicianId,
                HeadTechnicianName = entity.HeadTechnician?.Name,
                DepartmentId = entity.DepartmentId,
                DepartmentName = entity.Department?.Name,
                TestCount = entity.LabTests?.Count ?? 0,
                TechnicianCount = entity.LabTechnicians?.Count ?? 0,
                CreatedAt = entity.CreatedAt
            }).ToList();

            return PaginatedResult<LaboratoryReadDto>.Create(dtos, page.TotalCount, pagination);
        }

        public async Task<IEnumerable<LaboratoryReadDto>> GetActiveLaboratoriesAsync()
        {
            var entities = await _uow.Laboratories.GetActiveLaboratoriesAsync();

            return entities.Select(entity => new LaboratoryReadDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Location = entity.Location,
                Status = entity.Status
            });
        }

        public async Task<IEnumerable<LaboratoryForSelectDto>> GetForSelectAsync()
        {
            var entities = await _uow.Laboratories.GetForSelectAsync();

            return entities.Select(entity => new LaboratoryForSelectDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Location = entity.Location,
                Status = entity.Status
            });
        }

        public async Task<bool> IsLaboratoryNameUniqueAsync(string name, int? excludeId = null)
        {
            return await _uow.Laboratories.IsLaboratoryNameUniqueAsync(name, excludeId);


        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _uow.Laboratories.ExistsAsync(id);
            if (!exists) throw new NotFoundException("Laboratory", id);

            await _uow.Laboratories.SoftDeleteAsync(id);
            await _uow.SaveChangesAsync();
        }
    }
}

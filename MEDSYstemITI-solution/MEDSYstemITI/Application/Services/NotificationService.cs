using Application.Common;
using Application.DTOs.Notification;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<NotificationReadDto> CreateAsync(NotificationCreateDto dto)
        {
            var entity = new Domain.Entities.Notification(dto.UserId, dto.Message, DateTime.UtcNow);
            await _uow.Notifications.AddAsync(entity);
            return _mapper.Map<NotificationReadDto>(entity);
        }

        public async Task<IEnumerable<NotificationReadDto>> GetUnreadByUserAsync(int userId)
        {
            var items = await _uow.Notifications.GetUnreadByUserAsync(userId);
            return _mapper.Map<IEnumerable<NotificationReadDto>>(items);
        }

        public async Task<PaginatedResult<NotificationReadDto>> GetByUserAsync(int userId, PaginationParams pagination)
        {
            var page = await _uow.Notifications.GetByUserPaginatedAsync(userId, pagination);
            return PaginatedResult<NotificationReadDto>.Create(
                _mapper.Map<IEnumerable<NotificationReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task MarkAsReadAsync(int id)
        {
            // NOTE: INotificationRepo already exposes a MarkAsReadAsync(id) at the
            // repository level; we call it directly here to avoid a redundant
            // GetById + Update round trip. The entity's own MarkAsRead() method
            // (Domain/Entities/Notification.cs) is what the repository implementation
            // should call internally to keep the invariant in one place.
            var exists = await _uow.Notifications.ExistsAsync(id);
            if (!exists) throw new NotFoundException("Notification", id);
            await _uow.Notifications.MarkAsReadAsync(id);
        }
    }
}

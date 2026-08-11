using Application.DTOs.Notifiaction;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class NotificationAppService : INotificationAppService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public NotificationAppService(
            IUnitOfWork uow,
            IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<NotificationReadDto>> GetByUserAsync(
    int userId,
    PaginationParams pagination)
        {
            var page = await _uow.Notifications
                .GetByUserPaginatedAsync(userId, pagination);

            return PaginatedResult<NotificationReadDto>.Create(
                _mapper.Map<IEnumerable<NotificationReadDto>>(page.Items),
                page.TotalCount,
                pagination);
        }

        public async Task<PaginatedResult<NotificationReadDto>> GetUnreadByUserAsync(
            int userId,
            PaginationParams pagination)
        {
            var page = await _uow.Notifications
                .GetUnreadByUserPaginatedAsync(userId, pagination);

            return PaginatedResult<NotificationReadDto>.Create(
                _mapper.Map<IEnumerable<NotificationReadDto>>(page.Items),
                page.TotalCount,
                pagination);
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _uow.Notifications.GetUnreadCountAsync(userId);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            await _uow.Notifications.MarkAsReadAsync(notificationId);
        }
    }
}

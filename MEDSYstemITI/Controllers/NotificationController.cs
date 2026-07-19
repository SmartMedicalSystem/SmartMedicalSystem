using Application.DTOs.Notifiaction;
using Application.Services.Abstraction;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MEDSYstemITI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationAppService _notificationService;

        public NotificationsController(INotificationAppService notificationService)
        {
            _notificationService = notificationService;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Get all notifications for current user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<NotificationReadDto>>> GetMyNotifications(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var pagination = new PaginationParams(pageNumber, pageSize);

            var result = await _notificationService.GetByUserAsync(CurrentUserId, pagination);

            return Ok(result);
        }

        /// <summary>
        /// Get unread notifications
        /// </summary>
        [HttpGet("unread")]
        public async Task<ActionResult<PaginatedResult<NotificationReadDto>>> GetUnreadNotifications(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var pagination = new PaginationParams(pageNumber, pageSize);

            var result = await _notificationService.GetUnreadByUserAsync(CurrentUserId, pagination);

            return Ok(result);
        }

        /// <summary>
        /// Get unread notifications count
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var count = await _notificationService.GetUnreadCountAsync(CurrentUserId);

            return Ok(count);
        }

        /// <summary>
        /// Mark notification as read
        /// </summary>
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);

            return NoContent();
        }
    }
}
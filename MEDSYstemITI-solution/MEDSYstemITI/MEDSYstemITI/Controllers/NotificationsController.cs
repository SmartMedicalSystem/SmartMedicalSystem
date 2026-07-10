using System.Security.Claims;
using Application.DTOs.Notification;
using Application.Services.Abstraction;
using Application.Services.Auth;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MEDSYstemITI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>Notifications for the currently authenticated user.</summary>
        [HttpGet("me")]
        public async Task<ActionResult<PaginatedResult<NotificationReadDto>>> GetMine(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetByUserAsync(userId, new PaginationParams(pageNumber, pageSize));
            return Ok(result);
        }

        /// <summary>Unread notifications for the currently authenticated user.</summary>
        [HttpGet("me/unread")]
        public async Task<ActionResult<IEnumerable<NotificationReadDto>>> GetMineUnread()
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetUnreadByUserAsync(userId);
            return Ok(result);
        }

        /// <summary>Admin/back-office lookup of another user's notifications.</summary>
        [HttpGet("by-user/{userId:int}")]
        [HasPermission(Permissions.ReadNotification)]
        public async Task<ActionResult<PaginatedResult<NotificationReadDto>>> GetByUser(
            int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _notificationService.GetByUserAsync(userId, new PaginationParams(pageNumber, pageSize));
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.CreateNotification)]
        public async Task<ActionResult<NotificationReadDto>> Create([FromBody] NotificationCreateDto dto)
        {
            var result = await _notificationService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id:int}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return NoContent();
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out var id) ? id : 0;
        }
    }
}

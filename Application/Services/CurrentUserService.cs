using Application.Common;
using Application.DTOs.Auth;
using Application.DTOs.User;
using Application.Services.Abstraction;
using Application.Services.Abstraction.Auth;
using Domain.Constants;
using Domain.Enums;
using Domain.Identity;
using Domain.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;
        private readonly IMemberRepo _memberRepo;
        private readonly IAuthService _authService;

        public UserService(IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger, IMemberRepo memberRepo, IAuthService authService)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _memberRepo = memberRepo;
            _authService = authService;
        }

        public int? UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                return int.TryParse(value, out var id)
                    ? id
                    : null;
            }
        }

        public int? BasePersonId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(CustomClaimTypes.BasePersonId);

                return int.TryParse(value, out var id)
                    ? id
                    : null;
            }
        }


        public async Task<AuthResponseDto> CreateUserAsync(CreateUserRequestDto request)
        {
            _logger.LogInformation(
                "Admin is attempting to create user {Username} with role {Role}",
                request.Username,
                request.Role);

            if (string.IsNullOrWhiteSpace(request.Role))
            {
                _logger.LogWarning(
                    "Registration failed: role is missing for {Username}",
                    request.Username);

                throw new ValidationException(
                    "Role is required.",
                    new[] { new Application.Common.Models.ErrorDetail { Field = "role", Message = "ROLE_REQUIRED" } });
            }

            if (!Enum.TryParse<Roles>(
                    request.Role,
                    true,
                    out var requestedRole))
            {
                _logger.LogWarning(
                    "Creating user failed: invalid role {Role}",
                    request.Role);

                throw new ValidationException(
                    "Invalid role.",
                    new[] { new Application.Common.Models.ErrorDetail { Field = "role", Message = "INVALID_ROLE" } });
            }

            if (await _memberRepo.IsValidUsernameAsync(request.Username) != null)
            {
                _logger.LogWarning(
                    "Creating user failed: username exists {Username}",
                    request.Username);

                throw new ConflictException(
                    "Username already exists.",
                    "USERNAME_ALREADY_EXISTS");
            }

            if (await _memberRepo.IsValidEmailAsync(request.Email) != null)
            {
                _logger.LogWarning(
                    "Creating user failed: email exists {Email}",
                    request.Email);

                throw new ConflictException(
                    "Email already exists.",
                    "EMAIL_ALREADY_EXISTS");
            }

            var user = new ApplicationUser
            {
                FullName = $"{request.FirstName} {request.LastName}",
                UserName = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true,
                PersonId = request.PersonId,
            };


            var result = await _memberRepo.RegisterAsync(
                user,
                request.Password);

            if (!result.Succeeded)
            {
                var errorDetails = result.Errors.Select(e => new Application.Common.Models.ErrorDetail { Message = e.Description });

                _logger.LogWarning(
                    "Registration failed for {Username}: {Errors}",
                    request.Username,
                    string.Join(", ", result.Errors.Select(e => e.Description)));

                throw new ValidationException(
                    "Password validation failed.",
                    errorDetails);
            }

            var roleAdded = await _memberRepo.AddRoleAsync(
                user,
                requestedRole.ToString());

            if (!roleAdded)
            {
                _logger.LogError(
                    "User {Username} was created but role {Role} could not be assigned",
                    user.UserName,
                    requestedRole);

                throw new InternalServerException(
                    "User was created but role assignment failed.",
                    "ROLE_ASSIGNMENT_FAILED");
            }

            _logger.LogInformation(
                "User registered successfully {Username} with role {Role}",
                user.UserName,
                requestedRole);

            return await _authService.CreateAuthResponseAsync(
                user,
                requestedRole.ToString(),
                "User registered successfully");
        }

    }
}

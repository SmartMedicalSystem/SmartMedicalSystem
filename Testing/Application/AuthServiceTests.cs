using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Auth;
using Application.Services;
using Application.Services.Abstraction.Auth;
using Application.Services.Abstraction;
using Application.Common;
using Domain.Identity;
using Domain.IRepository;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Testing.Application
{
    public class AuthServiceTests
    {
        private readonly Mock<IMemberRepo> _memberRepo = new();
        private readonly Mock<ITokenService> _tokenService = new();
        private readonly Mock<IEmailSender> _emailSender = new();
        private readonly Mock<ILogger<AuthService>> _logger = new();

        private AuthService CreateService() => new(
            _memberRepo.Object,
            _tokenService.Object,
            _emailSender.Object,
            _logger.Object
        );

        [Fact]
        public async Task LoginAsync_UserNotFound_ThrowsUnauthorizedException()
        {
            // Arrange
            _memberRepo.Setup(m => m.FindByUsernameOrEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            var svc = CreateService();

            // Act
            Func<Task> act = async () => await svc.LoginAsync(new LoginRequestDto { UserNameOrEmail = "u", Password = "p" });

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>();
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ThrowsUnauthorizedException()
        {
            // Arrange
            var user = new ApplicationUser { Id = 1, UserName = "u", Email = "a@b" };
            _memberRepo.Setup(m => m.FindByUsernameOrEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _memberRepo.Setup(m => m.IsValidPasswordAsync(It.IsAny<string>(), user)).ReturnsAsync((ApplicationUser?)null);
            var svc = CreateService();

            // Act
            Func<Task> act = async () => await svc.LoginAsync(new LoginRequestDto { UserNameOrEmail = "u", Password = "bad" });

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>();
        }

        [Fact]
        public async Task LoginAsync_NoRole_ThrowsUnauthorizedException()
        {
            // Arrange
            var user = new ApplicationUser { Id = 2, UserName = "u2", Email = "e@x" };
            _memberRepo.Setup(m => m.FindByUsernameOrEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _memberRepo.Setup(m => m.IsValidPasswordAsync(It.IsAny<string>(), user)).ReturnsAsync(user);
            _memberRepo.Setup(m => m.GetRoleAsync(user)).ReturnsAsync((string?)null);

            var svc = CreateService();

            // Act
            Func<Task> act = async () => await svc.LoginAsync(new LoginRequestDto { UserNameOrEmail = "u2", Password = "p" });

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>();
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
        {
            // Arrange
            var user = new ApplicationUser { Id = 10, PersonId = 20, UserName = "jdoe", Email = "j@d" };
            _memberRepo.Setup(m => m.FindByUsernameOrEmailAsync("jdoe")).ReturnsAsync(user);
            _memberRepo.Setup(m => m.IsValidPasswordAsync("pwd", user)).ReturnsAsync(user);
            _memberRepo.Setup(m => m.GetRoleAsync(user)).ReturnsAsync("Admin");
            _memberRepo.Setup(m => m.GetPermissionsAsync("Admin")).ReturnsAsync(new List<string> { "CanView" });

            var tokenResponse = new TokenResponseDto { AccessToken = "AT", ExpirationDate = DateTime.UtcNow.AddHours(1) };
            var refreshResponse = new TokenResponseDto { AccessToken = "RT", ExpirationDate = DateTime.UtcNow.AddHours(2) };

            _tokenService.Setup(t => t.CreateTokenAsync(user.Id, user.PersonId, user.UserName, user.Email, "Admin", It.IsAny<IEnumerable<string>>())).ReturnsAsync(tokenResponse);
            _tokenService.Setup(t => t.GenerateRefreshToken()).ReturnsAsync(refreshResponse);

            _memberRepo.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>())).Returns(Task.CompletedTask);

            var svc = CreateService();

            // Act
            var result = await svc.LoginAsync(new LoginRequestDto { UserNameOrEmail = "jdoe", Password = "pwd" });

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.AccessToken.Should().Be(tokenResponse.AccessToken);
            result.RefreshToken.Should().Be(user.RefreshToken);
        }

        [Fact]
        public async Task RefreshTokenAsync_InvalidToken_ThrowsUnauthorizedException()
        {
            // Arrange
            _memberRepo.Setup(m => m.GetByRefreshTokenAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            var svc = CreateService();

            // Act
            Func<Task> act = async () => await svc.RefreshTokenAsync(new RefreshTokenRequestDto { RefreshToken = "nope" });

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>();
        }

        [Fact]
        public async Task RefreshTokenAsync_Expired_ThrowsUnauthorizedException()
        {
            // Arrange
            var user = new ApplicationUser { Id = 5, UserName = "u", Email = "e", RefreshToken = "rt", RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(-10) };
            _memberRepo.Setup(m => m.GetByRefreshTokenAsync("rt")).ReturnsAsync(user);
            var svc = CreateService();

            // Act
            Func<Task> act = async () => await svc.RefreshTokenAsync(new RefreshTokenRequestDto { RefreshToken = "rt" });

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>();
        }
    }
}

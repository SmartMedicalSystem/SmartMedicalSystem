using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Domain.Identity;
using Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IMemberRepo _memberRepo;
    private readonly ITokenService _tokenService;

   

    public AuthService(IMemberRepo memberRepo, ITokenService tokenService)
    {
        _memberRepo = memberRepo;
        _tokenService = tokenService;
    }

    // ---------------- REGISTER ----------------
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDTO request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
            EmailConfirmed = true
        };
        //  use member repo 

        var result = await _memberRepo.RegisterAsync(user);

        if (result!= null)
        {
            throw new Exception(string.Join(", ",
                result.Errors.Select(e => e.Description)));
        }
        // assign roles
        if (request.Roles is not null && request.Roles.Any())
        {
           // await  _memberRepo.AddRoleAsync(user, request.Roles);
        }

        //var role

        var token = await _tokenService.CreateTokenAsync(user.UserName!, user.Email);
        var refreshToken = _memberRepo.GenerateRefreshToken();

        return new AuthResponseDto
        { 
            IsSuccess = true,
            Message = string.Empty,
            AccessToken = token.AccessToken,
            Expiration = token.ExpirationDate,
            RefreshToken = refreshToken,
        };
    }

    // ---------------- LOGIN ----------------
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        //user member repo ==> is valid user name 
        var user = await _memberRepo.IsValidUsernameAsync(request.UserNameOrEmail)

            //isvalidemail
                   ?? await _memberRepo.IsValidEmailAsync(request.UserNameOrEmail);

        if (user == null)
            throw new Exception("Invalid username or email");

        //user ember repo => isvalidpassword

        var passwordValid = await _memberRepo.IsValidPasswordAsync( request.Password,user);

        if (passwordValid==null)
            throw new Exception("Invalid password");

      //  var roles = await _memberRepo.AddRoleAsync(user);

        var token = await _tokenService.CreateTokenAsync(user.UserName!, user.Email);

        var refreshToken = _memberRepo.GenerateRefreshToken();

        user.RefreshToken = refreshToken;

        



        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = string.Empty,
            AccessToken = token.AccessToken,
            Expiration = token.ExpirationDate,
            RefreshToken = refreshToken
            
        };

   
    }

  

   
}
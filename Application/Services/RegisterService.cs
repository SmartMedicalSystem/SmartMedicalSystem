using Application.DTOs.Register;
using Application.Interfaces.Services;
using Domain.Identity;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IRegisterRepo _registerRepo;

        public RegisterService(IRegisterRepo registerRepo)
        {
            _registerRepo = registerRepo;
        }

        public async Task RegisterAsync(RegisterDTO model)
        {
            //validation 
            //check if email already exists is in the dto 

            

           

            //mapping DTO to entity
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
            //register user
            await _registerRepo.RegisterAsync(user, model.Password);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _registerRepo.IsEmailExistsAsync(email);
        }

        public async Task UnregisterAsync(RegisterDTO model)
        {
            await _registerRepo.UnregisterAsync(model.Email);
        }
    }
}

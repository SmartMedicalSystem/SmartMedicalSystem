using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Auth
{
    public class RegisterRequestDTO
    {
        [Required(ErrorMessage = "First name can't be empty")]
        public string FirstName { get; set; } = string.Empty;



        [Required(ErrorMessage = "Last name can't be empty")]
        public string LastName { get; set; } = string.Empty;



        [Required(ErrorMessage = "Email can't be empty")]
        //[Remote( action:"IsEmailAvailable", controller:"Account", ErrorMessage = "Email is already in use")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone can't be empty")]

      //  [Remote(action: "IsEmailAvailable", controller: "Account", ErrorMessage = "Email is already in use")]

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Username can't be empty")]

        public string Username { get; set; }


        [Required(ErrorMessage = "Password can't be empty")]


        public string Password { get; set; } = string.Empty;


        [Required(ErrorMessage ="Role can't be null")]

        public IEnumerable<string> Roles { get; set; }




    }
}

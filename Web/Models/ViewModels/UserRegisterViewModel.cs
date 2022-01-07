using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models.ViewModels
{
    public class UserRegisterViewModel
    {
        [Required(ErrorMessage = "Name field is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Lastname field is required.")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Password field is required.")]

        public string Password { get; set; }

        [Required(ErrorMessage = "Email field is required.")] 
        public string Email { get; set; }


        public User.UserGenderEnum UserGender { get; set; }
    }
}
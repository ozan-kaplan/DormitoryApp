using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models.ViewModels
{
    public class UserForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

    }
}
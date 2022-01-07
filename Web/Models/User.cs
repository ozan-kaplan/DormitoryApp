using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class User : BaseModel
    { 
        public enum Role    
        {
            SystemAdmin, Admin, Student
        }

        public enum UserGenderEnum
        {
            Unknown , Male, Female 
        }
        public enum UserStatusEnum
        {
             Pending,Active , Passive
        }

        [Required(ErrorMessage = "Name field is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Lastname field is required.")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Email field is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password field is required.")]
        public string Password { get; set; } 
       
        public UserGenderEnum UserGender { get; set; } 
       
        public Role UserRole { get; set; }

        public UserStatusEnum UserStatus { get; set; }


        public int? RoomId { get; set; }

    }
}
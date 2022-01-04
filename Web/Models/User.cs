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
            Pending, Approved, Unapproved
        }

        public string Name { get; set; }
        
        public string Lastname { get; set; }
     
    
        public string Email { get; set; }
       
     
        public string Password { get; set; } 
       
        public UserGenderEnum UserGender { get; set; } 
       
        public Role UserRole { get; set; }

        public UserStatusEnum UserStatus { get; set; }


        public int? RoomId { get; set; }

    }
}
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

        public enum Gender
        {
            Unknown , Male, Female 
        } 

        
        public string Name { get; set; }
        
        public string Lastname { get; set; }
     
    
        public string Email { get; set; }
       
     
        public string Password { get; set; } 
       
        public Gender UserGender { get; set; } 
       
        public Role UserRole { get; set; }
          
    }
}
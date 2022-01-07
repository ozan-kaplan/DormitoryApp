using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Lastname { get; set; }
        public string Email { get; set; }
        public string UserGender { get; set; }
        public string UserStatus { get; set; }
        public string UserRole { get; set; }

        public string CreatedDate { get; set; }
    }
}
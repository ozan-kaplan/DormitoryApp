using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.ViewModels
{
    public class ProfileViewModel
    {

        public ProfileViewModel()
        {
            UserItem = new User();
        }

        public User UserItem { get; set; } 
        public RoomApplicationViewModel RoomItem { get; set; }


    }
}
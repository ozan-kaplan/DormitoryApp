using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class UserRoom : BaseModel
    {
        public int UserId { get; set; }
        public int RoomId { get; set; }

    }
}
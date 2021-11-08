using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class Room : BaseModel
    {
        public string RoomName { get; set; }
        public decimal RoomFee { get; set; }
        public byte RoomCapacity { get; set; }
        public byte CurrentCapacity { get; set; }

    }
}
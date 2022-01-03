using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class RoomApplication : BaseModel
    {
        public enum Status
        {
            Pending, WaitPayment, Approved, Unapproved
        }


        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateTime ApplyDate { get; set; }

        public DateTime AccommodationStartDate { get; set; }

        public DateTime AccodomodationEndDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        public Status RoomApplicationStatus { get; set; }

    }
}
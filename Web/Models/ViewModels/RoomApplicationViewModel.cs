using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Web.Models.RoomApplication;

namespace Web.Models.ViewModels
{
    public class RoomApplicationViewModel
    {
        public int Id { get; set; }

        public string UserFullName { get; set; }

        public string RoomName { get; set; }

        public DateTime AccommodationStartDate { get; set; }

        public DateTime AccodomodationEndDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        public DateTime  ApplyDate { get; set; }

        public string RoomApplicationStatus { get; set; }
    }
}
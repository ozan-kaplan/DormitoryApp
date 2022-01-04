using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.SearchModels
{
    public class RoomApplicationSearchModel : BaseSearchModel
    {
        public int? StatusId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
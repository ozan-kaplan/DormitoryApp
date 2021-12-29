using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class DormitoryAnnouncement : BaseModel
    {
        [Required(ErrorMessage = "Announcement field is required.")]
        public string Announcement { get; set; }
        public bool IsPublished { get; set; } 
        public DateTime? PublishedDate { get; set; }

    }
}
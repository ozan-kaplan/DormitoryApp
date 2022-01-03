using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.ViewModels
{
    public class DormitoryAnnouncementViewModel
    {
        public int Id { get; set; }
        public string  Announcement { get; set; }
        public string IsPublished { get; set; } 
        public string PublishedDate { get; set; }
    }
}
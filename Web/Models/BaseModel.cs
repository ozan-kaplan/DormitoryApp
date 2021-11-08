using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }  
        public int CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; } 
        public int? ModifiedUserId { get; set; }
        public DateTime? ModifiedDate { get; set; } 
    }
}
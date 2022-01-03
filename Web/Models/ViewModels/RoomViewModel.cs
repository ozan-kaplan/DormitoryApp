using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models.ViewModels
{
    public class RoomViewModel
    {
        public int Id { get; set; }
     
        [Required(ErrorMessage = "This field is required.")]
        public string RoomName { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public decimal RoomFee { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public byte RoomCapacity { get; set; }
        public byte CurrentCapacity { get; set; } 
        public string Status { get; set; }
    }
}
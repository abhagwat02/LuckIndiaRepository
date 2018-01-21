using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LuckIndia.UI.Models
{
    public class ContactModel
    {
        [Required]
        [Display(Name = "Name :")]
        public string Name { get; set; }

        [Required]        
        [Display(Name = "Mobile No :")]
        public string MobileNumber { get; set; }

        [Display(Name = "Email :")]
        public bool Email { get; set; }

        [Display(Name = "Address :")]
        public bool Address { get; set; }

        [Display(Name = "Enquiry :")]
        public bool Enquiry { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LuckIndia.UI.Models
{
    public class CreateUserViewModels
    {
        [Required]
        [Display(Name = "User Id / Email Id :")]
        public string UserID { get; set; }

        [Required]
        [Display(Name = "First Name :")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name :")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Password :")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password :")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Role :")]
        public string Role { get; set; }

        [Required]
        [Display(Name = "Authentication Type :")]
        public string AuthType { get; set; }

        [Required]
        [Display(Name = "Email Address :")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Phone Number :")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Commision :")]
        public string Commision { get; set; }

        [Required]
        [Display(Name = "Is User Visible to Viewer :")]
        public bool IsVisible { get; set; }

        [Required]
        [Display(Name = "Restricted Amount View :")]
        public bool IsRestricted { get; set; }

        [Required]
        [Display(Name = "Play On User Behalf :")]
        public bool IsAllowed { get; set; }

        [Required]
        [Display(Name = "Is Active :")]
        public bool IsActive { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp_ebuilder.Models
{
    public class employeeRegister
    {
        [Required (AllowEmptyStrings = false,ErrorMessage = "EID is required") ]
        [Display(Name ="EID")]
        public string EID { get; set; }

        [Required (AllowEmptyStrings = false, ErrorMessage ="Password is required")]
        [DataType(DataType.Password)]
        [Display(Name ="Password")]
        public string password { get; set; }

        [Required (AllowEmptyStrings =false, ErrorMessage ="Email is required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name ="Email")]
        public string email { get; set; }

        [Display(Name ="Date of Birth")]
        public DateTime dob { get; set; }

        [Required (AllowEmptyStrings =false,ErrorMessage ="First Name is required")]
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string fName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        public string lName { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string gender { get; set; }

        [Display(Name = "Home No.")]
        public string homeNo { get; set; }

        [Display(Name = "Street")]
        public string street { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }

        [Required]
        [Display(Name = "Job Category")]
        public string jobCategory { get; set; }

    }
}
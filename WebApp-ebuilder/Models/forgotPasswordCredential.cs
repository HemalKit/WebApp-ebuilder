using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp_ebuilder.Models
{
    public class forgotPasswordCredential
    {
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage ="Enter a valid email")]
        [Required(AllowEmptyStrings =false, ErrorMessage ="Please enter your email")]
        public string email { get; set; }

        [Display(Name ="Verification Code")]
        public string verificationCode { get; set; }

        [Display(Name ="New Password")]
        [DataType(DataType.Password)]
        [StringLength(int.MaxValue, MinimumLength =8,ErrorMessage = "Minimum 8 characters")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])).{8,}$", ErrorMessage = "Password must contain numbers and both uppercase and lowercase letters.")]
        public string newPassword { get; set; }
    }
}
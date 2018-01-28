﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp_ebuilder.Models
{
    public class changePasswordCredentials
    {
        public string EID { get; set; }

        [Required(ErrorMessage ="Required")]
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string oldPassword { get; set; }

        [Required(ErrorMessage ="Password")]
        [Display(Name ="New Password")]
        [DataType(DataType.Password)]
        public string newPassword { get; set; }
    }
}
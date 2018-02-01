﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp_ebuilder.Models
{
    public class leavWithStatusAndName
    {
        public int LID { get; set; }

        [Display(Name ="Date")]
        [DataType(DataType.Date)]
        public System.DateTime date { get; set; }

        [Display(Name ="Name")]
        public string reason { get; set; }

        [Display(Name ="Leave Category")]
        public string leaveCategory { get; set; }

        [Display(Name ="Job Category")]
        public string jobCategory { get; set; }
        public string EID { get; set; }

        [Display(Name ="Status")]
        public string status { get; set; }

        [Display(Name ="First Name")]
        public string fName { get; set; }

        [Display(Name ="Last Name")]
        public string lName { get; set; }
    }
}
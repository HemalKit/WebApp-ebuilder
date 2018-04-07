using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp_ebuilder.Models
{
    public class leaveApplyForm
    {
        [Required(ErrorMessage = "Date is required")]
        [Display(Name ="Date")]
        public DateTime date { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Reason")]
        public string reason { get; set; }

        [Required(ErrorMessage ="Leave Category is required")]
        [Display(Name ="Leave Category")]
        public string leaveCategory { get; set; }


    }
}
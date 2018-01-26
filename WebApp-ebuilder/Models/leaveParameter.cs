using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace WebApp_ebuilder.Models
{
    public class leaveParameter
    {
        
        [Required(ErrorMessage ="Leave category is required")]
        [Display(Name ="Leave Category")]
        public string leaveCategory { get; set; }

        [Display(Name = "Job Category")]
        public string jobCategory { get; set; }

        public string EID { get; set; }

        [Required(ErrorMessage = "This is required")]
        [Display(Name = "From")]
        [DataType(DataType.Date)]
        public DateTime startDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "To")]
        public DateTime endDate { get; set; }




    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp_ebuilder.Models
{
    public class attendanceWithWorkingHours
    {
        public int AID { get; set; }

        [Display(Name ="Date")]
        [DataType(DataType.Date)]
        public System.DateTime date { get; set; }

        [Display(Name = "Check In")]
        public Nullable<System.TimeSpan> checkIn { get; set; }
        
        [Display(Name ="Check Out")]
        public Nullable<System.TimeSpan> checkOut { get; set; }
        public string EID { get; set; }

        [Display(Name ="Working Hours")]
        public TimeSpan workingHours { get; set; }
    }
}
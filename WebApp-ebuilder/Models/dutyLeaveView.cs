using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp_ebuilder.Models
{
    public class dutyLeaveView
    {
        public dutyLeaveView(duty_leave dl)
        {
            this.DLID = dl.DLID;
            this.EID = dl.EID;
            this.appointmentTime = dl.appointmentTime;
            this.endTime = dl.endTime;
            this.location = dl.location;
            this.date = dl.date;
            this.purpose = dl.purpose;
        }


        public int DLID { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public System.DateTime date { get; set; }

        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Appointment Time")]
        public System.TimeSpan appointmentTime { get; set; }


        public string EID { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Location")]
        public string location { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Purpose")]
        public string purpose { get; set; }

        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Required")]
        [Display(Name = "End Time")]
        public System.TimeSpan endTime { get; set; }

    }
}
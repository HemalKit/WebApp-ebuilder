//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApp_ebuilder.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class duty_leave
    {
        public duty_leave()
        {
            this.trackings = new HashSet<tracking>();
        }

        public int DLID { get; set; }  

        [Required(ErrorMessage ="Required")]
        [Display(Name ="Date")]
        public System.DateTime date { get; set; }

        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Required")]
        [Display(Name ="Appointment Time")]
        public System.TimeSpan appointmentTime { get; set; }


        public string EID { get; set; }

        [Required(ErrorMessage ="Required")]
        [Display(Name ="Location")]
        public string location { get; set; }

        [Required(ErrorMessage ="Required")]
        [Display(Name ="Purpose")]
        public string purpose { get; set; }

        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Required")]
        [Display(Name ="End Time")]
        public System.TimeSpan endTime { get; set; }



        public virtual employee employee { get; set; }
        internal virtual ICollection<tracking> trackings { get; set; }
    }
}

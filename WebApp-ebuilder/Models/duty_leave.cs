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

    public partial class duty_leave
    {
        public duty_leave()
        {
            this.trackings = new HashSet<tracking>();
        }

        public int DLID { get; set; }
        public System.DateTime date { get; set; }
        public System.TimeSpan appointmentTime { get; set; }
        public Nullable<System.TimeSpan> duration { get; set; }
        public string EID { get; set; }
        public float longitude { get; set; }
        public float latitude { get; set; }

        public virtual employee employee { get; set; }
        internal virtual ICollection<tracking> trackings { get; set; }
    }
}

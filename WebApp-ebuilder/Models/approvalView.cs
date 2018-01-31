using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp_ebuilder.Models
{
    public class approvalView
    {
        public int APID { get; set; }
        public int LID { get; set; }

        [Display(Name ="Status")]
        public string status { get; set; }

        public string ManagerID { get; set; }

        [Display(Name ="First Name")]
        public string fName { get; set; }

        [Display(Name = "Last Name")]
        public string lName { get; set; }

        [Display(Name ="Leave Category")]
        public string leaveCategory { get; set; }

        [Display(Name ="Reason")]
        public string reason { get; set; }

        [Display(Name ="Date")]
        [DataType(DataType.Date)]
        public DateTime date { get; set; }
    }
}
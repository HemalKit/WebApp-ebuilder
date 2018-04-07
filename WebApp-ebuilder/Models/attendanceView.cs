using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp_ebuilder.Models
{
    public class attendanceView
    {
        [Required]
        public System.DateTime date { get; set; }

        public TimeSpan? checkIn { get; set; }
        public TimeSpan? checkOut { get; set; }

        [Required]
        public string EID { get; set; }
    }
}
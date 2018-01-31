using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp_ebuilder.Models
{
    public class forgotPasswordCredential
    {
        public string email { get; set; }
        public string verificationCode { get; set; }
        public string newPassword { get; set; }
    }
}
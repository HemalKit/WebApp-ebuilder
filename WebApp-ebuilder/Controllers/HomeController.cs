using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp_ebuilder.Authorizer;

namespace WebApp_ebuilder.Controllers
{
    public class HomeController : BaseController
    {
        public RedirectToRouteResult Index()
        {
            return RedirectToAction("Login","Employees");
        }
        

        [CustomAuthorize]
        public ActionResult Dashboard()
        {
            ViewBag.UserRole = User.Role; 
            return View();
        }
    }
}
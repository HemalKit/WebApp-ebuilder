using System;
using System.Web.Mvc;
using WebApp_ebuilder.Models;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web;

namespace WebApp_ebuilder.Controllers
{
    public class EmployeesController : Controller
    {
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Register([Bind(Exclude = "emailVerified,activationCode")] employee newEmployee)
        {
            string message = "";
            if (ModelState.IsValid)
            {
                using (HttpClient client = new HttpClient())
                {
                    string BaseUrl = "http://localhost:61355/api/";
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    // check whether an employee already exists with the given ID
                    var response = await client.GetAsync("Employees/" + newEmployee.EID);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        message = "Employee with the EID already available";
                    }
                    else
                    {
                        message = "The employee was not found with EID";
                        newEmployee.activationCode = Guid.NewGuid().ToString();
                        newEmployee.emailVerified = false;

                        var serializer = new JavaScriptSerializer();
                        var json = serializer.Serialize(newEmployee);
                        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                        response = await client.PostAsync("Employees", stringContent);
                        if (response.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            message = "Successfully added";
                            //code for sending email verification code should come here
                        }
                        else
                        {
                            message = message + "Some Error has occured";
                        }
                    }

                }

            }
            else
            {
                message = "Invalid request";

            }
            ViewBag.message = message;
            return View(newEmployee);


        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Login(employeeLogin login, string ReturnUrl="")
        {
            string message = "";
            using (HttpClient client = new HttpClient())
            {
                string BaseUrl = "http://localhost:61355/api/";
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(login);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");


                var response = await client.PostAsync("Access", stringContent);
                if (response.IsSuccessStatusCode)
                {
                    if (response.Content.ReadAsStringAsync().Result == "true")
                    {
                        message = "Success Login";

                        int timeout = login.rememberMe ? 525600 : 1; //525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(login.email, login.rememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);

                        }
                        else
                        {
                            return RedirectToAction("Dashboard", "Home");
                        }

                    }
                    else if (response.Content.ReadAsStringAsync().Result == "false")
                    {
                        message = " Wrong Email or Password";
                    }
                }
                else
                {
                    message = "An error occured";
                }

                ViewBag.message = message;
                return View();
            }


        }


        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Employees");
        }


        [Authorize]
        [HttpGet]
        public ActionResult Profile()
        {
            return View();
        }
    }
}
using System;
using System.Web.Mvc;
using WebApp_ebuilder.Models;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web;
using Newtonsoft.Json;

namespace WebApp_ebuilder.Controllers
{
    public class EmployeesController : BaseController
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
            string message = "Out of the try block";
            try
            {
                message = "Entered the try block";
                using (HttpClient client = new HttpClient())
                {
                    message = "Inside the using client block";
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
                       
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            message = "Success Login";

                            var responseData = response.Content.ReadAsStringAsync().Result;
                            var empData = JsonConvert.DeserializeObject<employee>(responseData);

                            customPrincipalSerializeModel serializeEmployee = new customPrincipalSerializeModel();
                            serializeEmployee.email = empData.email;
                            serializeEmployee.FirstName = empData.fName;
                            serializeEmployee.LastName = empData.lName;
                            serializeEmployee.Role = empData.jobCategory;

                            string accessData = JsonConvert.SerializeObject(serializeEmployee);

                            int timeout = login.rememberMe ? 525600 : 10; //525600 min = 1 year

                            var ticket = new FormsAuthenticationTicket(1, login.email, DateTime.Now, DateTime.Now.AddMinutes(timeout), true, accessData);
                            string encrypted = FormsAuthentication.Encrypt(ticket);

                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            cookie.HttpOnly = true;
                            Response.Cookies.Add(cookie);// add cookie with the encrypted ticket


                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);

                            }
                            else
                            {
                                return RedirectToAction("Dashboard", "Home");
                            }

                        }
                        else if (response.Content.ReadAsStringAsync().Result == null)
                        {
                            message = " Wrong Email or Password";
                        }
                    }
                    else
                    {
                        message = "Wrong Email or Password ";
                    }

                    ViewBag.message = message;
                    return View();
                }
            }
            catch(Exception ex)
            {
                ViewBag.message = ex;
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
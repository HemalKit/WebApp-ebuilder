using System;
using System.Web.Mvc;
using WebApp_ebuilder.Models;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web;
using Newtonsoft.Json;
using WebApp_ebuilder.Authorizer;
using System.Net.Http.Headers;
using System.Collections;
using System.Collections.Generic;

namespace WebApp_ebuilder.Controllers
{
    public class EmployeesController : BaseController
    {
        [HttpGet]
        [CustomAuthorize(Roles="HR Admin")]
        public ActionResult Register()
        {
            ViewBag.Message = null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles ="HR Admin")]
        public async System.Threading.Tasks.Task<ActionResult> Register(employeeRegister newEmployeeForm)
        {
            string message = null;
            if (ModelState.IsValid)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    // check whether an employee already exists with the given ID
                    var response = await client.GetAsync("Employees/" + newEmployeeForm.EID);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        message = "Employee with the EID already available";
                    }
                    else
                    {
                        employee newEmployee = new employee();

                        newEmployee.EID = newEmployeeForm.EID;
                        newEmployee.password = newEmployeeForm.password;
                        newEmployee.email = newEmployeeForm.email;
                        newEmployee.dob = newEmployeeForm.dob;
                        newEmployee.fName = newEmployeeForm.fName;
                        newEmployee.lName = newEmployeeForm.lName;
                        newEmployee.gender = newEmployeeForm.gender;
                        newEmployee.homeNo = newEmployeeForm.homeNo;
                        newEmployee.street = newEmployeeForm.street;
                        newEmployee.city = newEmployeeForm.city;
                        newEmployee.jobCategory = newEmployeeForm.jobCategory;
                        
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
            ViewBag.Message = message;
            return View(newEmployeeForm);
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
            try
            {
                message = "Entered the try block";
                using (HttpClient client = new HttpClient())
                {
                   
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/Json"));

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
                            serializeEmployee.EID = empData.EID;
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


        [CustomAuthorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Employees");
        }


        [CustomAuthorize(Roles = "HR Admin,Managerial")]
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> ViewEmployees()
        {
            using (HttpClient client = new HttpClient())
            {
                //var message = "";
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.GetAsync("Employees");
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var employeeData = JsonConvert.DeserializeObject<List<employee>>(responseData);
                    return View(employeeData);
                    
                }
                else
                {
                    return View();
                }
            }
        }


        [CustomAuthorize(Roles = "HR Admin")]
        public async System.Threading.Tasks.Task<ActionResult> Delete(string EID)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.DeleteAsync("");
                response = await client.DeleteAsync("Employees/"+EID);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ViewBag.Message = "Sucessfully deleted";
                }
                else
                {
                    ViewBag.Message = "Unsuccessful";
                }

                return View();
            }

        }


        [CustomAuthorize(Roles = "HR Admin")]
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> Edit(string EID)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.GetAsync("Employees/"+ EID);
                var responseData = response.Content.ReadAsStringAsync().Result;
                var employeeData = JsonConvert.DeserializeObject<employee>(responseData);
                return View(employeeData);
            }           
        }

        [HttpPost]
        [CustomAuthorize]
        public async System.Threading.Tasks.Task<ActionResult> Edit(employee newEmployeeData)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/Json"));

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(newEmployeeData);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync("Employees/"+newEmployeeData.EID, stringContent);

                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ViewBag.Message = "Successfully updated";
                }

                else
                {
                    ViewBag.Message = "Error occured";
                }
                return View();
            }

        }

        [HttpGet]
        [CustomAuthorize]
        public ActionResult Settings()
        {
            return View();
        }

        [HttpGet]
        [CustomAuthorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize]
        public ActionResult ChangePassword(changePasswordCredentials credentials)
        {
            return View();
        }

        [HttpGet]
        [CustomAuthorize]
        public async System.Threading.Tasks.Task<ActionResult> ViewProfile()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.GetAsync("Employees/" + User.EID);
                var responseData = response.Content.ReadAsStringAsync().Result;
                var employeeData = JsonConvert.DeserializeObject<employee>(responseData);
                return View(employeeData);
            }
        }


        [HttpGet]
        [CustomAuthorize(Roles ="Managerial,HR Admin")]
        public ActionResult Manage()
        {
            return View();
        }


        /* [HttpDelete]
         public async System.Threading.Tasks.Task<EmptyResult> RemoveAsync(string id)
         {
             using (HttpClient client = new HttpClient())
             {
                 client.BaseAddress = new Uri(BaseUrl);
                 client.DefaultRequestHeaders.Accept.Clear();
                 client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/Json"));

                 var response = await client.DeleteAsync("Employees/" + id);
                 if( response.StatusCode == System.Net.HttpStatusCode.OK)
                 {
                     return EmptyResult;
                 }

             }
         }*/
    }
}
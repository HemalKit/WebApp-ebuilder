using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebApp_ebuilder.Models;
using System.Collections;
using Newtonsoft.Json;
using WebApp_ebuilder.Authorizer;
using System.Web.Script.Serialization;
using System.Text;

namespace WebApp_ebuilder.Controllers
{
    [CustomAuthorize(Roles ="HR Admin")]
    public class LeaveTypesController : BaseController
    {
        // GET: LeaveTypes
        
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.GetAsync("LeaveTypes");
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var leaveTypes = JsonConvert.DeserializeObject<List<leave_type>>(responseData);
                    return View(leaveTypes);
                }
                return View();
            }

        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Create(leave_type newLeaveType)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(newLeaveType);
                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("LeaveTypes", stringContent);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        message = "Successfully created";
                    }
                    else
                    {
                        message = "Some error occured";
                    }

                }
            }
            else
            {
                message = "Invalid data";
            }
            ViewBag.Message = message;
            return View();
        }

        [HttpGet]
        public ActionResult Edit(string jobCategory, string leaveCategory)
        {
            return View();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Edit(leave_type newLeaveType)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(newLeaveType);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");


                var response = await client.PutAsync("LeaveTypes?jobCategory=" + newLeaveType.jobCategory + "&leaveCategory=" + newLeaveType.leaveCategory, stringContent);
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ViewBag.Message = "Updating leave types successful";
                }
                else
                {
                    ViewBag.Message = "Error occured";
                }
                return View();
            }
        }
    }
}
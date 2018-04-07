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
    //only HR admin users can do actions in this class
    [CustomAuthorize(Roles ="HR Admin")]
    public class LeaveTypesController : BaseController
    {
        //diplay all the leave types
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

        //display the page to add new leave type
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        //get the form data from the view and pass them to the api
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Create(leave_type newLeaveType)
        {
            var message = "";
            //validate the form data
            if (ModelState.IsValid)
            {
                
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    var json = JsonConvert.SerializeObject(newLeaveType);
                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("LeaveTypes", stringContent);
                    if (response.IsSuccessStatusCode)
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

        //diplay the page to edit an existing leave type
        [HttpGet]
        public ActionResult Edit(string jobCategory, string leaveCategory)
        {
            return View();
        }

        //get the form data from the form and update by passing the data to the api
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Edit(leave_type newLeaveType)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var json = JsonConvert.SerializeObject(newLeaveType);
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
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using WebApp_ebuilder.Authorizer;
using WebApp_ebuilder.Models;

namespace WebApp_ebuilder.Controllers
{
    [CustomAuthorize]
    public class LeavesController : BaseController
    {
        //diplay the page with apply leave frorm
        [CustomAuthorize]
        [HttpGet]               
        public ActionResult ApplyLeave()
        {
            
            return View();
        }


        //Get the completed form from from view and pass to the api
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize]
        public async System.Threading.Tasks.Task<ActionResult> ApplyLeave(leaveApplyForm leaveForm)
        {
            try
            {
                var message = "";
                if (ModelState.IsValid)
                {

                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                        var response = await client.GetAsync("Leaves/GetAvailable?EID=" + User.EID);

                        var responseData = response.Content.ReadAsStringAsync().Result;
                        var leavesAvailable = JsonConvert.DeserializeObject<List<leave_type>>(responseData);
                        
                        //check whether all leaves are taken
                        if (leavesAvailable.FirstOrDefault(lt => lt.leaveCategory == leaveForm.leaveCategory).maxAllowed <= 0)
                        {
                            message = "All leaves for this category are already taken";
                            ViewBag.Message = message;
                            return View();
                        }
                        leav newLeave = new leav();
                        newLeave.EID = User.EID;
                        newLeave.date = leaveForm.date.Date;
                        newLeave.reason = leaveForm.reason;
                        newLeave.jobCategory = User.Role;
                        newLeave.leaveCategory = leaveForm.leaveCategory;

                        
                        var json = JsonConvert.SerializeObject(newLeave);
                        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                        response = await client.PostAsync("Leaves", stringContent);

                        if (response.IsSuccessStatusCode)
                        {

                            ViewBag.Message = "Leave applying successful";
                            return View(new leaveApplyForm());
                        }
                        else
                        {
                            message = "Error Occured";
                          
                        }
                    }
                }
                ViewBag.Message = message;
                return View();
            }
            catch (Exception)
            {
                ViewBag.Message = "Error Occured";
                return View();
            }
            
        }

        
        //return to the page with charts for each leave category
        [HttpGet]
        [CustomAuthorize]
        public ActionResult ViewLeavesSummary()
        {
            
            return View();
        }


        //diplay the list of leaves of the employee
        [HttpGet]
        [CustomAuthorize]
        public async System.Threading.Tasks.Task<ActionResult> ViewLeaves(string EID)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                        var response = await client.GetAsync("Leaves/All?EID=" + EID);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseData = response.Content.ReadAsStringAsync().Result;
                            var leaveList = JsonConvert.DeserializeObject<List<leavWithStatusAndName>>(responseData);

                            return View(leaveList);
                        }
                        else
                        {
                            ViewBag.Message = "Error Occured";
                            return View();
                        }
                    }
                }
                ViewBag.Message = "Error Occured";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Message = "Error Occured";
                return View();
            }
        }


        //pass the json array to creates charts for the each type of leave category
        [CustomAuthorize]
        public async System.Threading.Tasks.Task<JsonResult> LeavesCount()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.GetAsync("Leaves/LeaveCount?EID=" + User.EID );
                if (true)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var leaveData = JsonConvert.DeserializeObject<List<count>>(responseData);

                    var chartList = new List<Chart>();
                    
                    foreach (var ld in leaveData)
                    {
                        Chart _chart = new Chart();
                        _chart.labels = new string[] { "taken", "left" };
                        _chart.datasets = new List<Datasets>();
                        List<Datasets> _dataSet = new List<Datasets>();
                        _dataSet.Add(new Datasets()
                        {
                            label = ld.leaveCategory,
                            yLabels = "Hours",
                            data = new int[] { ld.takenCount, ld.leftCount },
                            backgroundColor = new string[] { "#a30303", "#29ad2e" },
                            borderColor = new string[] { "#a30303", "#29ad2e" },
                            borderWidth = "1"
                        });
                        _chart.datasets = _dataSet;
                        chartList.Add(_chart);
                        
                    }
                    
                    return Json(chartList, JsonRequestBehavior.AllowGet);
                    
                }

            }
        }


        //diplay the page to edit  a leave
        [HttpGet]
        [CustomAuthorize]
        public async System.Threading.Tasks.Task<ActionResult> Edit(int LID)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.GetAsync("Leaves/" + LID.ToString());
                var responseData = response.Content.ReadAsStringAsync().Result;
                var leave = JsonConvert.DeserializeObject<leav>(responseData);

                return View(leave);
            }
        }

        //pass the edied leave to the api
        [HttpPost]
        [CustomAuthorize]
        public async System.Threading.Tasks.Task<ActionResult> Edit(leav leave)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    var json = JsonConvert.SerializeObject(leave);
                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PutAsync("Leaves/" + leave.LID.ToString(), stringContent);

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "Success";
                        return View();
                    }
                    else
                    {
                        ViewBag.Message = "Error Occured";
                        return View();
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Error Occured";
                return View();
            }
        }

        //diplay the page to delete a leave
        [HttpGet]
        [CustomAuthorize]
        public async System.Threading.Tasks.Task<ActionResult> Delete(int LID)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    var response = await client.GetAsync("Leaves/" + LID.ToString());
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;

                        var leave = JsonConvert.DeserializeObject<leav>(responseData);
                        return View(leave);
                    }
                    else
                    {
                        ViewBag.Message = "Error Occured";
                        return View();
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Error Occured";
                return View();
            }            
        }

        //call the api to delete a leave if the user press the delete button
        [HttpPost]
        [CustomAuthorize]
        public async System.Threading.Tasks.Task<ActionResult> Delete(leav leave)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    var response = await client.DeleteAsync("Leaves/"+leave.LID);
                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "Success";
                        return View();
                    }
                    else
                    {
                        ViewBag.Message = "Error Occured";
                        return View();
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Errror Occured";
                return View();
            }
        }        
    }
}
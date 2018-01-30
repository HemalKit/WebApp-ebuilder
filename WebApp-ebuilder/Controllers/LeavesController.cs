using Newtonsoft.Json;
using System;
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
        // GET: Leaves
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize]
        [HttpGet]               
        public ActionResult ApplyLeave()
        {
            return View();
        }


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

                        if (leavesAvailable.FirstOrDefault(lt => lt.leaveCategory == leaveForm.leaveCategory).maxAllowed <= 0)
                        {
                            message = "All leaves for this category are already taken";
                            ViewBag.Message = message;
                            return View();
                        }
                        leav newLeave = new leav();
                        newLeave.EID = User.EID;
                        newLeave.date = leaveForm.date;
                        newLeave.reason = leaveForm.reason;
                        newLeave.jobCategory = User.Role;
                        newLeave.leaveCategory = leaveForm.leaveCategory;

                        var serializer = new JavaScriptSerializer();
                        var json = serializer.Serialize(newLeave);
                        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                        response = await client.PostAsync("Leaves", stringContent);

                        if (response.IsSuccessStatusCode)
                        {

                            message = "Leave applying successful";
                        }
                        else
                        {
                            message = "Error occured";
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

        [HttpGet]
        public ActionResult ViewLeaves()
        {
            
            return View();
        }


        public async System.Threading.Tasks.Task<JsonResult> LeavesTaken()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.GetAsync("Leaves/LeaveCount?EID=" + User.EID + "&startDate=2017-1-1&endDate=2018-10-10");
                if (true)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var leaveData = JsonConvert.DeserializeObject<allLeaveCount>(responseData);
                    

                    List<string> labels = new List<string>();
                    leaveData.taken.ForEach(c => labels.Add(c.name));

                    List<int> takenCount = new List<int>();
                    List<int> leftCount = new List<int>();

                    leaveData.taken.ForEach(c => takenCount.Add(c.number));
                    leaveData.left.ForEach(c => leftCount.Add(c.number));



                    Chart _chart = new Chart();
                    _chart.labels = labels.ToArray();
                    _chart.datasets = new List<Datasets>();
                    List<Datasets> _dataSet = new List<Datasets>();
                    _dataSet.Add(new Datasets()
                    {
                        label = "Current Year",
                        data = takenCount.ToArray(),
                        backgroundColor = new string[] { "#00ff00", "#0000FF" },
                        borderColor = new string[] { "#00ff00", "#0000FF" },
                        borderWidth = "1"
                    });
                    _chart.datasets = _dataSet;
                    return Json(_chart, JsonRequestBehavior.AllowGet);


                    // return View();
                }

            }
        }

        public async System.Threading.Tasks.Task<JsonResult> LeavesLeft()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.GetAsync("Leaves/LeaveCount?EID=" + User.EID + "&startDate=2017-1-1&endDate=2018-10-10");
                if (true)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var leaveData = JsonConvert.DeserializeObject<allLeaveCount>(responseData);

                    List<string> labels = new List<string>();
                    leaveData.taken.ForEach(c => labels.Add(c.name));

                    List<int> takenCount = new List<int>();
                    List<int> leftCount = new List<int>();

                    leaveData.taken.ForEach(c => takenCount.Add(c.number));
                    leaveData.left.ForEach(c => leftCount.Add(c.number));



                    Chart _chart = new Chart();
                    _chart.labels = labels.ToArray();
                    _chart.datasets = new List<Datasets>();
                    List<Datasets> _dataSet = new List<Datasets>();
                    _dataSet.Add(new Datasets()
                    {
                        label = "Current Year",
                        data = leftCount.ToArray(),
                        backgroundColor = new string[] { "#0000FF", "#ffff00" },
                        borderColor = new string[] { "#0000FF", "#ffff00" },
                        borderWidth = "1"
                    });
                    _chart.datasets = _dataSet;
                    return Json(_chart, JsonRequestBehavior.AllowGet);


                    // return View();
                }

            }
        }


        public JsonResult LeavesTook()
        {
            Chart _chart = new Chart();
            _chart.labels = new string[] { "January", "February", "March", "April", "May", "June", "July" };
            _chart.datasets = new List<Datasets>();
            List<Datasets> _dataSet = new List<Datasets>();
            _dataSet.Add(new Datasets()
            {
                label = "Current Year",
                data = new int[] { 28, 48, 40, 19, 86, 27, 90 },
                backgroundColor = new string[] { "#FF0000", "#800000", "#808000", "#008080", "#800080", "#0000FF", "#000080", "#999999", "#E9967A", "#CD5C5C", "#1A5276", "#27AE60" },
                borderColor = new string[] { "#FF0000", "#800000", "#808000", "#008080", "#800080", "#0000FF", "#000080", "#999999", "#E9967A", "#CD5C5C", "#1A5276", "#27AE60" },
                borderWidth = "1"
            });
            _chart.datasets = _dataSet;
            return Json(_chart, JsonRequestBehavior.AllowGet);
        }
    }
}
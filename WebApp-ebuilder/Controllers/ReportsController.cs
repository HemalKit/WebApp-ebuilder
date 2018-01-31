using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebApp_ebuilder.Authorizer;
using WebApp_ebuilder.Models;

namespace WebApp_ebuilder.Controllers
{
    [CustomAuthorize(Roles = "HR Admin,Managerial")]
    public class ReportsController : BaseController
    {
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LeaveReports()
        {
            return View();
        }

        //needs edit
        [HttpPost]
        public RedirectToRouteResult LeaveReports(leaveParameter parameter)
        {
            if (parameter.leaveCategory == "all")
            {
                return RedirectToAction("AllLeaveReports", new { startDate = parameter.startDate, endDate = parameter.endDate });
            }
            else
            {
                return RedirectToAction("LeaveReportsByTypes", parameter);
            }
        }


        //Get the all taken and available leave counts within a given range for the logged in user
        //needs edit
        //public async System.Threading.Tasks.Task<ActionResult> AllLeaveReports(string startDate, string endDate)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(BaseUrl);
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

        //        var response = await client.GetAsync("Leaves/LeaveCount?EID=" + User.EID + "&startDate=" + startDate + "&endDate=" + endDate);
        //        var responseData = response.Content.ReadAsStringAsync().Result;
        //        var allLeaves = JsonConvert.DeserializeObject<allLeaveCount>(responseData);
        //        return View();
        //    }
        //}

        //needs edit
        public async System.Threading.Tasks.Task<ActionResult> LeaveReportsByTypes(leaveParameter parameter)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.GetAsync("LeaveCountByType?EID=" + User.EID + "&leaveCategory=" + parameter.leaveCategory + "&startDate=" + parameter.startDate + "&endDate=" + parameter.endDate);
                var responseData = response.Content.ReadAsStringAsync().Result;
                var leaveCount = JsonConvert.DeserializeObject<count>(responseData);
                return View();

            }
        }

        public async System.Threading.Tasks.Task<JsonResult> LeavesAppliedByMonth()
        {
            
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    var response = await client.GetAsync("Leaves/LeavesAppliedMonthly");
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    appliedLeavesPercentViewMonthly LeavePercentMonth = JsonConvert.DeserializeObject<appliedLeavesPercentViewMonthly>(responseData);                    

                    Chart _chart = new Chart();
                    _chart.labels = new string[] { "January", "Febrauary", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                    _chart.datasets = new List<Datasets>();
                    List<Datasets> _dataSet = new List<Datasets>();
                    _dataSet.Add(new Datasets()
                    {
                        label = "Applied Leaves by Month",
                        data = new int[] { (int)Math.Round(LeavePercentMonth.January,0),
                                           (int)Math.Round(LeavePercentMonth.February,0),
                                           (int)Math.Round(LeavePercentMonth.March,0),
                                           (int)Math.Round(LeavePercentMonth.April,0),
                                           (int)Math.Round(LeavePercentMonth.May,0),
                                           (int)Math.Round(LeavePercentMonth.June,0),
                                           (int)Math.Round(LeavePercentMonth.July,0),
                                           (int)Math.Round(LeavePercentMonth.August,0),
                                           (int)Math.Round(LeavePercentMonth.September,0),
                                           (int)Math.Round(LeavePercentMonth.October,0),
                                           (int)Math.Round(LeavePercentMonth.November,0),
                                           (int)Math.Round(LeavePercentMonth.December,0)
                        },
                        backgroundColor = new string[] { "#f4424b", "#5e4e84", "#5d7572", "#d2d615", "#1b0a66", "#05af57", "#177209", "#afe25d", "#9aa870", "#a2a2d8", "#e80909", "#5fa86d" },
                        borderColor = new string[] { "#00ff00", "#0000FF" },
                        borderWidth = "1"
                    });
                    _chart.datasets = _dataSet;

                    return Json(_chart, JsonRequestBehavior.AllowGet);
                }
            
        }

        public async System.Threading.Tasks.Task<ActionResult> LeavesAppliedByWeekday()
        {
           
                appliedLeavesPercentView LeaveCountByWeekDay;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    var response = await client.GetAsync("Leaves/LeavesAppliedWeekly");
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    LeaveCountByWeekDay = JsonConvert.DeserializeObject<appliedLeavesPercentView>(responseData);


                    Chart _chart = new Chart();
                    _chart.labels = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
                    _chart.datasets = new List<Datasets>();
                    List<Datasets> _dataSet = new List<Datasets>();
                    _dataSet.Add(new Datasets()
                    {
                        label = "Applied Leaves by Month",
                        data = new int[] { (int)Math.Round(LeaveCountByWeekDay.Monday,0),
                                           (int)Math.Round(LeaveCountByWeekDay.Tuesday,0),
                                           (int)Math.Round(LeaveCountByWeekDay.Wednesday,0),
                                           (int)Math.Round(LeaveCountByWeekDay.Thursday,0),
                                           (int)Math.Round(LeaveCountByWeekDay.Friday,0),
                                           (int)Math.Round(LeaveCountByWeekDay.Saturday,0),
                                           (int)Math.Round(LeaveCountByWeekDay.Sunday,0)                                           
                        },
                        backgroundColor = new string[] { "#f4424b", "#5e4e84", "#5d7572", "#d2d615", "#1b0a66", "#05af57", "#177209", "#afe25d", "#9aa870", "#a2a2d8", "#e80909", "#5fa86d" },
                        borderColor = new string[] { "#00ff00", "#0000FF" },
                        borderWidth = "1"
                    });
                    _chart.datasets = _dataSet;
                return Json(_chart, JsonRequestBehavior.AllowGet);
                }           
        }
    }
}
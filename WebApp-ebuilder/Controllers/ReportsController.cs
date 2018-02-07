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
        //go to the page with charts, the view contains the charts
        public ActionResult Index()
        {
            return View();
        }

        //pass the chart object as json to the index view to craete a chart 
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

        //pass the chart object as json to the index view to craete a chart
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
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApp_ebuilder.Authorizer;
using WebApp_ebuilder.Models;


namespace WebApp_ebuilder.Controllers
{
    [CustomAuthorize]
    public class AttendanceController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        public async System.Threading.Tasks.Task<ActionResult> ViewAttendance(string EID)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                //Set the headers to get the data in json format
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.GetAsync("Attendance?EID=" + User.EID);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    List<attendanceWithWorkingHours> attendances = JsonConvert.DeserializeObject<List<attendanceWithWorkingHours>>(responseData);
                    return View(attendances);
                }
                return View();
            }
        }
        
        public ActionResult ViewAttendanceChart()
        {
            return View();
        }
        
        public async System.Threading.Tasks.Task<JsonResult> ViewAttendanceAsChart(string EID )
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date;
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var response = await client.GetAsync("Attendance?EID=" + User.EID+"&startDate="+firstDayOfMonth.ToString()+"&endDate="+lastDayOfMonth.ToString());
                var responseData = response.Content.ReadAsStringAsync().Result;
                List<attendanceWithWorkingHours> attendance = JsonConvert.DeserializeObject<List<attendanceWithWorkingHours>>(responseData);

                List<string> xValue = new List<string>();
                List<int> yValue = new List<int>();
                attendance.ForEach(a => xValue.Add(a.date.Date.ToString("dd/MM/yyyy")));
                attendance.ForEach(a => yValue.Add((int)a.workingHours.TotalHours));

                List<string> bgColors = new List<string>();
                int bgColorBase = 0x000001;
                xValue.ForEach(x => bgColors.Add("#" + (bgColorBase +=0xA ).ToString()));


                Chart _chart = new Chart();
                _chart.labels = xValue.ToArray();
                _chart.datasets = new List<Datasets>();
                List<Datasets> _dataSet = new List<Datasets>();
                _dataSet.Add(new Datasets()
                {
                    label = "This Month",
                    data = yValue.ToArray(),
                    backgroundColor =bgColors.ToArray(),
                    borderColor = bgColors.ToArray(),
                    borderWidth = "1"
                });
                _chart.datasets = _dataSet;
                return Json(_chart, JsonRequestBehavior.AllowGet);                                 
            }
        }

        [HttpGet]
        public ActionResult AddAttendance()
        {
            return View();

        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> AddAttendance(attendanceView attView)
        {
            if (attView != null)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    ViewBag.Message = "";

                    attendance newAttendance = new attendance();
                    newAttendance.EID = attView.EID;
                    newAttendance.date = attView.date;
                    newAttendance.checkIn = TimeSpan.Parse("08:00:00"); 
                    newAttendance.checkOut = attView.checkOut;
                    
                    var json = JsonConvert.SerializeObject(newAttendance);
                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("Attendance", stringContent);
                    

                    //string attstring = att.ToString();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ViewBag.Message = "Created";
                    }
                    ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                    return View();
                }
            }
            else
            {
                ViewBag.Message = "attendance list is null";
                return View();
            }

        }

    }


}
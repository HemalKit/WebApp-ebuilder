using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApp_ebuilder.Models;
using Rotativa;

namespace WebApp_ebuilder.Controllers
{
    public class AttendanceController : BaseController
    {
        // GET: Attendance
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
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.GetAsync("Attendance?EID=" + EID);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    List<attendance> attendances = JsonConvert.DeserializeObject<List<attendance>>(responseData);
                    return View(attendances);
                }
                return View();
            }
        }


        public ActionResult ExportPdf()
        {
            ActionAsPdf result = 
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
                    

                    //var serializer = new JavaScriptSerializer();
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
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
    [CustomAuthorize(Roles ="HR Admin,Managerial")]
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
        public async System.Threading.Tasks.Task<ActionResult> AllLeaveReports(string startDate, string endDate)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.GetAsync("Leaves/LeaveCount?EID=" + User.EID + "&startDate=" + startDate + "&endDate=" + endDate);
                var responseData = response.Content.ReadAsStringAsync().Result;
                var allLeaves = JsonConvert.DeserializeObject<allLeaveCount>(responseData);


                return View();
            }
        }


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
    }
}
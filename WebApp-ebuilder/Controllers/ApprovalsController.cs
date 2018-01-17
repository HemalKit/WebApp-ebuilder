using Newtonsoft.Json;
using System;
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
    
    public class ApprovalsController : BaseController
    {
        // GET: Approvals
        [CustomAuthorize(Roles = "Managerial, HR Admin")]
        public async System.Threading.Tasks.Task<ActionResult> ViewPendingApprovals()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.GetAsync("Approvals?ManagerID=" + User.EID + "&status=pending");
                var responseData = response.Content.ReadAsStringAsync().Result;
                var approvalData = JsonConvert.DeserializeObject<List<approval>>(responseData);

                return View(approvalData);
            }           
        }

        [CustomAuthorize(Roles = "Managerial, HR Admin")]
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> Edit(int APID)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.GetAsync("Approvals/" + APID.ToString());
                var responseData = response.Content.ReadAsStringAsync().Result;
                var ApprovalData = JsonConvert.DeserializeObject<approval>(responseData);

                return View(ApprovalData);
            }            
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Edit(approval newApproval)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(newApproval);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync("Approvals/" + newApproval.APID.ToString(), stringContent);

                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ViewBag.Message = "Update Successful";
                }
                else
                {
                    ViewBag.Message = response.Content.ReadAsStringAsync().Result.ToString();
                }
                return View();
               
            }
        }

    }
}
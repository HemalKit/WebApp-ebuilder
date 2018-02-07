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
        //View Pending approvals that are to be approved
        //Only HR Admin and Managerial user are allowed
        [CustomAuthorize(Roles = "Managerial,HR Admin")]
        public async System.Threading.Tasks.Task<ActionResult> ViewPendingApprovals()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                //Call the API
                var response = await client.GetAsync("Approvals?ManagerID=" + User.EID + "&status=pending");
                //Read the body of http resonse from API
                var responseData = response.Content.ReadAsStringAsync().Result;
                //Convert to lis of objects from Json
                var approvalData = JsonConvert.DeserializeObject<List<approvalView>>(responseData);

                //Return the list of approval view objects to the view
                return View(approvalData);
            }           
        }

        //View an approval to approve or reject
        //APID = id of the approval
        [CustomAuthorize(Roles = "Managerial, HR Admin")]
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> Edit(int APID)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                //call the api to get the details of the approval
                var response = await client.GetAsync("Approvals/" + APID.ToString());
                var responseData = response.Content.ReadAsStringAsync().Result;

                //Convert from Json to approval object
                var ApprovalData = JsonConvert.DeserializeObject<approval>(responseData);

                //pass the approval object to view
                return View(ApprovalData);
            }            
        }

        //Call the api to update the status of an approval, newApproval object comes from the view
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Edit(approval newApproval)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                //Convert the edited approval object json
                var json = JsonConvert.SerializeObject(newApproval);

                //creates a stringcontent object to pass to api, which is derived from httpcontent class
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                //call the api
                var response = await client.PutAsync("Approvals/" + newApproval.APID.ToString(), stringContent);

                //check whether it was successfully updated
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //alert to be displayed
                    ViewBag.Message = "Update Successful";
                }
                else
                {
                    //alert to be dispalyed
                    ViewBag.Message = response.Content.ReadAsStringAsync().Result.ToString();
                }
                return View();
               
            }
        }

    }
}
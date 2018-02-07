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
    [CustomAuthorize]
    public class DutyLeavesController : BaseController
    {
       
        //get the trackings for a particular duty leave, DLID is the id of the duty leave
        public async System.Threading.Tasks.Task<ActionResult> TrackLocation(int DLID)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    //call the api
                    var response = await client.GetAsync("Trackings?DLID=" + DLID.ToString());
                    //if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        //read the content of the response
                        var responseData = response.Content.ReadAsStringAsync().Result;

                        //convert from json to lis of tracking objects
                        List<tracking> trackingData = JsonConvert.DeserializeObject<List<tracking>>(responseData);

                        //pass the list of trackings to the view
                        return View(trackingData);
                    }
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Error Occured";
                return View();
            }


        }


        //display the form to apply duty leave
        [HttpGet]        
        public ActionResult ApplyDutyLeave()
        {
            return View();
        }

        //pass the new duty leave object to api which is coming from the form
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> ApplyDutyLeave(duty_leave newDutyLeave)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        newDutyLeave.EID = User.EID;
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                        var json = JsonConvert.SerializeObject(newDutyLeave);
                        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync("DutyLeaves", stringContent);

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            ViewBag.Message = "Successfully Added";

                        }
                        else
                        {
                            ViewBag.Message = "Error Occured" ;
                        }
                    }
                }
                return View();
                                                
            }
            catch (Exception)
            {
                ViewBag.Message = "Error occured";
                return View();
            }
        }


        //display the list of duty leaves applied by a certain user
        public async System.Threading.Tasks.Task<ActionResult> ViewDutyLeaves(string EID)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    //call the api to get the list of duty leaves applied by the logged in user
                    var response = await client.GetAsync("DutyLeaves?EID=" +EID);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        var dutyLeavesData = JsonConvert.DeserializeObject<List<duty_leave>>(responseData);
                        var dutyLeaveViews = new List<dutyLeaveView>();
                        dutyLeavesData.ForEach(dl => dutyLeaveViews.Add(new dutyLeaveView(dl)));
                        return View(dutyLeaveViews);
                    }
                    else
                    {
                        ViewBag.Message = "An error occured";
                    }
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "An error occured";
                return View();
            }
        }


        //diplay the page to delete a duty leave applied by the user
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> Delete(int DLID)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    var response = await client.GetAsync("DutyLeaves/" + DLID.ToString());
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var dutyLeave = JsonConvert.DeserializeObject<duty_leave>(responseData);

                    return View(dutyLeave);
                }
            }
            catch (Exception)
            {
                return ViewBag.Message = "Error Occured";
            }
        }


        //delete a duty leave if the user pressed the delete button
        public async System.Threading.Tasks.Task<ActionResult> Delete(duty_leave dutyLeave)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    var response = await client.DeleteAsync("DutyLeaves/" + dutyLeave.DLID.ToString());
                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "Success";
                    }
                    else
                    {
                        ViewBag.Message = "Error Occured";
                    }
                }
                return View();
            }
            catch (Exception)
            {
                ViewBag.Message = "Error Occured";
                return View();
            }

        }


        //display the page to edit the duty leave
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> Edit(string DLID)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    var response = await client.GetAsync("DutyLeaves/" + DLID.ToString());
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var dutyLeave = JsonConvert.DeserializeObject<duty_leave>(responseData);

                    return View(dutyLeave);
                }
            }
            catch (Exception)
            {
                return ViewBag.Message = "Error Occured";
            }
        }


        //update the api if the user edited a duty leave
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Edit(duty_leave dutyLeave)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    var json = JsonConvert.SerializeObject(dutyLeave);
                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PutAsync("DutyLeaves/"+dutyLeave.DLID.ToString(),stringContent);
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
    }       
            
}
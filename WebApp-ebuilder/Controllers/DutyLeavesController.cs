﻿using Newtonsoft.Json;
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
    public class DutyLeavesController : BaseController
    {
        // GET: DutyLeaves
        public ActionResult Index()
        {
            return View();
        }

        public async System.Threading.Tasks.Task<ActionResult> TrackLocation(int DLID)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                var response = await client.GetAsync("Trackings?DLID=" + DLID.ToString() );
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    List<tracking> trackingData = JsonConvert.DeserializeObject<List<tracking>>(responseData);
                    return View(trackingData);

                }
                return View();
            }

        }

        [HttpGet]
        [CustomAuthorize]
        public ActionResult ApplyDutyLeave()
        {
            return View();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> ApplyDutyLeave(duty_leave newDutyLeave)
        {
            using (HttpClient client = new HttpClient())
            {
                newDutyLeave.EID = User.EID;
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                //var serializer = new JavaScriptSerializer();
                var json = JsonConvert.SerializeObject(newDutyLeave);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("DutyLeaves", stringContent);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ViewBag.Message = "Created";

                }
                else
                {
                    ViewBag.Message = "Some Error" + response.Content.ReadAsStringAsync().Result; ;
                }
            }
            return View();

        }


        [CustomAuthorize]
        public async System.Threading.Tasks.Task<ActionResult> ViewDutyLeaves()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                    var response = await client.GetAsync("DutyLeaves?EID="+User.EID);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        var dutyLeavesData = JsonConvert.DeserializeObject<List<duty_leave>>(responseData);
                        return View(dutyLeavesData);
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

    }
}
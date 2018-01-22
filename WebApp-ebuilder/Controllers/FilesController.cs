using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApp_ebuilder.Models;

namespace WebApp_ebuilder.Controllers
{
    public class FilesController : BaseController
    {
        // GET: Files
        public ActionResult Index()
        {
            return View();
        }

        public async System.Threading.Tasks.Task<ActionResult> Upload(FormCollection formCollection)
        {
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                    var attendances = new List<attendance>();

                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        ViewBag.Message = package.Workbook.ToString();
                        //return View(new List<user>());
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.FirstOrDefault();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;


                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var newAttendance = new attendance();
                            newAttendance.EID = workSheet.Cells[rowIterator, 1].Value.ToString();
                            char[] arr = { ' ' };
                            newAttendance.date = (DateTime)workSheet.Cells[rowIterator, 2].Value;
                            newAttendance.checkIn = TimeSpan.Parse(workSheet.Cells[rowIterator, 3].Value.ToString().Split(arr)[1]);
                            newAttendance.checkOut = TimeSpan.Parse(workSheet.Cells[rowIterator, 4].Value.ToString().Split(arr)[1]);
                            attendances.Add(newAttendance);
                        }
                    }
                    //return View(attendances);

                    if (attendances != null)
                    {
                        ViewBag.Message = "";
                        foreach (attendance newAttendance in attendances)
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(BaseUrl);
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/Json"));

                                //var serializer = new JavaScriptSerializer();
                                var json = JsonConvert.SerializeObject(newAttendance);
                                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                                var response = await client.PostAsync("Attendance", stringContent);

                                ViewBag.Message += response.Content.ReadAsStringAsync().Result;

                            }
                        }
                        return View(attendances);
                        //return RedirectToAction("AddAttendance", "Attendance", new { attendances = attendances });
                    }
                    return View(new List<attendance>());
                }
                return View(new List<attendance>());
            }
            return View(new List<attendance>());
        }
    }
}
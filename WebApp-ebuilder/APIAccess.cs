using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Text;

namespace WebApp_ebuilder
{
    public class APIAccess
    {
        private HttpClient SetupClient(HttpClient client)
        {
            string BaseUrl = "http://localhost:61355/api/";
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/Json"));
            return client;

        }
      
        public async Task<HttpResponseMessage> GetRequestAsync(string url)
        {           
            using (var client = new HttpClient())
            {                               
                var response = await SetupClient(client).GetAsync("url");
                return response;

            }
        }

        public async Task<HttpResponseMessage> PostRequestAsync(string url, object model)
        {
            using (var client = new HttpClient())
            {
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(model);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                
                return await SetupClient(client).PostAsync(url, stringContent);

            }
        }
    }
}
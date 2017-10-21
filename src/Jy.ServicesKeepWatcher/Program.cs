using Jy.ServicesKeep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jy.ServicesKeepWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if("Success".Equals(CheckAPI("http://localhost:5100/api/v1/Index")))
                        AuthAPIRegister.registerAuthAPIHostPort("192.168.1.115:12181,192.168.1.116:12181");

                    Thread.Sleep(10000);
                }
            });
            Console.WriteLine("ServicesKeepWatcher ready!");
            Console.ReadLine();
        }

        static string CheckAPI(string ServerURL)
        {
            HttpClient _httpClient = new HttpClient();
            var parameters = new Dictionary<string, string>();
            var message_token = _httpClient.PostAsync("http://localhost:5100/HealthCheck", new FormUrlEncodedContent(parameters)).Result;
            string res =  message_token.Content.ReadAsStringAsync().Result;
            if (message_token.StatusCode != HttpStatusCode.OK)
                return res;

            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
            return json.Where(t => t.Key == "status").FirstOrDefault().Value.ToString();
        }
    }
}
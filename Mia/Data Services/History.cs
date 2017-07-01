using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Mia.DataServices
{
    public class History
    {
        public static async Task<List<Dictionary<string,object>>> GetHistory(DateTime reqdt)
        {
            HttpClient wc = new HttpClient();
            string URL = "http://history.muffinlabs.com/date/" + reqdt.Month.ToString() + "/" + reqdt.Day.ToString();
            string result =await wc.GetStringAsync(URL);
            Dictionary<string,object> values= JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
            values = JsonConvert.DeserializeObject<Dictionary<string, object>>(values["data"].ToString());
            var strEvents = values["Events"].ToString();
            var events = JsonConvert.DeserializeObject<List<Dictionary<string,object>>>(strEvents);
            return events;
        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace Mia.DataServices
{
    public class Knowledge
    {
        private static string WAK2 = "LKLTPK-98QWG32A5X";
        private static string WAK1 = "U8UY5L-WAAGPAARQU";
        private static string KGK = "AIzaSyACbHtnNF5jshputRfDFH_BfJWPgWSN0n0";
        private static async Task<string> LocationParameter()
        {
            string loc = await Location.GetCurrentLocationAsync();
            if (loc == "autoip") return null;
            else loc = "&latlong=" + loc;
            return loc;
        }
        public static async Task<Dictionary<string,string>> GetGoogleKnowledge(string s)
        {
            string url = "https://kgsearch.googleapis.com/v1/entities:search?query=" + s + "&key="+KGK+"&limit=1&indent=True";
			HttpClient client = new HttpClient();
			var responseFromServer = await client.GetStringAsync(url);
			JObject parseJson = JObject.Parse(responseFromServer);
			var getlist = parseJson["itemListElement"];
			var getdetail = getlist[0]["result"]["detailedDescription"]["articleBody"];
			string result = getdetail.ToString();
			int count = 0;
			Dictionary<string, string> final = new Dictionary<string, string>();
			var imageurl = getlist[0]["result"]["image"]["contentUrl"].ToString();
			final.Add("imageurl", imageurl);
			for (int i = 0; i < result.Length; i++)
			{
				if (result[i] == '.') count++;
				if (count == 3)
				{
					final.Add("data", AccentsRemover.RemoveAccents(result.Remove(i + 1)));

					return final;
				}
			}
			final.Add("data", AccentsRemover.RemoveAccents(result));

			return final;
        }
        public static async Task<Dictionary<string, object>> GetKnowledge(string q,double FontSize,double Width)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            q = WebUtility.UrlEncode(q);
            string url = @"http://api.wolframalpha.com/v1/spoken?input=" + q + "&appid=" + WAK2+await LocationParameter();
            HttpClient client = new HttpClient();
            string responseFromServer;
            try
            {
                responseFromServer = await client.GetStringAsync(url);
            }
            catch(System.Net.Http.HttpRequestException e) 
            {
                responseFromServer = "Let see what I found on the web for you.";
            }
            if (responseFromServer != null && responseFromServer.Contains("Information about"))
            {
                responseFromServer = responseFromServer.Replace("Information about", "");

                var kg = await GetGoogleKnowledge(responseFromServer);
                if (kg != null)
                {
                    responseFromServer = kg["data"];
                    result.Add("imageurl", kg["imageurl"]);
                }
                
            }
            result.Add("response",AccentsRemover.RemoveAccents( responseFromServer));
            
            

            string uri = (@"http://api.wolframalpha.com/v2/simple?input=" + q + "&appid=" + WAK1 + "&fontsize="+(int)FontSize+"&width="+(int)Width+await LocationParameter());
            Stream simpleImage;
            try
            {
                simpleImage = await client.GetStreamAsync(uri);
            }
            catch{
                return null;
            }
            if (simpleImage == null)
                return null;
            result.Add("simpleimage",simpleImage); 
            return result;
        }
    }
}

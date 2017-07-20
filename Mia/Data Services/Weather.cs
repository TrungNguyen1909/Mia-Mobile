using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading;
using System.Net.Http;
using System.Globalization;

namespace Mia.DataServices
{
    public class Weather
    {
        static string wunderground_key = "5d6c7e2305c51574"; // You'll need to goto http://www.wunderground.com/weather/api/, and get a key to use the API.

        public static async Task<Dictionary<string, object>> GetCurrentWeather(string location = "autoip")
        {

            var cli = new HttpClient();
            if (location != "autoip")
            {
                location = await AutoComplete(location);
            }
            else
            {
                location=await Location.GetCurrentLocationAsync();
            }
            string weather = await cli.GetStringAsync("http://api.wunderground.com/api/" + wunderground_key + "/conditions/q/" + location + ".json");

            Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(weather);
            values = JsonConvert.DeserializeObject<Dictionary<string, object>>(values["current_observation"].ToString());
            string iconurl = values["icon_url"].ToString();
            values["icon"] = GetWeatherAnimation(values["icon"].ToString(), iconurl.Contains("nt_"), Convert.ToDouble(values["temp_c"]));
            return values;
        }
        public static async Task<Dictionary<string, object>> GetForecastWeather(DateTime reqdt, string location = "autoip")
        {
            var cli = new HttpClient();
            if (location != "autoip")
            {
                location = await AutoComplete(location);
            }
            else
            {
                location = await Location.GetCurrentLocationAsync();
            }
            string forecast = await cli.GetStringAsync("http://api.wunderground.com/api/" + wunderground_key + "/forecast10day/q/" + location + ".json");

            Dictionary<string, object> fvalues = JsonConvert.DeserializeObject<Dictionary<string, object>>(forecast);
            fvalues = JsonConvert.DeserializeObject<Dictionary<string, object>>(fvalues["forecast"].ToString());
            fvalues = JsonConvert.DeserializeObject<Dictionary<string, object>>(fvalues["simpleforecast"].ToString());
            var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(fvalues["forecastday"].ToString());
            foreach (var subdata in list)
            {
                var converted = JsonConvert.DeserializeObject<Dictionary<string, object>>(subdata["date"].ToString());
                var Year = Convert.ToInt32(converted["year"]);
                var Month = Convert.ToInt32(converted["month"]);
                var Day = Convert.ToInt32(converted["day"]);
                var parsed = new DateTime(Year, Month, Day);

                subdata["date"] = parsed.ToString("D");
                var low = JsonConvert.DeserializeObject<Dictionary<string, object>>(subdata["low"].ToString());
                var high = JsonConvert.DeserializeObject<Dictionary<string, object>>(subdata["high"].ToString());
                subdata.Add("low_c", low["celsius"]);
                subdata.Add("high_c", high["celsius"]);
                subdata.Add("low_f", low["fahrenheit"]);
				subdata.Add("high_f", high["fahrenheit"]);
                subdata["icon"] = GetWeatherAnimation(subdata["icon"].ToString(), subdata["icon_url"].ToString().Contains("nt_"), 30);
                if (parsed.Date == reqdt)
                    return subdata;
            }
            return null;
        }
        public static async Task<string> AutoComplete(string s)
        {
            string location = "autoip";
            var cli = new HttpClient();
            string SelectedLocation = await cli.GetStringAsync("http://autocomplete.wunderground.com/aq?format=xml&query=" + s);
            using (XmlReader reader = XmlReader.Create(new StringReader(SelectedLocation)))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.Equals("name"))
                            {
                                reader.Read();
                                return location = reader.Value;
                            }
                            break;
                    }
                }
            }
            return s;
        }
        public static string GetWeatherAnimation(string s,bool isNightTime,double temp=30)
        {
            s = s.Replace("chance", "");
            if (s == "mostlysunny") s= "partlycloudy";
            if (s == "mostlycloudy") s="cloudy";
            if (s == "hazy") s = "fog";
            if (s == "sleet" || s == "flurries") s = "snow";
            if (s == "clear")
            {
                return s;
            }
            if (s == "sunny" && temp > 35) return "hot";
            //Day
            if (!isNightTime)
            {
                s += "_day";
            }
            else s += "_night";
            return (s);
        }
        
    }
}


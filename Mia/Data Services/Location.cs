using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Plugin.Geolocator;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Mia.DataServices
{
    public class Location
    {
        public static async Task<string> GetCurrentLocationAsync()
        {
            try
            {
                var locator = CrossGeolocator.Current;

                locator.DesiredAccuracy = 100;
                var coord = await locator.GetPositionAsync();

                if (coord != null)
                {
                    return (coord.Latitude.ToString() + "," + coord.Longitude.ToString());
                }
                else
                {
                    return "autoip";
                }
            }
            catch { return "autoip"; }

        }
        public static async Task<string> GetCurrentAddressAsync()
        {
            var latlong = await GetCurrentLocationAsync();
            if (latlong != "autoip")
            {
                var strLatitude = latlong.Substring(0, latlong.IndexOf(','));
                strLatitude = strLatitude.Trim(',');
                var strLongitude = latlong.Substring(latlong.IndexOf(','));
                strLongitude = strLongitude.TrimStart(',');
                string Url = "http://maps.googleapis.com/maps/api/geocode/json?latlng=" + strLatitude + "," + strLongitude + "&sensor=true";
                var client = new HttpClient();
                var getResult = await client.GetStringAsync(Url);
                JObject parseJson = JObject.Parse(getResult);
                var getJsonres = parseJson["results"][0];
                var getAddress = getJsonres["formatted_address"];
                string Address = AccentsRemover.RemoveAccents(getAddress.ToString());
                return Address;
            }
            else return null;
        }
    }
}

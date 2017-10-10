using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace D365Functions
{
    public static class Geocode
    {
        [FunctionName("Geocode")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            dynamic data = await req.Content.ReadAsAsync<object>();
            String address1 = data.address1;
            String city = data.city;
            String state = data.state;
            String zip = data.zip;

            var address = CreateAddress(address1, city, state, zip);

            var xdoc = GeocodeRequest(address);

            if (!ParseGeocodeResponse(xdoc, out var latitude, out var longitude))
                return req.CreateResponse(HttpStatusCode.ServiceUnavailable, "Geocode Error");

            GeocodeResponse response = new GeocodeResponse
            {
                Latitude = latitude,
                Longitude = longitude
            };

            var r = req.CreateResponse(HttpStatusCode.OK);
            r.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json");
            return r;
        }

        private static bool ParseGeocodeResponse(XDocument xdoc, out decimal? latitude, out decimal? longitude)
        {
            latitude = null;
            longitude = null;

            var xElement = xdoc.Element("GeocodeResponse");

            var result = xElement?.Element("result");

            var element = result?.Element("geometry");

            var locationElement = element?.Element("location");
            if (locationElement == null)
                return false;

            var latElement = locationElement.Element("lat");
            if (latElement != null)
                latitude = decimal.Parse(latElement.Value);

            var lngElement = locationElement.Element("lng");
            if (lngElement != null)
                longitude = decimal.Parse(lngElement.Value);

            return true;
        }

        private static XDocument GeocodeRequest(string address)
        {
            var requestUri = $"http://maps.googleapis.com/maps/api/geocode/xml?address={Uri.EscapeDataString(address)}&sensor=false";

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            return xdoc;
        }

        private static string CreateAddress(string address1, string city, string state, string zip)
        {
            string address = address1;

            if (!string.IsNullOrEmpty(city))
                address += " " + city;

            if (!string.IsNullOrEmpty(state))
                address += " " + state;

            if (!string.IsNullOrEmpty(zip))
                address += " " + zip;

            return address;
        }
    }
}

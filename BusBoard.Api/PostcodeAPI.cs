using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace BusBoard.Api
{
    public class PostCodeAPI
    {
        public string postcode { get; }
        public RestClient PCclient = new RestClient("https://api.postcodes.io");
        public string response { get; set; }

        public PostcodeLocation ReturnPostCode(string postcode)
        {
            RestRequest PCrequest = new RestRequest("/postcodes/" + postcode + "", Method.GET);
            var response = PCclient.Execute<PostCodeFields>(PCrequest);
            return response.Data.result;
        }
    }
}

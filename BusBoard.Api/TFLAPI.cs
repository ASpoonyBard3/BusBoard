using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace BusBoard.Api
{
    public class TFLAPI
    {
    public RestClient client = new RestClient("https://api.tfl.gov.uk");
    private HttpBasicAuthenticator Authentication = new HttpBasicAuthenticator("6d4298b1", "63f65d0071453ee130f132a483601da1");

    public string GetBusStopCodeForLocationFromTFL(float lat, float lon)
    {
        //method to return buses stops in lat and lon area.
        var request = new RestRequest("StopPoint?stopTypes=NaptanPublicBusCoachTram&returnLines=true&lat=" + lat + "&lon=" + lon + "", Method.GET);
        var response = client.Execute<StopPointDetails>(request);
        return response.Data.stopPoints.First().naptanId;
    }

    //method to return to main and give the ordered list of bus arrives
    public IEnumerable<ArrivalPrediction> GetBusStopArrivalTimesFromTFL(string busStop)
    {
        var request = new RestRequest("StopPoint/" + busStop + "/Arrivals", Method.GET);
        var response = client.Execute<List<ArrivalPrediction>>(request);
        var arrivals = response.Data;
        var OrderedArrivals = arrivals.OrderBy(arrival => arrival.TimeToStation).Take(5);
        return OrderedArrivals;
    }
}
}

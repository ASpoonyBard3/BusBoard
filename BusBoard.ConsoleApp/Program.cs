using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using System.Collections;

namespace BusBoard.ConsoleApp
{
    class Program
    {
        private static string AskUserForPostcode()
        {
            Console.WriteLine("Please enter your postcode.");
            string postCode = Console.ReadLine();
            return postCode;
        }

        private static PostcodeLocation GetPostcodeLocationFromPostcodeAPI(string postcode)
        {
            var PCclient = new RestClient("https://api.postcodes.io");
            var PCrequest = new RestRequest("/postcodes/" + postcode + "", Method.GET);
            var response = PCclient.Execute<PostCodeFields>(PCrequest);
            return response.Data.result;
        }

        private static string GetBusStopCodeForLocationFromTFL(float lat,  float lon)
        {
            var client = new RestClient("https://api.tfl.gov.uk");
            var request = new RestRequest("StopPoint?stopTypes=NaptanPublicBusCoachTram&returnLines=true&lat=" + lat + "&lon=" + lon + "", Method.GET);
            request.AddParameter("app_id", "6d4298b1");
            request.AddParameter("app_key", "63f65d0071453ee130f132a483601da1");
            var response = client.Execute<StopPointDetails>(request);
            return response.Data.stopPoints.First().naptanId;
        }
        private static IEnumerable<ArrivalPrediction> GetBusStopArrivalTimesFromTFL(string busStop)
        {
            var client = new RestClient("https://api.tfl.gov.uk");
            client.Authenticator = new HttpBasicAuthenticator("6d4298b1", "63f65d0071453ee130f132a483601da1");
            var request = new RestRequest("StopPoint/" + busStop + "/Arrivals", Method.GET);
            var response = client.Execute<List<ArrivalPrediction>>(request);
            var arrivals = response.Data;
            var OrderedArrivals = arrivals.OrderBy(arrival => arrival.TimeToStation).Take(5);
            return OrderedArrivals;
        }
        private static void PrintArrivalTimesToConsole(IEnumerable<ArrivalPrediction> arrivalTimes)
        {
            foreach (var Arrival in arrivalTimes)
            {
                Console.WriteLine(Arrival.Id);
                Console.WriteLine(Arrival.StationName);
                Console.WriteLine("This bus is due in {0}", Arrival.GetArrivalTime());
            }
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            var postcode = AskUserForPostcode();
            var postcodeLocation = GetPostcodeLocationFromPostcodeAPI(postcode);
            var busStopCode = GetBusStopCodeForLocationFromTFL(postcodeLocation.latitude, postcodeLocation.longitude);
            var arrivalTimes = GetBusStopArrivalTimesFromTFL(busStopCode);
            PrintArrivalTimesToConsole(arrivalTimes);

        }
        
    }
}

public class ArrivalPrediction
{
    public string Id { get; set; }
    public string StationName { get; set; }
    public int TimeToStation { get; set; }

    public string GetArrivalTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(TimeToStation);
        String stringTime = time.ToString(@"\:mm\:ss");
        return stringTime;
    }
}


public class PostCodeFields
{
    public string status { get; set; }
    public PostcodeLocation result { get; set; }
}
public class PostcodeLocation
{
    public float longitude { get; set; }
    public float latitude { get; set; }
}

public class StopPointDetails
{
    public List<float> centrePoint { get; set; }
    public List<SingleStopPoint> stopPoints { get; set; }
}
public class SingleStopPoint
{
    public string naptanId { get; set; }
}



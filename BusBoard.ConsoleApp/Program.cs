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
            client.Authenticator = new HttpBasicAuthenticator("6d4298b1", "63f65d0071453ee130f132a483601da1");
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


            //Console.WriteLine("Please enter your bus stop code.");
            //string busStop = Console.ReadLine();

            //Console.WriteLine("Please enter your postcode.");
            //string postCode = Console.ReadLine();

            //BusStopAPI(busStop, out RestClient client, out RestRequest request);

            //PostCodeAPI(postCode, out RestClient PCclient, out RestRequest PCrequest);

            //IRestResponse<PostCodeFields> response = client.Execute<PostCodeFields>(request);
            //var postcodeLocation = response.Data;

            //IEnumerable<ArrivalPrediction> limitedArrivals = OrderedByArrivals(response);

            //foreach (var Arrival in limitedArrivals)
            //{
            //    Console.WriteLine(Arrival.Id);
            //    Console.WriteLine(Arrival.StationName);
            //    Console.WriteLine("This bus is due in {0}", Arrival.GetArrivalTime());
            //}
            //Console.ReadLine();
        }

        private static IEnumerable<ArrivalPrediction> OrderedByArrivals(IRestResponse response)
        {
            List<ArrivalPrediction> ArrivalPredictions = JsonConvert.DeserializeObject<List<ArrivalPrediction>>(response.Content);
            IEnumerable<ArrivalPrediction> orderedArrivals = ArrivalPredictions.OrderBy(ap => ap.TimeToStation); //lambda expression.
            var limitedArrivals = orderedArrivals.Take(5);//Take(n) returns the specified amount of objects from the list.
            return limitedArrivals; //returns the limited arrivals Enum to the main function, as method is not void 
        }

        private static void BusStopAPI(string busStop, string postcodeLocation, out RestClient client, out RestRequest request)
        {
            client = new RestClient("https://api.tfl.gov.uk");
            request = new RestRequest("StopPoint/" + busStop + "/StopPoint_GetByGeoPoint", Method.GET);
            request = new RestRequest("StopPoint?stopTypes=NaptanPublicBusCoachTram&returnLines=true&lat=" + postcodeLocation + "&lon=" + postcodeLocation + "", Method.GET);

            client.Authenticator = new HttpBasicAuthenticator("6d4298b1", "63f65d0071453ee130f132a483601da1");
        }

        private static void PostCodeAPI(string postCode, out RestClient PCclient, out RestRequest PCrequest)
        {
            PCclient = new RestClient("https://api.postcodes.io");
            PCrequest = new RestRequest("/postcodes/" + postCode + "", Method.GET);
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



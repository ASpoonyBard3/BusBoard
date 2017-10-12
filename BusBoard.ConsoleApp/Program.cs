using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;
using RestSharp.Authenticators;
using BusBoard.Api;

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
        /*
        private static PostcodeLocation GetPostcodeLocationFromPostcodeAPI(string postcode)

        {
            var PCclient = new RestClient("https://api.postcodes.io");
            var PCrequest = new RestRequest("/postcodes/" + postcode + "", Method.GET);
            var response = PCclient.Execute<PostCodeFields>(PCrequest);
            return response.Data.result;
        }

        private static string GetBusStopCodeForLocationFromTFL(float lat, float lon)
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
        */
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
            var PostCodeQuery = new 
            PrintArrivalTimesToConsole(arrivalTimes);
        }
    }
}
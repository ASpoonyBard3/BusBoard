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
        static void Main(string[] args)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            Console.WriteLine("Please enter your bus stop code.");
            var busStop = Console.ReadLine();

            BusStopAPI(busStop, out RestClient client, out RestRequest request);
            IRestResponse response = client.Execute(request);

            IEnumerable<ArrivalPrediction> limitedArrivals = OrderedByArrivals(response);

            foreach (var Arrival in limitedArrivals)
            {
                Console.WriteLine(Arrival.Id);
                Console.WriteLine(Arrival.StationName);
                Console.WriteLine("This bus is due in {0}", Arrival.GetArrivalTime());
            }
            Console.ReadLine();
        }

        private static IEnumerable<ArrivalPrediction> OrderedByArrivals(IRestResponse response)
        {
            List<ArrivalPrediction> ArrivalPredictions = JsonConvert.DeserializeObject<List<ArrivalPrediction>>(response.Content);
            IEnumerable<ArrivalPrediction> orderedArrivals = ArrivalPredictions.OrderBy(ap => ap.TimeToStation); //lambda expression.
            var limitedArrivals = orderedArrivals.Take(5);
            return limitedArrivals;
        }

        private static void BusStopAPI(string busStop, out RestClient client, out RestRequest request)
        {
            client = new RestClient("https://api.tfl.gov.uk");
            request = new RestRequest("StopPoint/" + busStop + "/Arrivals", Method.GET);
            client.Authenticator = new HttpBasicAuthenticator("6d4298b1", "63f65d0071453ee130f132a483601da1");
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
        String stringTime = time.ToString(@"hh\:mm\:ss");
        return stringTime;
    }
}

    






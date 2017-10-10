using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;

namespace BusBoard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            Console.WriteLine("Please enter your bus stop code.");
            var busStop = Console.ReadLine();
            //
            BusStopProxyAPI busStopAPI = new BusStopProxyAPI("6d4298b1", "63f65d0071453ee130f132a483601da1");

            var client = new RestClient("https://api.tfl.gov.uk");
            var request = new RestRequest("StopPoint/490008660N/Arrivals", Method.GET);

            client.Authenticator = new HttpBasicAuthenticator("6d4298b1", "63f65d0071453ee130f132a483601da1");
            //var request = new RestResponse();

            IRestResponse response = client.Execute(request);

            List<ArrivalPredictions> ArrivalPredictions = JsonConvert.DeserializeObject<List<ArrivalPredictions>>(response.Content);

            foreach (var Arrival in ArrivalPredictions)
            {
                Console.WriteLine(Arrival.TimeToStation);
            }

            Console.ReadLine();
        }
    }
    /*
    public static Task<IRestResponse> GetRestResponseContentAsync(RestClient theClient, RestRequest theRequest)
    {
        var tcs = new TaskCompletionSource<IRestResponse>();
        theClient.ExecuteAsync(theRequest, theResponse =>
        {
            tcs.SetResult(theResponse);
        });
        return tcs.Task;
    }
    */

    //proxy class as restsharp api asks
    public class BusStopProxyAPI
    {
        const string BaseUrl = "https://api.tfl.gov.uk/StopPoint/490008660N/Arrivals";

        private readonly string _accountSid;
        private readonly string _secretKey;

        public BusStopProxyAPI(string appID, string appKey)
        {
            _accountSid = appID;
            _secretKey = appKey;
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient
            {
                BaseUrl = new System.Uri(BaseUrl),
                Authenticator = new HttpBasicAuthenticator(_accountSid, _secretKey)
            };
            request.AddParameter("AccountSid", _accountSid, ParameterType.UrlSegment); //
            var response = client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info.";
                var twilioException = new ApplicationException(message, response.ErrorException);
                throw twilioException;
            }
            return response.Data;
        }
    }


    public class ArrivalPredictions
    {
        public string Id { get; set; }
        public string StationName { get; set; }
        public int TimeToStation { get; set; }

    }
}



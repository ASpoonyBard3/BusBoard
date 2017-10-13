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
            var PostLonLat = new PostCodeAPI().ReturnPostCode(postcode);
            var TFIGeoReturn = new TFLAPI().GetBusStopCodeForLocationFromTFL(PostLonLat.latitude, PostLonLat.longitude);
            var StopReturns = new TFLAPI().GetBusStopArrivalTimesFromTFL(TFIGeoReturn);
            PrintArrivalTimesToConsole(StopReturns);
        }
    }
}
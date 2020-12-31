using Clima.Contracts.Models;
using Meadow.Foundation.DataLoggers;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WildernessLabs.Clima.AdafruitIO.Tests
{
    public class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello from AdafruitIO test!");
            Console.WriteLine("Create the Adafruit IO Logger");

            // TODO: Change this to a factory pattern so we can jsut call
            // LoggerFactory.CreateLogger(LoggerType.AdafruitIO) were LoggerType is an enum of the logging types.
            // need to difine standard logging interface.
            Meadow.Foundation.DataLoggers.AdafruitIO logger =
                new Meadow.Foundation.DataLoggers.AdafruitIO(Secrets.IO_UserName, Secrets.IO_Key, "meadow-1");

            Console.WriteLine("Creating the dummy data to upload to Adafruit IO");
            DateTime loggedTime = DateTime.Now;
            SensorReading[] sensorReadings = new SensorReading[]
            {
                new SensorReading(Secrets.IO_FeedKeys[0].Key, 21.5m.ToString("N2"), loggedTime), // temperature [°C]
                new SensorReading(Secrets.IO_FeedKeys[1].Key, 88.2m.ToString("N2"), loggedTime), // humidity [%]
                new SensorReading(Secrets.IO_FeedKeys[2].Key, 99.5m.ToString("N2"), loggedTime)  // pressure [hPa]
            };

            Console.WriteLine("Writing to Adafruit IO using AdafruitIO.PostValues");
            logger.PostValues(sensorReadings);

            Console.WriteLine("Reading from Adafruit IO using AdafruitIO.PostValues...");
            AdafruitIOData[] feeds;

            Console.WriteLine("-------GetFeedDataAsync");
            feeds = GetFeedDataAsync(Secrets.IO_UserName, Secrets.IO_Key, "meadow-1.temperature").Result;
            PrintOutData(feeds);

            Console.WriteLine("-------GetPreviousDataAsync");
            feeds = GetPreviousDataAsync(Secrets.IO_UserName, Secrets.IO_Key, "meadow-1.temperature").Result;
            PrintOutData(feeds);

            Console.WriteLine("-------GetNextDataAsync");
            feeds = GetNextDataAsync(Secrets.IO_UserName, Secrets.IO_Key, "meadow-1.temperature").Result;
            PrintOutData(feeds);

            Console.WriteLine("-------GetLastDataAsync");
            feeds = GetLastDataAsync(Secrets.IO_UserName, Secrets.IO_Key, "meadow-1.temperature").Result;
            PrintOutData(feeds);

            Console.WriteLine("-------GetFirstDataAsync");
            feeds = GetFirstDataAsync(Secrets.IO_UserName, Secrets.IO_Key, "meadow-1.temperature").Result;
            PrintOutData(feeds);
        }

        public static async Task<AdafruitIOData[]> GetFeedDataAsync(string userName, string iO_Key, string feed_Key)
        {
            string uri = $"http://io.adafruit.com/api/v2/{userName}/feeds/{feed_Key}/data";
            return await GetAdafruitFeedAsync(uri, iO_Key);
        }

        public static async Task<AdafruitIOData[]> GetPreviousDataAsync(string userName, string iO_Key, string feed_Key)
        {
            string uri = $"http://io.adafruit.com/api/v2/{userName}/feeds/{feed_Key}/data/previous";
            return await GetAdafruitFeedAsync(uri, iO_Key);
        }

        public static async Task<AdafruitIOData[]> GetNextDataAsync(string userName, string iO_Key, string feed_Key)
        {
            string uri = $"http://io.adafruit.com/api/v2/{userName}/feeds/{feed_Key}/data/next";
            return await GetAdafruitFeedAsync(uri, iO_Key);
        }

        public static async Task<AdafruitIOData[]> GetLastDataAsync(string userName, string iO_Key, string feed_Key)
        {
            string uri = $"http://io.adafruit.com/api/v2/{userName}/feeds/{feed_Key}/data/last";
            return await GetAdafruitFeedAsync(uri, iO_Key);
        }

        public static async Task<AdafruitIOData[]> GetFirstDataAsync(string userName, string iO_Key, string feed_Key)
        {
            string uri = $"http://io.adafruit.com/api/v2/{userName}/feeds/{feed_Key}/data/first";
            return await GetAdafruitFeedAsync(uri, iO_Key);
        }

        public static async Task<AdafruitIOData[]> GetAllFeedsDataAsync(string userName, string iO_Key)
        {
            string uri = $"http://io.adafruit.com/api/v2/{userName}/feeds";
            return await GetAdafruitFeedAsync(uri, iO_Key);
        }

        private static async Task<AdafruitIOData[]> GetAdafruitFeedAsync(string uri, string iO_Key)
        {
            using HttpClient httpClient = new HttpClient { Timeout = new TimeSpan(0, 5, 0) };
            httpClient.DefaultRequestHeaders.Add("X-AIO-Key", iO_Key);
            HttpResponseMessage response = await httpClient.GetAsync(uri);

            try
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                AdafruitIOData[] returnData;
                if (json.Substring(0, 1) == "[") // an array of feeds was returned
                    returnData = JsonSerializer.Deserialize<AdafruitIOData[]>(json);
                else
                    returnData = new AdafruitIOData[] { JsonSerializer.Deserialize<AdafruitIOData>(json) };

                return returnData;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Request timed out.");
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Request went sideways: {e.Message}");
                return null;
            }
        }

        private static void PrintOutData(AdafruitIOData[] feeds)
        {
            foreach (var item in feeds)
            {
                Console.WriteLine($"ClimateReading:");
                Console.WriteLine($"\tId: {item.Id}");
                Console.WriteLine($"\tValue: {item.Value}");
                Console.WriteLine($"\tFeed ID: {item.FeedId}");
                Console.WriteLine($"\tFeed Key {item.FeedKey}");
                Console.WriteLine($"\tCreated At: {item.CreatedAt}");
            }
        }
    }
}

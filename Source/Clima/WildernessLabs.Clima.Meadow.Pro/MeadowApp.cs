using System;
using System.Threading.Tasks;
using Clima.Meadow.HackKit.Controllers;
using Clima.Meadow.HackKit.ServiceAccessLayer;
using Clima.Contracts.Models;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Gateway.WiFi;
using Meadow.Peripherals.Sensors.Atmospheric;
using Meadow.Foundation.DataLoggers;

namespace Clima.Meadow.HackKit
{
    public partial class MeadowApp : App<F7Micro, MeadowApp>
    {
        // Controllers
        DisplayController displayController;
        readonly ClimateReading climateReading = new ClimateReading();
        AdafruitIO logger;

        public MeadowApp()
        {
            InitializeHardware();
            Startup();





            


            //==== grab the climate readings
            Console.WriteLine("Fetching climate readings.");
            //ClimateServiceFacade.FetchReadings().Wait();

            //==== post the reading
            //Console.WriteLine("Posting the temp reading");
            //ClimateServiceFacade.PostTempReading((decimal)conditions.Temperature).Wait();
            //Console.WriteLine("Posted.");

            //==== fetch the readings again
            //Console.WriteLine("Fetching the readings agian.");
            //ClimateServiceFacade.FetchReadings().Wait();

            //==== farewell, Batty
            Console.WriteLine("All those moments will be lost in time, like tears in rain. Time to die.");

        }

        private void Startup()
        {
            Console.WriteLine($"Startup.");
            // Connect to network
            Console.WriteLine($"Connecting to Wifi Network {Secrets.WIFI_NAME} with password: {Secrets.WIFI_PASSWORD}");
            Console.WriteLine($"Wifi Scan freqency: {Device.WiFiAdapter.ScanFrequency}\tCount: {Device.WiFiAdapter.Networks.Count}");

            var result = Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);

            Console.WriteLine($"Connection status: {result.ConnectionStatus}");

            if (result.ConnectionStatus != ConnectionStatus.Success)
            {
                throw new Exception($"Cannot connect to network: {result.ConnectionStatus}");
            }
            Console.WriteLine($"Connected to {Secrets.WIFI_NAME}");

            // starting up the display
            displayController = new DisplayController(display);

            Console.WriteLine("Creating AdafruitIO logger");
            logger = new AdafruitIO(Secrets.IO_UserName, Secrets.IO_Key, "meadow-1");

            rgbLed.SetColor(RgbLed.Colors.Green);

            //==== take a set of atmospheric readings
            AtmosphericConditions conditions = new AtmosphericConditions();
            conditions = bme280.Read().Result;

            // === craate the timestamp
            DateTime atmoReadingTime = DateTime.Now;
            Console.WriteLine("Atmospheric Conditions:");
            Console.WriteLine($"\t{conditions.Temperature:0.0} C");
            Console.WriteLine($"\t{conditions.Humidity:0.0} %");
            Console.WriteLine($"\t{(conditions.Pressure / 1000):0.0} kPa");
            Console.WriteLine($"\t{atmoReadingTime:g}");

            // update the display with the current temperature
            Console.WriteLine("Updating display.");
            this.displayController.UpdateDisplay(conditions);

            rgbLed.SetColor(RgbLed.Colors.Magenta);
            // logging the climate data to AdafruitIO
            Console.WriteLine("creating the sensor reading data from the atmospheric conditions data");
            SensorReading[] sensorReadings = new SensorReading[]
            {
                new SensorReading(Secrets.IO_FeedKeys[0].Key,conditions.Temperature?.ToString(), atmoReadingTime ),
                new SensorReading(Secrets.IO_FeedKeys[1].Key,conditions.Humidity?.ToString(), atmoReadingTime),
                new SensorReading(Secrets.IO_FeedKeys[2].Key,conditions.Pressure?.ToString(), atmoReadingTime)
            };

            logger.PostValues(sensorReadings);
            rgbLed.SetColor(RgbLed.Colors.Green);

        }
    }
}
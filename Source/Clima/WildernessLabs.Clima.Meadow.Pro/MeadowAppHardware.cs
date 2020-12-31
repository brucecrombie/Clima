using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Hardware;
using Meadow.Foundation.RTCs;
using Meadow.Foundation.Sensors.Atmospheric;

namespace Clima.Meadow.HackKit
{
    public partial class MeadowApp
    {
        private RgbLed rgbLed;
        private Ds3231 rtc;
        private Bme280 bme280;
        // private AnalogTemperature temperature;
        private St7789 display;
        private void InitializeHardware()
        {
            Console.WriteLine("Initialize Hardware...");

            // initialize the onbaord rgb LED
            Console.WriteLine($"Initializing onboard rgb LED.");
            rgbLed = new RgbLed(device: Device,
                                redPin: Device.Pins.OnboardLedRed,
                                greenPin: Device.Pins.OnboardLedGreen,
                                bluePin: Device.Pins.OnboardLedBlue);
            rgbLed.SetColor(RgbLed.Colors.Red);

            // create the I2C bust that the real time clock and BME280 will use
            II2cBus i2cBus = Device.CreateI2cBus();

            // initialize the real time clock DS3232
            Console.WriteLine($"Initializing real time clock.");
            rtc = new Ds3231(device: Device,
                             i2cBus: i2cBus,
                             interruptPin: null);
            // rtc.CurrentDateTime = new DateTime(2020, 11, 22, 16, 54, 0);
            Device.SetClock(rtc.CurrentDateTime + new TimeSpan(8, 0, 0));
            Console.WriteLine($"Meadow UTC time set to {DateTime.Now}");

            // our display needs mode3
            var config = new SpiClockConfiguration(6000, SpiClockConfiguration.Mode.Mode3);

            // create the spi bus that the display will use
            ISpiBus spiBus = MeadowApp.Device.CreateSpiBus(clock: Device.Pins.SCK,
                                                           mosi: Device.Pins.MOSI,
                                                           miso: Device.Pins.MISO,
                                                           config: config);

            // new up the actual display on the SPI bus
            display = new St7789(device: Device,
                                 spiBus: spiBus,
                                 chipSelectPin: null,
                                 dcPin: Device.Pins.D01,
                                 resetPin: Device.Pins.D00,
                                 width: 240, height: 240);

            // initialize the Wifi adapter
            Console.WriteLine($"Initializing Wifi adapter.");
            Device.InitWiFiAdapter().Wait();

            // initalize analog BME280 atmospheric sensor
            Console.WriteLine("Initializing BME280 atmospheric sensor.");
            bme280 = new Bme280(i2c: i2cBus);

            rgbLed.SetColor(RgbLed.Colors.Yellow);
        }
    }
}

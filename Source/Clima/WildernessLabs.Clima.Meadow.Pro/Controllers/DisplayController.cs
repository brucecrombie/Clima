using Meadow.Foundation;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors.Atmospheric;
using SimpleJpegDecoder;
using System;
using System.IO;
using System.Reflection;

namespace Clima.Meadow.HackKit.Controllers
{
    public class DisplayController
    {
        #region Private Fields

        private readonly St7789 display;
        private GraphicsLibrary graphicsLibrary;
        private AtmosphericConditions conditions;

        // rendering state and lock
        private bool isRendering = false;
        private readonly object renderLock = new object();

        #endregion Private Fields

        #region Constructor(s)

        public DisplayController(St7789 display)
        {
            this.display = display;
            InitializeDisplay();
        }

        #endregion Constructor(s)

        #region Public Methods

        public void UpdateDisplay(AtmosphericConditions conditions)
        {
            this.conditions = conditions;
            Render();
        }

        #endregion Public Methods

        #region Private Methoods

        /// <summary>
        ///  Initialize the backing graphics library.
        /// </summary>
        protected void InitializeDisplay()
        {
            // create our graphics surface that we'll draw onto and then blit
            // to the display with.
            graphicsLibrary = new GraphicsLibrary(display) { CurrentFont = new Font12x20() };

            Console.WriteLine("Clear display");
            graphicsLibrary.Clear(true);
        }

        /// <summary>
        /// Does the actual rendering. If it's already rendering, it'll bail out,
        /// so render requests don't stack up.
        /// </summary>
        protected void Render()
        {
            Console.WriteLine($"Render() - is rendering: {isRendering}");

            lock (renderLock)
            {
                if (isRendering)
                {
                    Console.WriteLine("Already in a rendering loop, bailing out.");
                    return;
                }

                isRendering = true;
            }

            graphicsLibrary.Clear(true);

            graphicsLibrary.Stroke = 1;
            graphicsLibrary.DrawRectangle(xLeft: 0,
                                          yTop: 0,
                                          width: (int)display.Width,
                                          height: (int)display.Height,
                                          color: Color.White);
            graphicsLibrary.DrawRectangle(xLeft: 5,
                                          yTop: 5,
                                          width: (int)display.Width - 10,
                                          height: (int)display.Height - 10,
                                          color: Color.White);
            graphicsLibrary.DrawCircle(centerX: (int)display.Width / 2,
                                       centerY: (int)display.Height / 2,
                                       radius: (int)display.Width / 2,
                                       color: Color.FromHex("#23abe3"),
                                       filled: true);
            DisplayJPG(50, 40);

            string tempText = $"{conditions.Temperature?.ToString("##.#")}°C";
            string humidityText = $"{conditions.Humidity?.ToString("##.#")}% rh";
            string pressureText = $"{(conditions.Pressure/1000f)?.ToString("####.0")} kPa";

            graphicsLibrary.CurrentFont = new Font12x20();
            graphicsLibrary.DrawText(x: (int)(display.Width - (tempText.Length * 24)) / 2,
                                      y: 120,
                                      text: tempText,
                                      color: Color.Black,
                                      scaleFactor: GraphicsLibrary.ScaleFactor.X2);

            graphicsLibrary.CurrentFont = new Font8x12();
            graphicsLibrary.DrawText(x: (int)(display.Width - (humidityText.Length * 16)) / 2,
                                      y: 160,
                                      text: humidityText,
                                      color: Color.Black,
                                      scaleFactor: GraphicsLibrary.ScaleFactor.X2);

            graphicsLibrary.CurrentFont = new Font8x12();
            graphicsLibrary.DrawText(x: (int)(display.Width - (pressureText.Length * 16)) / 2,
                                      y: 185,
                                      text: pressureText,
                                      color: Color.Black,
                                      scaleFactor: GraphicsLibrary.ScaleFactor.X2);

            graphicsLibrary.Rotation = GraphicsLibrary.RotationType._270Degrees;

            graphicsLibrary.Show();
            Console.WriteLine("Show complete");

            isRendering = false;
        }

        protected void DisplayJPG(int locX, int locY)
        {
            var jpgData = LoadResource("meadow.jpg");
            var decoder = new JpegDecoder();
            var jpg = decoder.DecodeJpeg(jpgData);

            int x = 0;
            int y = 0;
            byte r, g, b;

            for (int i = 0; i < jpg.Length; i += 3)
            {
                r = jpg[i];
                g = jpg[i + 1];
                b = jpg[i + 2];

                graphicsLibrary.DrawPixel(x + locX, y + locY, Color.FromRgb(r, g, b));

                x++;
                if (x % decoder.Width == 0)
                {
                    y++;
                    x = 0;
                }
            }

            display.Show();
        }

        protected byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Clima.Meadow.HackKit.{filename}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
        #endregion Private Methoods
    }
}
using System.Threading;
using QldEd.MonoPi.GPIO;
using QldEd.MonoPi.GPIO.Pinouts;

namespace QldEd.MonoPi.Demo
{
    public static class Program
    {
        private static void Main()
        {
            // get a GPIO pin ready for output
            var pin = Pi3Pins.Gpio17.Prepare().AsOutput();
            
            // toggle the pin on and off, e.g. flash an led
            for (var i = 0; i < 100; i++)
            {
                pin.Toggle();
                Thread.Sleep(500);
            }
        }
    }
}

using System.Threading;
using QldEd.MonoPi.GPIO;
using QldEd.MonoPi.GPIO.Helpers;
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

            var shiftRegisters = new ShiftRegister(
                Pi3Pins.Gpio17.Prepare(),
                Pi3Pins.Gpio27.Prepare(),
                Pi3Pins.Gpio22.Prepare(),
                Pi3Pins.Gpio18.Prepare(),
                Pi3Pins.Gpio23.Prepare(),
                3);

            shiftRegisters[0] = true; // turns on Pin 15 on the first chip immediately 
            shiftRegisters[1] = true; // turns on Pin 1 on the first chip immediately

            shiftRegisters.AutoCommit = false; // disabled AutoCommit to set multiple values at once
            shiftRegisters[8] = true; // Pin 15 on the second chip
            shiftRegisters[16] = true; // Pin 15 on the third chip
            shiftRegisters.Commit(); // turns on both pins simulatniously

            shiftRegisters.Clear();
        }
    }
}

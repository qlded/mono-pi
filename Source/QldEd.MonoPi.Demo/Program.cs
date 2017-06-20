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
            FlashAnLed();
            ShiftRegisterCounter();
        }

        private static void FlashAnLed()
        {
            // get a GPIO pin ready for output
            var pin = Pi3Pins.Gpio25.Prepare().AsOutput();

            // toggle the pin on and off, e.g. flash an led
            for (var i = 0; i < 10; i++)
            {
                pin.Toggle();
                Thread.Sleep(500);
            }
        }

        private static void ShiftRegisterCounter()
        {
            var shiftRegisters = new ShiftRegister(
                Pi3Pins.Gpio17.Prepare(), // signal (SER - chip pin 14)
                Pi3Pins.Gpio27.Prepare(), // signal clock (SRCLK - chip pin 11)
                Pi3Pins.Gpio22.Prepare(), // register clock (RCLK - chip pin 12)
                Pi3Pins.Gpio18.Prepare(), // clear (SRCLR - chip pin 10 - Important: tie this high if not connected to Pi!)
                Pi3Pins.Gpio23.Prepare(), // enabled (OE - chip pin 13)
                3); // 3 chips to demo chaining, see readme.md

            for (var i = 0; i < 8; i++) // demo looping through first chips outputs
            {
                shiftRegisters.Clear();
                shiftRegisters[i] = true;
                Thread.Sleep(1000);
            }

            shiftRegisters.Clear();
            shiftRegisters[0] = true; // turns on Pin 15 on the first chip immediately 
            Thread.Sleep(100);
            shiftRegisters[1] = true; // turns on Pin 1 on the first chip immediately
            Thread.Sleep(100);
            shiftRegisters[2] = true; // turns on Pin 2 on the first chip immediately
            Thread.Sleep(100);
            shiftRegisters[3] = true; // turns on Pin 3 on the first chip immediately
            
            shiftRegisters.AutoCommit = false; // disabled AutoCommit to set multiple values at once
            shiftRegisters.Clear();
            shiftRegisters[4] = true; // turns on Pin 4 on the first chip immediately 
            Thread.Sleep(100);
            shiftRegisters[5] = true; // turns on Pin 5 on the first chip immediately
            Thread.Sleep(100);
            shiftRegisters[6] = true; // turns on Pin 6 on the first chip immediately
            Thread.Sleep(100);
            shiftRegisters[7] = true; // turns on Pin 7 on the first chip immediately
            Thread.Sleep(100);
            shiftRegisters[8] = true; // Pin 15 on the second chip
            Thread.Sleep(100);
            shiftRegisters[16] = true; // Pin 15 on the third chip
            shiftRegisters.Commit(); // turns on the above pins simultaneously
        }
    }
}

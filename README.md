# QldEd.MonoPi
C# Mono Library for various single board computers such as the RaspberryPi.

# Aim
The aim of this project is to create a simple and clean class library for 
accessing various Pi functionality from mono. Ideally the library will have no
third party dependencies.

The library will provide access to GPIO pins and logic for basic
bit banging operations such as I2C and shift registers.

Primarily this project is for my own education and fun but I aim to comment
the code well for others to follow and learn.

# Current Functionality
## Basic GPIO
```csharp
            // get a GPIO pin ready for output
            var pin = Pi3Pins.Gpio17.Prepare().AsOutput();
            
            // toggle the pin on and off, e.g. flash an led
            for (var i = 0; i < 100; i++)
            {
                pin.Toggle();
                Thread.Sleep(500);
            }
```
Pinouts for Pi1, Pi2, Pi3 and BananaPi (easy to add your own).

## SN74HC595N Shift Register
 ```csharp
            /// To connect multiple chips in series connect
            /// Pi                     Chip 1      Chip 2      Chip 3
            /// signalPin           -> Pin 14
            /// signalClockPin      -> Pin 11   -> Pin 11   -> Pin 11
            /// registerClockPin    -> Pin 12   -> Pin 12   -> Pin 12
            /// clearPin            -> Pin 10   -> Pin 10   -> Pin 10
            /// outputEnabledPin    -> Pin 13   -> Pin 13   -> Pin 13
            ///                        Pin 9    -> Pin 14
            ///                                    Pin 9    -> Pin 14
            
            var shiftRegisters = new ShiftRegister(
                Pi3Pins.Gpio17.Prepare(), // signalPin
                Pi3Pins.Gpio27.Prepare(), // signalClockPin
                Pi3Pins.Gpio22.Prepare(), // registerClockPin
                Pi3Pins.Gpio18.Prepare(), // clearPin
                Pi3Pins.Gpio23.Prepare(), // outputEnabledPin
                3);

            shiftRegisters[0] = true; // turns on Pin 15 on the first chip immediately 
            shiftRegisters[1] = true; // turns on Pin 1 on the first chip immediately

            shiftRegisters.AutoCommit = false; // disabled AutoCommit to set multiple values at once
            shiftRegisters[8] = true; // Pin 15 on the second chip
            shiftRegisters[16] = true; // Pin 15 on the third chip
            shiftRegisters.Commit(); // turns on both pins simultaneously

            shiftRegisters.Clear();
```

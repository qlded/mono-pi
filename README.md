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
Basic GPIO.

            // get a GPIO pin ready for output
            var pin = Pi3Pins.Gpio17.Prepare().AsOutput();
            
            // toggle the pin on and off, e.g. flash an led
            for (var i = 0; i < 100; i++)
            {
                pin.Toggle();
                Thread.Sleep(500);
            }

Pinouts for Pi1, Pi2, Pi3 and BananaPi (easy to add your own).

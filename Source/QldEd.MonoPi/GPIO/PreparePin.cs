using System;
using System.IO;
using System.Threading;

namespace QldEd.MonoPi.GPIO
{
    /// <summary>
    /// Represents a GPIO pin that is ready for use
    /// 
    /// Based on: https://www.kernel.org/doc/Documentation/gpio/sysfs.txt
    /// 
    /// This is a minimal implementation intended for simply turning the pin
    /// on/off or reading it's current value. If you need access to other 
    /// functionality such as edge, active_low, unexport create your own subclass.
    /// 
    /// To acquire an instance of PreparedPin use either:
    ///     1. the extension methods for the Pin enums e.g.
    ///         var pin = Pi3Pins.Gpio23.Prepare();
    ///     2. the static prepare method with a known pin number e.g.
    ///         var pin = PreparedPin.Prepare(42); // Pi 4 with 80 pin header?
    /// </summary>
    public class PreparedPin
    {
        private const string Dir = "/sys/class/gpio";

        private readonly int _pin;

        private bool _isOutput;
        private bool _lastOutputValue;

        private PreparedPin(int pin)
        {
            _pin = pin;
        }

        /// <summary>
        /// Exports the pin to user space and once it is ready returns an instance
        /// </summary>
        /// <param name="pinNumber">the GPIO number of the pin (not the physical pin number)</param>
        /// <returns>an object for simple control of the pin</returns>
        public static PreparedPin Prepare(int pinNumber)
        {
            // if the pin isn't already exported to user space
            if (!Directory.Exists($"{Dir}/gpio{pinNumber}"))
            {
                // export it ready for use
                using (var writer = new StreamWriter($"{Dir}/export", false))
                {
                    writer.Write(pinNumber);
                }
            }

            var i = 0;
            // try a safe number of times to access the new pin
            while (i++ < 3)
            {
                try
                {
                    // try to update the direction to check that the pin is ready for use
                    File.AppendAllText($"{Dir}/gpio{pinNumber}/direction", "");

                    // return a pin that is ready for use
                    return new PreparedPin(pinNumber);
                }
                catch (UnauthorizedAccessException)
                {
                    // slight delay before retrying due to permissions issue for non-root users 
                    Thread.Sleep(10);
                }
            }

            // we failed to initialise the pin correctly
            throw new UnauthorizedAccessException($"Unable to access pin {pinNumber}");
        }

        private void Write(bool value)
        {
            using (var writer = new StreamWriter($"{Dir}/gpio{_pin}/value", false))
            {
                writer.Write(value ? "1" : "0");
                _lastOutputValue = value;
            }
        }

        /// <summary>
        /// Turns the pin on, ensuring it is in output mode
        /// </summary>
        public PreparedPin On()
        {
            if (!_isOutput)
                AsOutput();

            Write(true);

            return this;
        }

        /// <summary>
        /// Turns the pin off, ensuring it is in output mode
        /// </summary>
        public PreparedPin Off()
        {
            if (!_isOutput)
                AsOutput();

            Write(false);

            return this;
        }

        /// <summary>
        /// Helper to toggle between on and off
        /// </summary>
        public PreparedPin Toggle()
        {
            if (Read())
            {
                Off();
            }
            else
            {
                On();
            }

            return this;
        }

        /// <summary>
        /// Determines the current state of the pin
        /// </summary>
        /// <returns>the current state of the pin</returns>
        public bool Read()
        {
            if (_isOutput)
            {
                return _lastOutputValue; // don't bother reading the real value just return the last value we wrote
            }

            using (var reader = new StreamReader(new FileStream($"{Dir}/gpio{_pin}/value",
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite)))
            {
                var rawValue = reader.ReadToEnd();
                return rawValue.StartsWith("1");
            }
        }

        private void SetDirection(string direction)
        {
            using (var writer = new StreamWriter($"{Dir}/gpio{_pin}/direction", false))
            {
                writer.Write(direction);
            }
        }

        /// <summary>
        /// Puts the pin into output mode maintaining it's current on/off state
        /// </summary>
        public PreparedPin AsOutput()
        {
            // Read() return whether the pin is currently on or not
            // "high" sets the pin to output but leaves it on
            // "low" set the pin to output but leaves it off
            SetDirection(Read() ? "high" : "low");

            _isOutput = true;

            return this;
        }

        /// <summary>
        /// Puts the pin into input mode
        /// </summary>
        public PreparedPin AsInput()
        {
            SetDirection("in");

            _isOutput = false;

            return this;
        }
    }
}

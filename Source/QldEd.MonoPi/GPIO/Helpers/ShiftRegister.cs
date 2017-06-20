using System.Linq;

namespace QldEd.MonoPi.GPIO.Helpers
{
    /// <summary>
    /// Basic implementation of output via a SN74HC595N
    /// </summary>
    public class ShiftRegister
    {
        private readonly PreparedPin _signalPin;
        private readonly PreparedPin _signalClockPin;
        private readonly PreparedPin _registerClockPin;
        private readonly PreparedPin _outputEnabledPin;

        private readonly bool[] _values;

        /// <summary>
        /// If you wish to set multiple values at once set this to false.
        /// Once you have set all the values to want call Commit().
        /// </summary>
        public bool AutoCommit { get; set; } = true;

        /// <summary>
        /// Create a shift register with the appropriate Pi pins
        /// </summary>
        /// <param name="signalPin">GPIO Pin connected to Pin 14 of the SN74HC595N</param>
        /// <param name="signalClockPin">GPIO Pin connected to Pin 11 of the SN74HC595N</param>
        /// <param name="registerClockPin">GPIO Pin connected to Pin 12 of the SN74HC595N</param>
        /// <param name="clearPin">GPIO Pin connected to Pin 10 of the SN74HC595N</param>
        /// <param name="outputEnabledPin">GPIO Pin connected to Pin 13 of the SN74HC595N</param>
        /// <param name="numChips">The number of SN74HC595N connected in series.</param>
        /// To connect multiple chips in series connect
        /// Pi                  Chip 1      Chip 2      Chip 3
        /// signalPin           -> Pin 14
        /// signalClockPin      -> Pin 11   -> Pin 11   -> Pin 11
        /// registerClockPin    -> Pin 12   -> Pin 12   -> Pin 12
        /// clearPin            -> Pin 10   -> Pin 10   -> Pin 10
        /// outputEnabledPin    -> Pin 13   -> Pin 13   -> Pin 13
        ///                        Pin 9    -> Pin 14
        ///                                    Pin 9    -> Pin 14
        public ShiftRegister(PreparedPin signalPin, PreparedPin signalClockPin, PreparedPin registerClockPin, PreparedPin clearPin, PreparedPin outputEnabledPin,
            int numChips = 1)
        {
            _signalPin = signalPin;
            _signalClockPin = signalClockPin;
            _registerClockPin = registerClockPin;
            clearPin.On(); // make sure the clear pin is pulled high
            _outputEnabledPin = outputEnabledPin;

            _values = new bool[numChips * 8];

            SendAllValues();
        }

        /// <summary>
        /// To use the register simply treat it as an array of bools
        /// </summary>
        public bool this[int i]
        {
            get { return _values[i]; }
            set
            {
                _values[i] = value;

                if (AutoCommit)
                {
                    SendAllValues();
                }
            }
        }

        /// <summary>
        /// When AutoCommit is disabled use this to send the values to the output pin
        /// </summary>
        public void Commit()
        {
            SendAllValues();
        }

        /// <summary>
        /// Reset the array to false. NB: doesn't use the shift registers clear pin
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _values.Length; i++)
            {
                _values[i] = false;
            }

            if (AutoCommit)
            {
                SendAllValues();
            }
        }

        /// <summary>
        /// This is the bit that actually does the bit banging to the chip
        /// </summary>
        private void SendAllValues()
        {
            _registerClockPin.Off();

            foreach (var b in _values.Reverse())
            {
                _signalClockPin.Off();
                if (b)
                {
                    _signalPin.On();
                }
                else
                {
                    _signalPin.Off();
                }
                _signalClockPin.On();
            }

            _registerClockPin.On();
            _registerClockPin.Off();

            _outputEnabledPin.Off(); // low = enabled, ensure that the output is actually enabled
        }
    }
}

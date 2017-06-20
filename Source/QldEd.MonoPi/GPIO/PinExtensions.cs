using QldEd.MonoPi.GPIO.Pinouts;

namespace QldEd.MonoPi.GPIO
{
    /// <summary>
    /// Extension methods to allow you to initialise a <see cref="PreparedPin"/> from an enum value. 
    /// This allows the pinouts to be simple enums but without the need for the consumer to cast to an int. 
    /// In theory you could make the PreparedPin constructor internal to restrict consumers to known pin values.
    /// </summary>
    public static class PinExtensions
    {
        public static PreparedPin Prepare(this Pi1Pins pin)
        {
            return PreparedPin.Prepare((int)pin);
        }

        public static PreparedPin Prepare(this Pi2Pins pin)
        {
            return PreparedPin.Prepare((int)pin);
        }

        public static PreparedPin Prepare(this Pi3Pins pin)
        {
            return PreparedPin.Prepare((int)pin);
        }

        public static PreparedPin Prepare(this BananaPiPins pin)
        {
            return PreparedPin.Prepare((int)pin);
        }
    }
}

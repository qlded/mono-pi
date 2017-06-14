using QldEd.MonoPi.GPIO.Pinouts;

namespace QldEd.MonoPi.GPIO
{
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

using System.Threading;

namespace QldEd.MonoPi.GPIO.Helpers
{
    public class I2C
    {
        private readonly PreparedPin _sda;
        private readonly PreparedPin _scl;
        
        /// <summary>
        /// The time to sleep between changing SDA before changing SCL
        /// Realistically it could probably be 0 and still work 
        /// 5 works reliably for MPL3115A2 from a RaspberryPi 3 
        /// </summary>
        public int SleepTimeMilliseconds { get; set; }= 5;

        public I2C(PreparedPin sdaPin, PreparedPin sclPin)
        {
            _sda = sdaPin;
            _scl = sclPin;
        }

        private void I2CSleep()
        {
            Thread.Sleep(SleepTimeMilliseconds);
        }

        private void I2CStart()
        {
            // start signal = pull sda down while scl is high
            _sda.On();
            I2CSleep();
            _scl.On();
            I2CSleep();
            _sda.Off();
            I2CSleep();
            _scl.Off();
            I2CSleep();
        }

        private void I2CStop()
        {
            // stop signal = pull sda up while scl is high
            _sda.Off();
            I2CSleep();
            _scl.On();
            I2CSleep();
            _sda.On();
            I2CSleep();
            _scl.Off();
        }

        public void WriteRegister(byte address, byte register, byte value)
        {
            I2CStart();

            I2CSendByte(address);

            I2CSendByte(register);

            I2CSendByte(value);

            I2CStop();
        }

        public byte ReadRegister(byte address, byte register)
        {
            var readAddress = (byte)(address | 0x01);

            I2CStart();

            I2CSendByte(address);

            I2CSendByte(register);

            I2CStart();

            I2CSendByte(readAddress);

            var a = I2CReadByte(false);

            I2CStop();

            return a;
        }

        private void I2CSendBit(bool bit)
        {
            if (bit)
            {
                _sda.On();
            }
            else
            {
                _sda.Off();
            }

            I2CSleep();

            do
            {
                _scl.On();
            } while (!_scl.Read()); // wait for clock stretching

            I2CSleep();

            _scl.Off();
        }

        private bool I2CReadBit()
        {
            _sda.AsInput();
            I2CSleep();

            do
            {
                _scl.On();
            } while (!_scl.Read()); // wait for clock stretching

            I2CSleep();

            var result = _sda.Read();

            _scl.Off();

            return result;
        }

        private bool I2CSendByte(byte b)
        {
            for (var i = 0; i < 8; i++)
            {
                var bit = (b & 0x80) != 0;
                I2CSendBit(bit);
                b <<= 1;
            }

            I2CSleep();

            return I2CReadBit();
        }

        private byte I2CReadByte(bool ack)
        {
            byte b = 0;

            for (var i = 0; i < 8; i++)
            {
                var bit = I2CReadBit() ? 1 : 0;
                b = (byte)((b << 1) | bit);
            }

            if (ack)
            {
                _sda.Off();
            }
            else
            {
                _sda.On();
            }

            _scl.On();
            I2CSleep();
            _scl.Off();
            _sda.On();

            return b;
        }
    }
}

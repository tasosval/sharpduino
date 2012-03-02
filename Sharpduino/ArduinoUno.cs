using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Messages.Send;
using Sharpduino.Library.Base.Messages.TwoWay;
using Sharpduino.Library.Base.SerialProviders;

namespace Sharpduino
{
    public enum ArduinoUnoDigitalPins
    {
        D0_RX = 0,
        D1_TX,
        D2,
        D3_PWM,
        D4,
        D5_PWM,
        D6_PWM,
        D7,
        D8,
        D9_PWM,
        D10_PWM,
        D11_PWM,
        D12,
        D13,
        A0 = 16,
        A1 = 17,
        A2 = 18,
        A3 = 19,
        A4 = 20,
        A5 = 21
    }

    public enum ArduinoUnoPWMPins
    {
        D3_PWM = 3,
        D5_PWM = 5,
        D6_PWM = 6,
        D9_PWM = 9,
        D10_PWM = 10,
        D11_PWM = 11
    }

    public enum ArduinoUnoAnalogPins
    {
        A0 = 0,
        A1,
        A2,
        A3,
        A4,
        A5
    }

    public static class ArduinoUnoConstantsHelper
    {
        public static int PinToAnalog(this int pin)
        {
            return pin - 16;
        }

        public static int AnalogToPin(this int analogPin)
        {
            return analogPin + 16;
        }
    }

    public class ArduinoUno : IDisposable
    {
        private readonly EasyFirmata firmata;

        public ArduinoUno(string comPort)
        {
            var provider = new ComPortProvider(comPort);
            firmata = new EasyFirmata(provider);
        }

        public void SetServoMode(ArduinoUnoDigitalPins pin, int minPulse, int maxPulse, int startAngle)
        {
            throw new NotImplementedException();
            if (firmata.IsInitialized == false)
                return;

        }

        public void SetPinMode(ArduinoUnoDigitalPins pin, PinModes mode)
        {
            if ( firmata.IsInitialized == false )
                return;

            // TODO : Special case for servo message...
            // TODO : Special case for I2C message...
            if (mode == PinModes.I2C || mode == PinModes.Servo)
                throw new NotImplementedException();

            // Set the pin...
            firmata.SendMessage(new PinModeMessage{Mode = mode, Pin = (byte) pin});
            
            // TODO : see if we need this or the next way
            //firmata.Pins[(byte) pin].CurrentMode = mode;
            
            // Update the pin state
            firmata.SendMessage(new PinStateQueryMessage(){Pin = (byte) pin});
        }

        public void SetDO(ArduinoUnoDigitalPins pin, bool newValue)
        {
            if (firmata.IsInitialized == false)
                return;

            // TODO : Decide on whether this should throw an exception
            if ( firmata.Pins[(int) pin].CurrentMode != PinModes.Output )
                return;

            // find the port which this pin belongs to
            var port = (byte) pin/8;
            // get the values for the other pins in this port
            var previousValues = firmata.GetDigitalPortValues(port);
            // update the new value for this pin
            previousValues[(int) pin] = newValue;
            // Send the message to the board
            firmata.SendMessage(new DigitalMessage(){Port = port, PinStates = previousValues});
            // update the new value to the firmata pins list
            firmata.Pins[(int) pin].CurrentValue = newValue ? 1 : 0;
        }

        public void SetPWM(ArduinoUnoPWMPins pin, int newValue)
        {
            if (firmata.IsInitialized == false)
                return;

            // TODO : Decide on whether this should throw an exception
            if (firmata.Pins[(int)pin].CurrentMode != PinModes.PWM)
                return;

            // Send the message to the board
            firmata.SendMessage(new AnalogMessage(){Pin = (byte)pin, Value = newValue});

            // Update the firmata pins list
            firmata.Pins[(int) pin].CurrentValue = newValue;
        }

        public void SetServo(ArduinoUnoDigitalPins pin, int newValue)
        {
            if (firmata.IsInitialized == false)
                return;

            // TODO : Decide on whether this should throw an exception
            if (firmata.Pins[(int)pin].CurrentMode != PinModes.Servo)
                return;

            firmata.SendMessage(new AnalogMessage(){Pin = (byte)pin,Value = newValue});

            // Update the firmata pins list
            firmata.Pins[(int)pin].CurrentValue = newValue;
        }

        public float ReadAnalog(ArduinoUnoAnalogPins pin)
        {
            if (firmata.IsInitialized == false)
                return -1;

            // TODO : Decide on whether this should throw an exception
            if (firmata.AnalogPins[(int)pin].CurrentMode != PinModes.Analog)
                return -1;

            return firmata.AnalogPins[(int)pin].CurrentValue;
        }


        public void Dispose()
        {
            firmata.Dispose();
        }
    }
}

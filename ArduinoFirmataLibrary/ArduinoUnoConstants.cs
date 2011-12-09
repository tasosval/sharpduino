using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoFirmataLibrary
{
    public enum ArduinoUnoDigitalPins
    {
        D0_RX,
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
        A0,
        A1,
        A2,
        A3,
        A4,
        A5
    }

    public static class ArduinoUnoConstantsHelper
    {
        public static int PinToAnalog(int pin)
        {
            return pin - 16;
        }

        public static int AnalogPinToPin(int analogPin)
        {
            return analogPin + 16;
        }
    }
}

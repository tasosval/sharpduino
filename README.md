# sharpduino

A basic library that permits controlling an arduino through the firmata protocol. The current version is developed and tested against the StandardFirmata? 2.3 sketch distributed with the arduino 1.0 software.

The purpose of this library is to make it easy to .net developers to connect their programs to the real world using the arduino hardware.

The library is developed with the simple user in mind, while allowing the more advanced user (someone who might make changes to the Arduino sketch or someone who wants complete control over the communication of his program with the board) to do anything he wants, by providing many extension points where he can plug his own code. He can complement the available functionality or override some or all of it, by simply inheriting from a class.

Here are some examples for the simple implementation:

```C#
// Create a new connection
var arduino = new ArduinoUno("COM4");

// Read an analog value 
float valueInVolts = arduino.ReadAnalog(ArduinoUnoAnalogPins.A0);

// Read the state of a pin
arduino.GetCurrentPinState(ArduinoUnoPins.D8);

// Write a digital value to an output pin
arduino.SetPinMode(ArduinoUnoPins.D4,PinModes.Output);
arduino.SetDO(ArduinoUnoPins.D4,True);

// Write an analog value (PWM) to a PWM pin
arduino.SetPinMode(ArduinoUnoPins.D3_PWM,PinModes.PWM);
arduino.SetPWM(ArduinoUnoPWMPins.D3_PWM, 90);

// Use a servo
arduino.SetPinMode(ArduinoUnoPins.D9_PWM,PinModes.Servo);
arduino.SetServo(ArduinoUnoPins.D9_PWM,90);

// dispose of the object
arduino.Dispose();
```
In order to hide much of the protocol complexity, a lot of what is going on is hidden from the user. For example the board is setup to continuously send update messages for the analog and digital values which are handled by the ArduinoUno? class, so for example readAnalog always contains the latest value, even though the user is never notified.

I will be posting some examples in my blog
I would like to thank the following people for their contributions:

Noriaki Mitsunaga. You can find his useful applications using this library here 

Anton Smirnov. You can find his take on creating such a library in Java here 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Messages.Receive;
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

    public interface IHandleAdvancedMessages :
        IHandle<AnalogMappingMessage>, IHandle<CapabilityMessage>, IHandle<CapabilitiesFinishedMessage>,
        IHandle<I2CResponseMessage>, IHandle<PinStateMessage>, IHandle<ProtocolVersionMessage>,
        IHandle<SysexFirmwareMessage>, IHandle<SysexStringMessage>
    {}

    public interface IHandleBasicMessages : IHandle<AnalogMessage>, IHandle<DigitalMessage>
    {}

    public interface IHandleAllMessages : IHandleBasicMessages, IHandleAdvancedMessages
    {}

    public class PinCapability : CapabilityMessage
    {}

    public class NewAnalogValueEventArgs : EventArgs
    {
        public int NewValue { get; set; }
        public byte AnalogPin { get; set; }
    }

    public class NewDigitalValueEventArgs : EventArgs
    {
        public byte Port { get; set; }
        public bool[] Pins { get; set; }
    }

    public class NewStringMessageEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public class Pin
    {
        public PinModes CurrentMode { get; set; }
        public Dictionary<PinModes, int> Capabilities { get; set; }
        public int CurrentValue { get; set; }

        public Pin()
        {
            Capabilities = new Dictionary<PinModes, int>();
            CurrentValue = 0;
        }
    }

    /// <summary>
    /// This is an easy to use master for an board running Standard Firmata 2.3 
    /// (bundled with Arduino 1.0 software)
    /// </summary>
    public class EasyFirmata : FirmataBase, IHandleAllMessages
    {
        private enum InitializationStages
        {
            QueryProtocolVersion = 0,
            QueryFirmwareVersion,
            QueryCapabilities,
            QueryAnalogMappings,
            QueryPinStates,
            StartReports,
            FullyInitialized
        }

        private InitializationStages currentInitState;

        /***********************************************************************************************/
        //                                       PROPERTIES                                            //
        /***********************************************************************************************/
        #region Properties

        /// <summary>
        /// This is true if we have finished the first communications with the board
        /// to setup the main functionality. The EasyFirmata can be used when this is true
        /// </summary>
        public bool IsInitialized
        {
            get { return currentInitState == InitializationStages.FullyInitialized; }
        }

        /// <summary>
        /// This event marks the end of the initialization procedure
        /// The EasyFirmata is usable now
        /// </summary>
        public event EventHandler Initialized;

        /// <summary>
        /// Event to notify about new analog values
        /// </summary>
        public event EventHandler<NewAnalogValueEventArgs> NewAnalogValue;

        /// <summary>
        /// Event that is raised when a digital message is received 
        /// </summary>
        public event EventHandler<NewDigitalValueEventArgs> NewDigitalValue;

        /// <summary>
        /// Event that is raised when a string message is received
        /// </summary>
        public event EventHandler<NewStringMessageEventArgs> NewStringMessage;

        /// <summary>
        /// The pins available
        /// </summary>
        public List<Pin> Pins { get; private set; }

        /// <summary>
        /// The analog pins of the board
        /// </summary>
        public List<Pin> AnalogPins { get; private set; }

        /// <summary>
        /// The protocol version that the board uses to communicate
        /// </summary>
        public string ProtocolVersion { get; private set; }

        /// <summary>
        /// The firmware version that the board is running
        /// </summary>
        public string Firmware { get; private set; }

        #endregion

        public EasyFirmata(ISerialProvider serialProvider): base(serialProvider)
        {
            // Initialize the objects
            Pins = new List<Pin>();
            AnalogPins = new List<Pin>();
            
            // Subscribe ourselves to the message broker
            MessageBroker.Subscribe(this);

            // Start the init procedure
            ReInit();
        }

        /// <summary>
        /// Do the initialization from the start
        /// </summary> 
        public void ReInit()
        {
            currentInitState = 0;
            AdvanceInitialization();
        }

        /// <summary>
        /// Go through the initialization procedure
        /// </summary>
        private void AdvanceInitialization()
        {
            // Do nothing if we are initialized
            if ( currentInitState == InitializationStages.FullyInitialized)
                return;

            switch (currentInitState)
            {
                case InitializationStages.QueryProtocolVersion:
                    // This is the first inistialization stage
                    // Stop any previous reports
                    StopReports();
                    base.SendMessage(new ProtocolVersionRequestMessage());
                    break;
                case InitializationStages.QueryFirmwareVersion:
                    base.SendMessage(new QueryFirmwareMessage());
                    break;
                case InitializationStages.QueryCapabilities:
                    // Clear the pins, as we will be receiving new ones
                    Pins.Clear();
                    AnalogPins.Clear();
                    // Send the message to get the capabilities
                    base.SendMessage(new QueryCapabilityMessage());
                    break;
                case InitializationStages.QueryAnalogMappings:
                    base.SendMessage(new AnalogMappingQueryMessage());
                    break;
                case InitializationStages.QueryPinStates:
                    for (int i = 0; i < Pins.Count; i++)
                    {
                        base.SendMessage(new PinStateQueryMessage{Pin = (byte) i});
                    }
                    break;
                case InitializationStages.StartReports:
                    var portsCount = (byte) Math.Ceiling(Pins.Count/8.0);
                    for (byte i = 0; i < portsCount; i++)
                    {
                        base.SendMessage(new ToggleDigitalReportMessage() { Port = i, ShouldBeEnabled = true });
                    }

                    for (byte i = 0; i < AnalogPins.Count; i++)
                    {
                        base.SendMessage(new ToggleAnalogReportMessage() { Pin = i, ShouldBeEnabled = true });
                    }

                    // There is no callback for the above messages so advance anyway                    
                    OnInitialized();
                    break;
                    case InitializationStages.FullyInitialized:
                    // Do nothing we are finished with the initialization
                    break;
                default:
                    throw new ArgumentOutOfRangeException("stage");
            }

            // Go to the next state
            if ( !IsInitialized)
                currentInitState++;
        }

        /***********************************************************************************************/
        //                               INCOMING MESSAGE HANDLING                                     //
        /***********************************************************************************************/
        #region Incoming Message Handling

        /// <summary>
        /// Handle the Protocol Message. Contains info about the protocol that the board is using to communicate
        /// </summary>
        public void Handle(ProtocolVersionMessage message)
        {
            if ( IsInitialized )return;
            ProtocolVersion = string.Format("{0}.{1}", message.MajorVersion, message.MinorVersion);
            AdvanceInitialization();
        }

        /// <summary>
        /// Handle the Firmware Message. Contains info about the firmware running in the board
        /// </summary>
        public void Handle(SysexFirmwareMessage message)
        {
            if ( IsInitialized ) return;
            Firmware = string.Format("{0}:{1}.{2}", message.FirmwareName, message.MajorVersion, message.MinorVersion);
            AdvanceInitialization();
        }        

        /// <summary>
        /// Handle the capability messages. There will be one such message for each pin
        /// </summary>
        public void Handle(CapabilityMessage message)
        {
            var pin = new Pin();
            foreach (var mes in message.Modes)
                pin.Capabilities[mes.Key] = mes.Value;

            // Add it to the collection
            Pins.Add(pin);
        }

        /// <summary>
        /// Handle the Capabilities Finished Message. This is used to advance to the next step of
        /// the initialization after the capabilities
        /// </summary>
        public void Handle(CapabilitiesFinishedMessage message)
        {
            // If we haven't initialized then do the next thing in the init procedure
            if ( !IsInitialized )
                AdvanceInitialization();

            // Otherwise this message conveys no information
        }
        
        /// <summary>
        /// Handle the Analog Mapping Message. This is used to find out which pins have 
        /// analog input capabilities and fill the AnalogPins list
        /// </summary>
        public void Handle(AnalogMappingMessage message)
        {
            if (IsInitialized) return;
            
            for (int i = 0; i < message.PinMappings.Count; i++)
            {
                // If we have an analog pin
                if ( message.PinMappings[i] != 127 )
                {
                    // Put the corresponding pin to the analog pins dictionary
                    // this is a reference, so any changes to the primary object
                    // will be reflected here too.
                    AnalogPins.Add(Pins[i]);
                }
            }
            AdvanceInitialization();
        }

        /// <summary>
        /// Handler the Pin State Message. Get more information about each pin.
        /// This is called multiple times and we advance to the next step, only after
        /// we have received information about the last pin
        /// </summary>
        public void Handle(PinStateMessage message)
        {            
            Pin currentPin = Pins[message.PinNo];
            currentPin.CurrentMode = message.Mode;
            currentPin.CurrentValue = message.State;

            if (IsInitialized) return;

            // here we check to see if we have finished with the PinState Messages
            // and advance to the next step. Test the following:
            if ( message.PinNo == Pins.Count - 1 )
                AdvanceInitialization();
        }

        /// <summary>
        /// Handle the Analog Messsage. Update the value for the pin and raise a
        /// NewAnalogValue event
        /// </summary>
        public void Handle(AnalogMessage message)
        {
            // Here we are in the twilight zone
            if (currentInitState <= InitializationStages.QueryPinStates )
                return;

            // First save the value in the Pins and AnalogPins lists
            AnalogPins[message.Pin].CurrentValue = message.Value;

            OnNewAnalogValue(message.Pin,message.Value);
        }

        /// <summary>
        /// Handle the Digital Message. Update the values for the pins of the port
        /// and raise a NewDigitalValue event
        /// </summary>
        /// <param name="message"></param>
        public void Handle(DigitalMessage message)
        {
            var pinStart = (byte)(8*message.Port);
            for (byte i = 0; i < 8; i++)
            {
                Pins[i + pinStart].CurrentValue = message.PinStates[i] ? 1 : 0;
            }
        }

        /// <summary>
        /// Handle the Sysex String Message. Raise a NewStringMessage event
        /// </summary>
        /// <param name="message"></param>
        public void Handle(SysexStringMessage message)
        {
            OnNewStringMessage(message.Message);
        }

        public void Handle(I2CResponseMessage message)
        {
            throw new NotImplementedException();
        }
        #endregion

        /***********************************************************************************************/
        //                                  EVENTS CREATION                                            //
        /***********************************************************************************************/
        #region Event Creation
        public void OnInitialized()
        {
            var handler = Initialized;
            if ( handler != null )
            {
                handler(this,new EventArgs());
            }
        }

        public void OnNewAnalogValue(byte pin, int value)
        {
            var handler = NewAnalogValue;
            if ( handler != null )
            {
                handler(this, new NewAnalogValueEventArgs() { AnalogPin = pin, NewValue = value });
            }
        }

        public void OnNewDigitalValue(byte port,bool[] pins)
        {
            var handler = NewDigitalValue;
            if ( handler != null )
            {
                handler(this, new NewDigitalValueEventArgs() { Port = port, Pins = pins });
            }
        }

        public void OnNewStringMessage(string message)
        {
            var handler = NewStringMessage;
            if ( handler != null )
            {
                handler(this, new NewStringMessageEventArgs() { Message = message });
            }
        }
        #endregion

        /// <summary>
        /// Stop receiving reports.
        /// </summary>
        private void StopReports()
        {
            for (byte i = 0; i < MessageConstants.MAX_DIGITAL_PORTS; i++)
            {
                base.SendMessage(new ToggleDigitalReportMessage() { Port = i, ShouldBeEnabled = false });
            }

            for (byte i = 0; i < AnalogPins.Count; i++)
            {
                base.SendMessage(new ToggleAnalogReportMessage() { Pin = i, ShouldBeEnabled = false });
            }
        }

        /// <summary>
        /// Send a message to the firmata board. 
        /// Take care: if the EasyFirmata hasn't finished with the initialization this will do nothing
        /// </summary>
        public override void SendMessage<T>(T message)
        {
            if ( !IsInitialized )
                return;
            base.SendMessage(message);
        }

        protected override void Dispose(bool shouldDispose)
        {
            StopReports();

            base.Dispose(shouldDispose);
        }
    }
}
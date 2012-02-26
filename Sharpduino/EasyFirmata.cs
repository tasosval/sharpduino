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
            ToggleDigitalReports,
            ToggleAnalogReports,
            FullyInitialized
        }

        private InitializationStages currentInitState;

        #region Properties

        public bool IsInitialized
        {
            get { return currentInitState == InitializationStages.FullyInitialized; }
        }

        /// <summary>
        /// This event marks the end of the initialization procedure
        /// The EasyFirmata is usable now
        /// </summary>
        public EventHandler Initialized;

        /// <summary>
        /// The pins available
        /// </summary>
        public List<Pin> Pins { get; private set; }

        /// <summary>
        /// The analog pins of the board
        /// </summary>
        public List<Pin> AnalogPins { get; private set; }

        public string ProtocolVersion { get; private set; }

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

        // Do the initialization from the start
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
                    base.SendMessage(new ProtocolVersionMessage());
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
                    base.SendMessage(new PinStateMessage());
                    break;
                case InitializationStages.ToggleDigitalReports:
                    break;
                case InitializationStages.ToggleAnalogReports:
                    break;
                case InitializationStages.FullyInitialized:
                    if (Initialized != null )
                        Initialized(this,new EventArgs());
                    break;
                default:
                    throw new ArgumentOutOfRangeException("stage");
            }

            // Go to the next state
            currentInitState++;
        }


        public void Handle(ProtocolVersionMessage message)
        {
            ProtocolVersion = string.Format("{0}.{1}", message.MajorVersion, message.MinorVersion);
        }

        public void Handle(SysexFirmwareMessage message)
        {
            Firmware = string.Format("{0}:{1}.{2}", message.FirmwareName, message.MajorVersion, message.MinorVersion);
        }        

        /// <summary>
        /// Handle the capability messages there will be one such message for each pin
        /// </summary>
        public void Handle(CapabilityMessage message)
        {
            var pin = new Pin();
            foreach (var mes in message.Modes)
                pin.Capabilities[mes.Key] = mes.Value;

            // Add it to the collection
            Pins.Add(pin);
        }

        public void Handle(CapabilitiesFinishedMessage message)
        {
            // If we haven't initialized then do the next thing in the init procedure
            if ( !IsInitialized )
                AdvanceInitialization();

            // Otherwise this message conveys no information
        }
        
        public void Handle(AnalogMappingMessage message)
        {
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
        }

        public void Handle(PinStateMessage message)
        {
            Pin currentPin = Pins[message.PinNo];
            currentPin.CurrentMode = message.Mode;
            currentPin.CurrentValue = message.State;

            // TODO : here we should check to see if we have finished with the PinState Messages
            // and advance to the next step. Test the following:
            if ( message.PinNo == Pins.Count )
                AdvanceInitialization();
        }


        /// <summary>
        /// Stop receiving reports.
        /// </summary>
        private void StopReports()
        {
            throw new NotImplementedException();
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
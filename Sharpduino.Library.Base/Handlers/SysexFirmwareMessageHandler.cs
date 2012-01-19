using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages;

namespace Sharpduino.Library.Base.Handlers
{

/*Query Firmware Name and Version
Query Firmware Name and Version
0  START_MESSAGE (0xF0)
1  queryFirmware (0x79)
2  END_SYSEX (0xF7)
*/
    /// <summary>
    /// Class that handles the receipt of the firmware sysex message
    /// The firmware name to be reported should be exactly the same as the name of the Arduino file, minus the .pde. So for Standard_Firmata.pde, the firmware name is: Standard_Firmata.
    /// Receive Firmware Name and Version (after query)
    /// 0  START_MESSAGE (0xF0)
    /// 1  queryFirmware (0x79)
    /// 2  major version (0-127)
    /// 3  minor version (0-127)
    /// 4  first 7-bits of firmware name
    /// 5  second 7-bits of firmware name
    /// x  ...for as many bytes as it needs)
    /// 6  END_SYSEX (0xF7)
    /// </summary>
    public class SysexFirmwareMessageHandler : BaseMessageHandler
    {
        public const byte QUERY_FIRMWARE = 0x79;
        public const byte END_SYSEX = 0xF7;

        private enum HandlerState
        {
            StartSysex = 0,
            QueryFirmware,
            MajorVersion,
            MinorVersion,
            FirmwareName,
            EndStart
        }

        private HandlerState currentHandlerState;
        private SysexFirmwareMessage sysexFirmwareMessage;
        private int currentByteCount;

        public SysexFirmwareMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
        {
            currentHandlerState = HandlerState.EndStart;
            currentByteCount = 0;
            START_MESSAGE = 0xF0;        
        }

        /// <summary>
        /// Find out if the handler can handle the next byte
        /// </summary>
        /// <param name="firstByte">The first byte of the message</param>
        /// <returns>True if the handle is able to handle the message</returns>
        public override bool CanHandle(byte firstByte)
        {
            switch (currentHandlerState)
            {
                case HandlerState.StartSysex:
                    return firstByte == 0x79;
                case HandlerState.QueryFirmware:
                case HandlerState.MajorVersion:
                case HandlerState.MinorVersion:
                case HandlerState.FirmwareName:
                    return true;
                case HandlerState.EndStart:
                    return firstByte == 0xF0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Handle the byte that came from the communication port
        /// </summary>
        /// <param name="messageByte">The byte that came from the port. It might be the first one, or a subsequent one</param>
        /// <returns>True if it should handle the next byte too</returns>
        public override bool Handle(byte messageByte)
        {
            if (!CanHandle(messageByte))
            {
                // Reset the state of the handler
                currentHandlerState = HandlerState.EndStart;
                throw new MessageHandlerException("Error with the incoming byte. This is not a valid SysexMessage");
            }

            if (++currentByteCount > MAXDATABYTES)
            {
                // Reset the state of the handler
                currentHandlerState = HandlerState.EndStart;
                throw new MessageHandlerException("Error with the incoming byte. This is not a valid SysexMessage. Max message length was exceeded.");
            }

            switch (currentHandlerState)
            {
                case HandlerState.StartSysex:
                    // MAX_DATA bytes AFTER the command byte
                    currentByteCount = 0;
                    currentHandlerState = HandlerState.QueryFirmware;
                    return true;
                case HandlerState.QueryFirmware:
                    currentHandlerState = HandlerState.MajorVersion;
                    sysexFirmwareMessage.MajorVersion = messageByte;
                    return true;
                case HandlerState.MajorVersion:
                    currentHandlerState = HandlerState.MinorVersion;
                    sysexFirmwareMessage.MinorVersion = messageByte;
                    return true;
                case HandlerState.MinorVersion:
                    currentHandlerState = HandlerState.FirmwareName;
                    sysexFirmwareMessage.FirmwareName += Convert.ToChar(messageByte);
                    return true;
                case HandlerState.FirmwareName:
                    if (messageByte == 0xF7)
                    {
                        currentHandlerState = HandlerState.EndStart;
                        messageBroker.CreateEvent(sysexFirmwareMessage);
                        return false;
                    }
                    sysexFirmwareMessage.FirmwareName += Convert.ToChar(messageByte);
                    return true;
                case HandlerState.EndStart:
                    currentHandlerState = HandlerState.StartSysex;
                    sysexFirmwareMessage = new SysexFirmwareMessage{FirmwareName = ""};
                    return true;
                default:
                    throw new MessageHandlerException("Unknown SysexMessage handler state");
            }
        }
    }
}

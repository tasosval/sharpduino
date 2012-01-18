﻿namespace Sharpduino.Library.Base.Messages
{
    public struct SysexFirmwareMessage
    {
        public byte MajorVersion { get; set; }
        public byte MinorVersion { get; set; }
        public string FirmwareName { get; set; }
    }
}
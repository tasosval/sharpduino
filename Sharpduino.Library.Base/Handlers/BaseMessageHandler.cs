﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpduino.Library.Base.Handlers
{
    public abstract class BaseMessageHandler : IMessageHandler
    {
        protected readonly IMessageBroker messageBroker;
        
        public const int MAXANALOGPINS = 16;
        public const int MAXDIGITALPORTS = 16;
        public const int MAXDIGITALPINS = 128;
        public const int MAXDATABYTES = 1024;
		public const byte MESSAGETYPEMASK = 0xF0;
		public const byte MESSAGEPINMASK = 0x0F;

        public byte START_MESSAGE { get; protected set; }

        protected BaseMessageHandler(IMessageBroker messageBroker)
        {
            this.messageBroker = messageBroker;
        }

        /// <summary>
        /// Find out if the handler can handle the next byte
        /// </summary>
        /// <param name="firstByte">The first byte of the message</param>
        /// <returns>True if the handle is able to handle the message</returns>
        public abstract bool CanHandle(byte firstByte);

        /// <summary>
        /// Handle the byte that came from the communication port
        /// </summary>
        /// <param name="messageByte">The byte that came from the port. It might be the first one, or a subsequent one</param>
        /// <returns>True if it should handle the next byte too</returns>
        public abstract bool Handle(byte messageByte);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Library.Base.Constants;
using Sharpduino.Library.Base.Exceptions;
using Sharpduino.Library.Base.Messages.Send;

namespace Sharpduino.Library.Base.Creators
{
    public class SamplingIntervalMessageCreator : BaseMessageCreator<SamplingIntervalMessage>
    {
        public override byte[] CreateMessage(SamplingIntervalMessage message)
        {
            if (message == null)
                throw new MessageCreatorException("This is not a valid Sampling Interval Message");

            var bytes = new List<byte>()
                       {
                           MessageConstants.SYSEX_START,
                           SysexCommands.SAMPLING_INTERVAL,
                       };
            bytes.AddRange(BitHelper.GetBytesFromInt(message.Interval));
            bytes.Add(MessageConstants.SYSEX_END);

            return bytes.ToArray();
        }
    }
}

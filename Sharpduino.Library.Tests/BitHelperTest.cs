using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sharpduino.Library.Base;
using FluentAssertions;

namespace Sharpduino.Library.Tests
{
    [TestFixture]
    public class BitHelperTest
    {
        [Test]
        public void PortVal2PinVals_Functions_Correctly()
        {
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                var values = BitHelper.PortVal2PinVals(i);
                for (int valueIndex = 0; valueIndex < values.Length; valueIndex++)
                {
                    var value = values[valueIndex];
                    value.Should().Be( ( (i >> valueIndex) & 1 )== 1);
                }
            }
        }

        [Test]
        public void PinVals2PortVal_Functions_Correctly()
        {
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                // We know from the other test that PortVal2PinVals functions correctly
                var values = BitHelper.PortVal2PinVals(i);
                BitHelper.PinVals2PortVal(values).Should().Be(i);
            }            
        }

        [Test]
        public void Sevens2Fourteen_Functions_Correctly()
        {
            for (byte lsb = 0; lsb < byte.MaxValue; lsb++)
            {
                for (byte msb = 0; msb < byte.MaxValue; msb++)
                {
                    var tempMSB = msb & 0x7F;
                    var tempLSB = lsb & 0x7F;
                    var value = tempMSB << 7 | tempLSB;
                    BitHelper.Sevens2Fourteen(lsb, msb).Should().Be(value);
                }
            }
        }

        [Test]
        public void Fourteen2Sevens_Functions_Correctly()
        {
            for (int i = 0; i < Math.Pow(2,14); i++)
            {
                byte lsb,msb;
                BitHelper.Fourteen2Sevens(i, out lsb, out msb);
                lsb.Should().Be((byte) (i & 0x7F));
                msb.Should().Be((byte) ((i & 0x3F80) >> 7));
            }
        }
    }
}

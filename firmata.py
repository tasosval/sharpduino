# Get the Suitable references
import sys
sys.path.append("e:\\projects\\tasos\\sharpduino\\sharpduino-hg\\sharpduino\\bin\\debug")
import clr
clr.AddReference("Sharpduino")
clr.AddReference("Sharpduino.Library.Base")

# Import namespaces that we need
from Sharpduino import *
from Sharpduino.Library.Base import *
from Sharpduino.Library.Base.SerialProviders import *
from Sharpduino.Library.Base.Messages.Send import *
from Sharpduino.Library.Base.Messages.TwoWay import *

#create the serial provider
port = ComPortProvider("COM3")

#create the firmata class
firm = EasyFirmata(port)

#Here we should wait
while firm.IsInitialized == False:
	pass

#create the message we want to send
mes = ServoConfigMessage()
mes.Pin = 9

#send the message
firm.SendMessage(mes)

# Go to another Angle
mes2 = AnalogMessage()
mes2.Pin = 9
mes2.Value = 92
firm.SendMessage(mes2)

firm.Dispose()
firm = None
del firm
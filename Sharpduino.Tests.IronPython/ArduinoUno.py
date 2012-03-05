import sys
sys.path.append("c:\\projects\\sharpduino\\sharpduino\\bin\\debug")
import clr
clr.AddReference("Sharpduino")
clr.AddReference("Sharpduino.Library.Base")

# Import namespaces that we need
from Sharpduino import *
from Sharpduino.Library.Base import *
from Sharpduino.Library.Base.SerialProviders import *
from Sharpduino.Library.Base.Messages.Send import *
from Sharpduino.Library.Base.Messages.TwoWay import *
from Sharpduino.Library.Base.Constants import *

ard = ArduinoUno("COM4")
ard.SetSamplingInterval(100)



from System.Threading.Thread import Thread

def blink():
    for i in range(50):    
        global flag        
        ard.SetDO(ArduinoUnoPins.D13,flag)
        flag = not flag
        Thread.Sleep(100)

flag = True
ard.SetPinMode(ArduinoUnoPins.D13,PinModes.Output)
blink()

ard.Dispose()
ard = None
del ard

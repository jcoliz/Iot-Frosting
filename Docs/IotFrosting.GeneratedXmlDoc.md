# IotFrosting #

## Type ADS1015

 https://cdn-shop.adafruit.com/datasheets/ads1015.pdf 



---
#### Method ADS1015.Open

 Open a connection to the device 



> Use this instead of a constructor 

**Returns**: Newly opened ADS1015



---
#### Method ADS1015.Read(System.Int32)

 Read the analog value on a given channel 

|Name | Description |
|-----|------|
|channel: |Which channel to read|
**Returns**: Value of input from 0.0 (0V) to 1.0 (Max V)



---
#### Field ADS1015.sem

 Semaphore to protect against multiple concurrent all-channel reads 



---
#### Method ADS1015.Tick

 Call regularly to update the software status based on the hardware 



---
#### Method ADS1015.ReadInto(System.Double[])

 Read all channels into a single buffer 

|Name | Description |
|-----|------|
|result: |Buffer of values|
**Returns**: Awaitable task



---
#### Property ADS1015.PGA

 Current voltage range value for the PGA 



---
#### Property ADS1015.SamplesPerSecond

 Current value for how many samples per secound do we want 



---
#### Method ADS1015.#ctor(Windows.Devices.I2c.I2cDevice)

 Constructor 

|Name | Description |
|-----|------|
|device: |I2C device we attach to|


---
#### Method ADS1015.CalculateConfigValues

 Pre-calculate the correct CFG register values for each channel 



---
## Type ADS1015.PGAValues

 Acceptable voltage range values for the PGA 



> In units of 1/1000V. E.g. "4096" is 4.096V 



---
## Type ADS1015.SPSValues

 Acceptable values for samples per second 



---
#### Property ADS1015.Input.Voltage

 Current voltage on the input 



---
#### Property ADS1015.Input.AutoLight

 Whether the autolight should in fact be updated with our state 



---
#### Property ADS1015.Input.Light

 The light which shows our state automatically 



---
#### Method ADS1015.Input.Tick

 Call regularly to update the status of the auto light. The auto light will be set to PWM brightness corresponding to the analog input level. 



> Recommend calling on your timer tick 



---
#### Method ADS1015.Input.#ctor(System.Int32,System.Double,IotFrosting.Pimoroni.ILight)

 Constructor 

|Name | Description |
|-----|------|
|channel: |Which ADC channel (0-4) is the input connected to |
|maxvoltage: |The voltage which would drive a 1.0 reading on the underlying ADC|
|light: |The light which will show status|


---
#### Field ADS1015.Input.Channel

 Which ADC channel (0-4) is the input connected to 



---
#### Field ADS1015.Input.MaxVoltage

 The voltage which would drive a 1.0 reading on the underlying ADC 



---
#### Field ADS1015.Input.Values

 All of the analog input values 



---
#### Field ADS1015.Input.NumberOfAnalogInputs

 How many total lights are there in an ADS1015 bank 



---
## Type CAP1XXX

 https://github.com/pimoroni/cap1xxx https://cdn-shop.adafruit.com/datasheets/CAP1188.pdf 



---
#### Field CAP1XXX.Pads

 All the pads we control 



---
#### Field CAP1XXX.Lights

 Direct access to control the lights 



---
#### Method CAP1XXX.#ctor(Windows.Devices.I2c.I2cDevice,System.Int32)

 Constructor 

|Name | Description |
|-----|------|
|alert_pin: |Which pin is the interrupt tied to|


---
#### Property CAP1XXX.Item(System.Byte)

 Quick access to single-byte registers 

|Name | Description |
|-----|------|
|register: |Which register|
**Returns**: Current register value



---
#### Field CAP1XXX.Alert_Sem

 Controls access to any action coming off the alert pin 



---
#### Method CAP1XXX.Alert_Updated(IotFrosting.IInput,System.EventArgs)

 Catch interrupts on the 'alert' pin 



> This is the primary driver of action on this class. The alert pin interrupt tells us we have something to do 

|Name | Description |
|-----|------|
|sender: |Alert input pin|
|e: |Empty event args|


---
#### Method CAP1XXX.Check_Inputs

 Update the Inputs in software to what's there on the hardware 



> Not really sure why I have this as separate from Alert_Updated... 



---
#### Field CAP1XXX.Device

 I2C Device we're attached to 



---
## Type CAP1XXX.Pad

 A single capacitive touch pad 



---
#### Method CAP1XXX.Pad.#ctor(IotFrosting.CAP1XXX.Pad)

 External constructor 

|Name | Description |
|-----|------|
|copy: |The existing pad we're overriding|


---
#### Method CAP1XXX.Pad.#ctor(IotFrosting.CAP1XXX,System.Int32)

 Internal Constructor 

|Name | Description |
|-----|------|
|parent: |Capacitive controller who controls us|
|id: |Which light are we, starting at 0|


---
#### Property CAP1XXX.Pad.State

 Whether we are currently being pressed 



---
#### Property CAP1XXX.Pad.AutoLight

 Whether the cap1xxx hardware automatically controls our light 



---
#### Property CAP1XXX.Pad.Light

 Direct manual access to the underlying light 



---
#### Method CAP1XXX.Pad.Check_Input(System.Byte,System.Byte)

 Update the state based on the known hardware state 

|Name | Description |
|-----|------|
|delta_2c: |Current hardware delta value(2's complement)|
|threshold: |Current hardware threshold value|


> Why not just move this out into the cap1xxx class?? 



---
#### Method CAP1XXX.Pad.DoUpdated

 Raise the Updated event if we have indeed been updated 



---
#### Event CAP1XXX.Pad.Updated

 Event raised when our state is updated 



---
#### Property CAP1XXX.Pad._Light

 Direct manual access to the underlying light 



---
#### Property CAP1XXX.Pad.OldState

 State last time we raised the updated event 



---
## Type CAP1XXX.Light

 This is the cap1xxx-controlled auto light 



> This is only here in case you want to manually control one of the autolights. Otherwise, the chip takes care of the auto light. 



---
#### Method CAP1XXX.Light.#ctor(IotFrosting.CAP1XXX,System.Int32)

 Constructor 

|Name | Description |
|-----|------|
|parent: |Capacitive controller who controls us|
|id: |Which light are we, starting at 0|


---
#### Property CAP1XXX.Light.AutoLight

 Whether we are automatically tied our corresponding pad 



---
#### Property CAP1XXX.Light.Value

 Current Analog light state 



---
#### Property CAP1XXX.Light.State

 Current binary light state 



---
#### Property CAP1XXX.Light.Brightness

 Ignored. Supplied for interface compliance 



---
#### Method CAP1XXX.Light.Toggle

 Toggle the state 



---
## Type ITick

 An object which needs to be updated regularly 



---
#### Method ITick.Tick

 Call regularly to update the status of this thing 



---
## Type Pimoroni.DrumHat.Pad

 One particular drum pad 



---
#### Property Pimoroni.DrumHat.Pad.Id

 Id of the key, starting with 0 



---
#### Method Pimoroni.DrumHat.Pad.#ctor(IotFrosting.CAP1XXX.Pad,IotFrosting.Pimoroni.ILight)

 Constructor 

|Name | Description |
|-----|------|
|light: |The light showing our state|


---
#### Property Pimoroni.DrumHat.Pad.AutoLight

 Whether the light is managed automatically 



---
#### Property Pimoroni.DrumHat.Pad.Light

 The light showing our state 



---
## Type Pimoroni.DrumHat.PadUpdateEventHandler

 Handler for Key.Updated events 

|Name | Description |
|-----|------|
|sender: |The key which was updated|
|args: |Empty args, may be used for expansion|


---
## Type Pimoroni.DrumHat.PadSet

 A combined set of keys, which shared a combined Updated event 



---
#### Method Pimoroni.DrumHat.PadSet.AddRange(System.Collections.Generic.IEnumerable{IotFrosting.IInput})

 Add keys into the set 

|Name | Description |
|-----|------|
|keys: |Keys to add|


---
#### Property Pimoroni.DrumHat.PadSet.Item(System.Int32)

 Extract a key by name 

|Name | Description |
|-----|------|
|id: |Identifier for this pad|
**Returns**: Pad with that id



---
#### Event Pimoroni.DrumHat.PadSet.Updated

 Raised every time any one of the keys are updated 



---
#### Field Pimoroni.DrumHat.PadSet.Pads

 Internal dictinoary of keys for fast lookup 



---
#### Method Pimoroni.DrumHat.Open

 Open a connection to the piano hat 

**Returns**: Piano Hat controller



---
#### Method Pimoroni.DrumHat.#ctor

 Don't call consturctor directly, use PianoHat.Open() 



---
#### Field Pimoroni.DrumHat.Cap

 Capacitive input controller 



---
## Type Pimoroni.IAnalogInput

 Interface for an analog input with an auto light 



---
#### Property Pimoroni.IAnalogInput.Voltage

 Current voltage on the input 



---
#### Method Pimoroni.IAnalogInput.Tick

 Call regularly to update the autolight 



---
## Type Pimoroni.AutomationHat

 The master control class for a single Automation Hat 



---
#### Method Pimoroni.AutomationHat.Open

 Open a connection to the hat 

**Returns**: An AutomstionHat object you can use to control the hat



---
#### Field Pimoroni.AutomationHat.Analog

 The analog inputs 



---
#### Field Pimoroni.AutomationHat.Input

 The digital inputs 



---
#### Field Pimoroni.AutomationHat.Output

 The outputs 



---
#### Field Pimoroni.AutomationHat.Relay

 The relays 



---
#### Field Pimoroni.AutomationHat.Light

 The user-controllable lights 



---
#### Method Pimoroni.AutomationHat.FastTick

 Called regularly from our own internal timer thread to update the state of everything 



---
#### Method Pimoroni.AutomationHat.SlowTick

 Called regularly from our own internal timer, less frequently 



> This is used for the ADC which takes some time, so we don't want the next interval coming along while we're still workign on the current one 



---
#### Method Pimoroni.AutomationHat.#ctor

 Constructor. Do not call directly. Use AutomationHat.Open() 



---
## Type Pimoroni.AutomationHat.Lights

 Class containing the user-controlled lights 



> We use this instead of a list so that each light can have easy names 



---
## Type Pimoroni.IDigitalInput

 Interface for a digital input line with an autolight 



---
## Type Pimoroni.Input

 A digital input with an automatic light showing its state 



---
#### Method Pimoroni.Input.#ctor(System.Int32,IotFrosting.Pimoroni.ILight,System.Boolean)

 Constructor 

|Name | Description |
|-----|------|
|pin: |The underlying pin we are watching|
|light: |The automatic light which shows the state|
|pulldown: |Whether the switch is wired to VCC (true) or GND (false)|


---
#### Property Pimoroni.Input.AutoLight

 Whether the autolight should in fact be updated with our state 



---
#### Property Pimoroni.Input.Light

 The light which shows our state automatically 



---
#### Method Pimoroni.Input.Tick

 Call regularly to update the status of the auto light 



> Recommend calling on your timer tick 



---
#### Field Pimoroni.Input.pressedhigh

 Whether the switch is wired to VCC (true) or GND (false) 



---
## Type Pimoroni.ILight

 Interface for a light with PWM control 



> Useful for dependency injection 



---
#### Property Pimoroni.ILight.State

 Whether the light is currently lit 



---
#### Method Pimoroni.ILight.Toggle

 Toggle the state 



---
#### Property Pimoroni.ILight.Value

 Value from 0.0-1.0 for an analog light 



---
#### Property Pimoroni.ILight.Brightness

 Default brightness value 0.0-1.0 when the light is lit 



---
## Type Pimoroni.IAutoLight

 Interface for something with an automatic light. 



> If you implement this interface, YOU are responsible for setting the light state 



---
#### Property Pimoroni.IAutoLight.AutoLight

 True if the light should be automatically set to match the state of the undelying component 



---
#### Property Pimoroni.IAutoLight.Light

 Access to the light related this component, so you can set it on or off yourself 



---
## Type Pimoroni.DirectLight

 A directly connected light (like an LED) 



> Wire the LED between the pin and VCC. pin to ground should turn light on. 



---
#### Method Pimoroni.DirectLight.#ctor(System.Int32,System.Boolean)

 Constructor 

|Name | Description |
|-----|------|
|pin: |Which GPIO pin we are wired to|
|activelow: |Whether the light is wired between pin and VCC (true) or pin and GND (false)|


---
#### Property Pimoroni.DirectLight.Value

 Current value of the light (0.0-1.0) 



> Currently only 0.0 and 1.0 are supported, as this is a digital light. For the sake of the ILight interface, we take a double value 



---
#### Property Pimoroni.DirectLight.State

 Whether the light is currently lit 



---
#### Property Pimoroni.DirectLight.Brightness

 Implemented for the interface. Ignored for a digital light. 



---
#### Field Pimoroni.DirectLight.ActiveLow

 Whether the light is wired between pin and VCC (true) or pin and GND (false) 



---
## Type Pimoroni.SingleAutoLight

 General-purpose automatic light which follows any pin 



---
#### Method Pimoroni.SingleAutoLight.#ctor(IotFrosting.IInput,System.Boolean,IotFrosting.Pimoroni.ILight)

 Constructor 

|Name | Description |
|-----|------|
|source: |Input to show status for|
|inverted: |Whether we are inverted from the usual logic. True if we show false when source is true.|
|light: |The particular light to control|


---
#### Property Pimoroni.SingleAutoLight.Light

 The particular light we are controlling 



---
#### Property Pimoroni.SingleAutoLight.AutoLight

 Whether we are connected to our controlled source 



---
#### Field Pimoroni.SingleAutoLight.Inverted

 Whether we are inverted from the usual logic. True if we show false when source is true 



---
#### Field Pimoroni.SingleAutoLight.Source

 Pin to show status for 



---
## Type Pimoroni.IDigitalOutput

 Interface for a digital output line with an autolight 



---
## Type Pimoroni.Output

 An output pin with a light showing its current state 



---
#### Method Pimoroni.Output.#ctor(System.Int32,IotFrosting.Pimoroni.ILight)

 Constructor 

|Name | Description |
|-----|------|
|pin: |Unerlying GPIO pin to output values|
|autolight: |Light which should show our current state|


---
#### Property Pimoroni.Output.AutoLight

 Whether the autolight should in fact be updated with our state 



---
#### Property Pimoroni.Output.Light

 The light which shows our state automatically 



---
## Type Pimoroni.Pad

 A drum pad, driven by a Cap1xxx 



> This is just a straight Input (including autolight), adding a 'Hit' event, raised only when the input goes high 



---
#### Method Pimoroni.Pad.#ctor(System.Int32,IotFrosting.Pimoroni.ILight)

 Constructor 

|Name | Description |
|-----|------|
|number: |Which input are we on the Cap1xxx bank?|
|light: |Automatic light|


---
#### Event Pimoroni.Pad.Hit

 Raised when we are first touched 



---
#### Property Pimoroni.Pad.AutoLight

 Whether the autolight should in fact be updated with our state 



---
#### Property Pimoroni.Pad.Light

 The light which shows our state automatically 



---
#### Method Pimoroni.Pad.Tick

 Call regularly to update the status of the auto light 



> Recommend calling on your timer tick 



---
#### Field Pimoroni.Pad.Number

 Which input ## are we on the Cap1xxx bank? 



---
## Type Pimoroni.PianoHat.KeyName

 All the valid key names on a PianoHat 



---
## Type Pimoroni.PianoHat.Key

 One particular piano key 



---
## Type Pimoroni.PianoHat.KeyUpdateEventHandler

 Handler for Key.Updated events 

|Name | Description |
|-----|------|
|sender: |The key which was updated|
|args: |Empty args, may be used for expansion|


---
## Type Pimoroni.PianoHat.KeySet

 A combined set of keys, which shared a combined Updated event 



---
#### Method Pimoroni.PianoHat.KeySet.AddRange(System.Collections.Generic.IEnumerable{IotFrosting.IInput})

 Add keys into the set 

|Name | Description |
|-----|------|
|keys: |Keys to add|


---
#### Property Pimoroni.PianoHat.KeySet.Item(IotFrosting.Pimoroni.PianoHat.KeyName)

 Extract a key by name 

|Name | Description |
|-----|------|
|name: |Name of a key|
**Returns**: Key with that name



---
#### Event Pimoroni.PianoHat.KeySet.Updated

 Raised every time any one of the keys are updated 



---
#### Field Pimoroni.PianoHat.KeySet.Keys

 Internal dictinoary of keys for fast lookup 



---
#### Method Pimoroni.PianoHat.Open

 Open a connection to the piano hat 

**Returns**: Piano Hat controller



---
#### Method Pimoroni.PianoHat.#ctor

 Don't call consturctor directly, use PianoHat.Open() 



---
#### Field Pimoroni.PianoHat.Cap1

 Left-side cap1xxx 



---
#### Field Pimoroni.PianoHat.Cap2

 Right-side cap1xxx 



---
## Type Pimoroni.IRelay

 Interface for a relay with two autolights 



---
## Type Pimoroni.Relay

 Control a relay with two autolights, one for each state 



---
#### Method Pimoroni.Relay.#ctor(System.Int32,IotFrosting.Pimoroni.ILight,IotFrosting.Pimoroni.ILight)

 Constructor 

|Name | Description |
|-----|------|
|pin: |Pin where the relay is connected|
|light_no: |Light to show state when normally-open side is connected (that is, relay is closed)|
|light_nc: |Light to show state when normally-closed side is connected (that is, relay is open)|


---
#### Property Pimoroni.Relay.NO

 The Normally open light. The IAutoLight is public in the IRelay interface 



---
#### Property Pimoroni.Relay.NC

 The Normally closed light. The IAutoLight is public in the IRelay interface 



---
## Type DS3231

 Control for the DS3231 real-time clock 



---
#### Method DS3231.Open

 Open a connection to the DS3231 chip 

**Returns**: DS3231 object to control the clock



---
#### Property DS3231.Now

 The current time 



> Be sure to call Tick() first to ensure the time is set 



---
#### Method DS3231.Tick

 Call regularly to update the time 



---
#### Method DS3231.#ctor

 Do not call direcrtly, use Open() 



---
#### Method DS3231.decToBcd(System.Int32)

 Convert normal decimal numbers to binary coded decimal 

|Name | Description |
|-----|------|
|val: ||
**Returns**: 



---
## Type IInput

 The most basic kind of input, which can be used for more complex types 



---
#### Property IInput.State

 Whether the line is currently HIGH 



---
#### Event IInput.Updated

 Raised when State changes 



---
## Type InputUpdateEventHandler

 Delegate for use in IInput.Updated event 

|Name | Description |
|-----|------|
|sender: |The input being update|
|args: |Empty ags for now, may be extended in the future.|


---
## Type IPin

 Minimum generic interface for all pins, input or output 



> Useful for dependency injection 



---
## Type IOutputPin

 Generic interface for output pins 



> Useful for dependency injection 



---
#### Property IOutputPin.State

 Whether the line is currently HIGH 



---
#### Method IOutputPin.Toggle

 Toggle the state 



---
## Type OutputPin

 Wraps the platform GpioPin with extra functionality 



---
#### Method OutputPin.#ctor(System.Int32)

 Constructor 

|Name | Description |
|-----|------|
|pin: |Which pin to control|


---
#### Event OutputPin.Updated

 Raised when the pin changes state 



---
#### Property OutputPin.State

 Current state of the pin, true is High 



---
#### Method OutputPin.Toggle

 Toggle the state 



---
#### Method OutputPin.Dispose

 Return the pin back to the system, we're done! 



---
#### Field OutputPin.Pin

 The underlying pin we are controlling 



---
## Type InputPin

 Wraps the platform GpioPin with extra functionality 



---
#### Method InputPin.#ctor(System.Int32,System.Boolean)

 Constructor 

|Name | Description |
|-----|------|
|pin: |Which pin to control|
|pulldown: |Whether we pull down to ground (true) or up to 3v3 (false)|


---
#### Event InputPin.Updated

 Raised when the value is updated 



---
#### Property InputPin.State

 Current state of the pin, true is high 



---
#### Field InputPin.Pin

 The underlying pin we are controlling 



---
#### Method InputPin.Dispose

 Return the pin to the system 



---
## Type SN3218

 Control an SN3218 18-port LED driver 



> Influenced by: https://github.com/pimoroni/sn3218/blob/master/library/sn3218.py and https://github.com/ms-iot/samples/blob/develop/I2cPortExpander/CS/MainPage.xaml.cs 



---
#### Method SN3218.Tick

 Call regularly to update the hardware from the software light values 



---
#### Method SN3218.#ctor

 Do not construct directly. Use Open() 



---
#### Field SN3218.NumberOfLights

 How many total lights are there in an SN3218 bank 



---
## Type SN3218.Light

 A light with PWM control, which is part of a bank of 18 lights. 



---
#### Property SN3218.Light.State

 Digital state, true if on, false if off 



---
#### Property SN3218.Light.Value

 PWM brightness value (0.0-1.0) 



---
#### Property SN3218.Light.Brightness

 How bright the light is when it's on 



---
#### Method SN3218.Light.#ctor(System.Int32)

 Constructor 

|Name | Description |
|-----|------|
|number: |Which light number on the SN3218 bank are we|


---
#### Event SN3218.Light.Updated

 Raised when the value of the light changes 



---
#### Field SN3218.Light.Number

 Which light ## are we on the SM3218 bank? 



---
#### Field SN3218.Light.Values

 All of the light values 



---



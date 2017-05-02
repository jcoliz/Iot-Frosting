# IotFrosting #

## Type Pimoroni.IAnalogInput

 Interface for an analog input with an auto light 



> Incomplete. Analog inputs are not yet implemented. 



---
#### Method Pimoroni.IAnalogInput.Tick

 Call regularly to update the autolight 



---
#### Method Pimoroni.AnalogInput.Tick

 Call regularly to update the status of the auto light 



> Recommend calling on your timer tick 



---
#### Field Pimoroni.AnalogInput.Values

 All of the analog input values 



---
#### Field Pimoroni.AnalogInput.NumberOfAnalogInputs

 How many total lights are there in an SN3218 bank 



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
#### Method Pimoroni.AutomationHat.Tick

 Called regularly from our own internal timer thread to update the state of everything 



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
#### Method Pimoroni.IDigitalInput.Tick

 Call regularly to update the autolight 



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
## Type Pimoroni.Light

 A light with PWM control, which is part of a bank of 18 lights. Suitable for use with SN3218. 



---
#### Property Pimoroni.Light.State

 Digital state, true if on, false if off 



---
#### Property Pimoroni.Light.Value

 PWM brightness value (0.0-1.0) 



---
#### Property Pimoroni.Light.Brightness

 How bright the light is when it's on 



---
#### Method Pimoroni.Light.#ctor(System.Int32)

 Constructor 

|Name | Description |
|-----|------|
|number: |Which light number on the SN3218 bank are we|


---
#### Event Pimoroni.Light.Updated

 Raised when the value of the light changes 



---
#### Field Pimoroni.Light.Number

 Which light ## are we on the SM3218 bank? 



---
#### Field Pimoroni.Light.Values

 All of the light values 



---
#### Field Pimoroni.Light.NumberOfLights

 How many total lights are there in an SN3218 bank 



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
#### Method Pimoroni.SingleAutoLight.#ctor(IotFrosting.IPin,System.Boolean,IotFrosting.Pimoroni.ILight)

 Constructor 

|Name | Description |
|-----|------|
|source: |Pin to show status for|
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
## Type IPin

 Minimum generic interface for all pins, input or output 



> Useful for dependency injection 



---
#### Property IPin.State

 Whether the line is currently HIGH 



---
#### Event IPin.Updated

 Raised when State changes 



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
#### Method SN3218.#ctor

 Do not construct directly. Use Open() 



---



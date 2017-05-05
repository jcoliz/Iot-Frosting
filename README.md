## Iot Frosting

C# library and examples for selected hardware components on Windows 10 IoT Core, including the Pimoroni Automation HAT.

* [Pimoroni Automation Hat](https://shop.pimoroni.com/products/automation-hat) (Also at [Adafruit](https://www.adafruit.com/product/3289))
* [Pimoroni Piano Hat](https://shop.pimoroni.com/products/piano-hat)
* [DS3231 Real Time Clock](https://www.adafruit.com/product/3013)
* SN3218 18-channel LED Driver ([DataSheet](http://www.si-en.com/uploadpdf/s2011517171720.pdf))
* [ADS1015 12-bit 4-channel ADC](https://www.adafruit.com/product/1083) ([DataSheet](https://cdn-shop.adafruit.com/datasheets/ads1015.pdf))

## Piano HAT

Piano HAT is a tiny Pi piano with 16 touch-sensitive buttons. It features:

* 16 Capacitive Touch Buttons
* 13 Notes from C to C
* Octave Up/Down
* Instrument Select

### Documentation & Support

* [Guides and tutorials](https://learn.pimoroni.com/piano-hat)
* [GPIO Pinout](https://pinout.xyz/pinout/piano_hat)
* [Get help](http://forums.pimoroni.com/c/support)

### Namespace

At the top of your C# file, reference the namespace for the library, like so:

```c#
using IotFrosting.Pimoroni
```

### Open the device

Before using the device, you'll need to open a connection to it, and wrap your code in a 'using', like so:

```c#
using (var Hat = await PianoHat.Open())
{
}
```

### Notes

The hat raises an event when a note is pressed or released.

```c#
Hat.Notes.Updated += (s,e) => 
{ 
    if (s.State)
    {
        switch (s.Name)
        {
        case PianoHat.KeyNames.C:
            Media.Play("C.WAV");
            break;
        case PianoHat.KeyNames.D:
            Media.Play("D.WAV");
            break;
        }
    }
};
```

### Octave Up/Down

The hat raises an event when the octave up or down keys is pressed or released.

```c#
Hat.OctaveUp.Updated += (s,e) => Log("Octave Up " + s.State?"Pressed":"Released");
Hat.OctaveDown.Updated += (s,e) => Log("Octave Down " + s.State?"Pressed":"Released");
```

### Instrument Key

The hat raises an event when the Instrument key is pressed or released.

```c#
Hat.Instrument.Updated += (s,e) => Log("Instrument " + s.State?"Pressed":"Released");
```

## Automation HAT

Automation HAT is a home monitoring and automation controller featuring relays, analog channels, powered outputs, and buffered inputs (all 24V tolerant).

### Documentation & Support

* [Guides and tutorials](https://learn.pimoroni.com/automation-hat)
* [GPIO Pinout](https://pinout.xyz/pinout/automation_hat)  
* [Get help](http://forums.pimoroni.com/c/support)

### Namespace

At the top of your C# file, reference the namespace for the library, like so:

```c#
using IotFrosting.Pimoroni
```

### Open the device

Before using the device, you'll need to open a connection to it, and wrap your code in a 'using', like so:

```c#
using (var Hat = await AutomationHat.Open())
{
}
```

### Inputs

The three inputs on Automation HAT are 24V tolerant, switching on at 3V and off at 1V. Behaviour at voltages between 1V and 3V is undefined.

You can read an input like so:

```c#
bool state = Hat.Input[0].State;
```

Inputs raise an event when they transition their state. You can listen to this event to take action. 
In this example, we toggle output 0 whenever a button connected to input 0 is pressed.

```c#
Hat.Input[0].Updated += (s, a) =>
{
    if (s.State)
        Hat.Output[0].Toggle();
};
```

### Outputs

The three outputs on Automation HAT are 24V tolerant, sinking outputs. That means you should connect them between your load and ground. They act like a switch down to ground, toggling your load on and off.

You can turn an output on like so:

```c#
Hat.Output[0].State = true;
```

### Lights

Automation HAT includes three user-controllable lights: Power, Comms and Warn. You can take control of these lights to turn them on/off:

```c#
Hat.Light.Comms.State = true;
```

By default, 'true' sets a light to 1.0 brightness, and 'false' sets a light to 0.0. The lights are very bright. To adjust the default brightness level for 'true', you can set the Brightness property. This does not have any immediate impact on the light. Instead, it controls what happens next time you set the State to 'true'

```c#
Hat.Light.Comms.Brightness = 0.2;
```

You can directly set a brightness value (0.0-1.0). The lights are automatically gamma-corrected.
 
```c#
Hat.Light.Comms.Value = 0.8;
```

Lights also have the same properties and methods as relays and outputs: State and Toggle().

The Power light is automatically turned on when you open a connection to the Hat, and turned off when the connection is closed. This is useful so that you know your app is running, especially becuase Windows 10 IoT Core can take a full minute to boot and start the default app. You can turn it off immediately if you don't like that.

Lights associated with Inputs, Outputs, Relays and Analog are automatic by default, but you can switch them to manual if you want. First turn off the automation:

```c#
Hat.Input[0].AutoLight = false;
```

Then toggle the light:

```c#
Hat.Input[0].Light.State = true;
Hat.Input[0].Light.State = false;
```

### Relays

The three relays on Automation HAT supply both NO (Normally Open) and NC (Normally Closed) terminals. You can use them to switch a single load, or alternate between two. The relays should be placed between the voltage supply and your load.

You can turn a relay on like so:

```c#
Hat.Relay[0].State = true;
```

Or off:

```c#
Hat.Relay[0].State = false;
```

Toggle it from its previous state:

```c#
Hat.Relay[0].Toggle()
```

Or write a specific value:

```c#
Hat.Relay[0].State = true;
Hat.Relay[0].State = false;
```

Relays have two lights associated with them, one showing the relay is connecting the NC circuit, and another showing it's connecting the NO circuit.

You can take control of these lights independently, and turn them on and off at will.

```c#
Hat.Relay[0].NO.AutoLight = false;
Hat.Relay[0].NO.Light.State = true;
```

By default the NC lights are not enabled, because NC is the default position, so you can infer NC from the NO light being off.
If you don't like this, you can enable the NC light like so:

```c#
Hat.Relay[0].NC.AutoLight = true;
```

### Analog

Three of the four analog inputs on Automation HAT are 24V tolerant, with a forth 3.3V input in the breakout header.

You can read the voltage currently applied to an analog input like so:

```c#
double voltage = Hat.Analog[0].Voltage;
```

The 12V analog inputs have lights which automatically illuminate to a level of brightness corresponding to the voltage level on the input. For example, if there is 12V on an input, its light will show half brightness. Just as with any other automatic lights, you can take control of the light independently:

```c#
Hat.Analog[0].AutoLight = false;
Hat.Analog[0].Light.State = true;
```

## DS3231 Real Time Clock

### Namespace

At the top of your C# file, reference the namespace for the library, like so:

```c#
using IotFrosting
```

### Open the device

Before using the device, you'll need to open a connection to it, and wrap your code in a 'using', like so:

```c#
using (var Clock = await DS3231.Open())
{
}
```

### Update the time

Update the time as frequently as you need. I recommend doing this in your timer tick, or you could do it just before you check the time.

```c#
Clock.Tick();
```

### Get the time

```c#
DateTime whattimeisit = Clock.Now;
```

### Change the time

You can change the time, just by setting the Now property. This doesn't take effect until you call Tick() next.

```C#
Clock.Now = DateTime.Parse("12/31/2017 9:30 PM");
Clock.Tick();
```

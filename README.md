## Automation HAT

https://shop.pimoroni.com/products/automation-hat  

Automation HAT is a home monitoring and automation controller featuring relays, analog channels, powered outputs, and buffered inputs (all 24V tolerant).

## Goal

This project provides a C# library for this HAT usable on Windows 10 IoT Core

## Documentation & Support

* Guides and tutorials  
https://learn.pimoroni.com/automation-hat  
https://learn.pimoroni.com/automation-phat  
* GPIO Pinout  
https://pinout.xyz/pinout/automation_hat  
https://pinout.xyz/pinout/automation_phat  
* Get help with the HAT
http://forums.pimoroni.com/c/support

## Function Reference

### Namespace

At the top of your C# file, reference the namespace for the library, like so:

```c#
using Pimoroni.MsIot
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
    if (Hat.Input[0].State)
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

Automation HAT includes three user-controllable lights: Power, Comms and Warn. You can take control of these lights to turn them on/off or write a brightness value:

```c#
Hat.Light.Comms.State = true;
```

```c#
Hat.Light.Comms.State = false;
```

Note: lights use the same properties and methods as relays and outputs: State and Toggle().

The Power light is automatically turned on when you open a connection to the Hat, and turned off when the connection is closed. You can turn it off immediately if you don't like that.

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
Hat.Input[0].NO.Light.State = true;
```

By default the NC lights are not enabled, because NC is the default position, so you can infer NC from the NO light being off.
If you don't like this, you can enable the NC light like so:

```c#
Hat.Relay[0].NC.AutoLight = true;
```

### Analog

Three of the four analog inputs on Automation HAT are 24V tolerant, with a forth 3.3V input in the breakout header.

You can read an analog input like so:

```c#
int value = Hat.Analog[0].Value;
```

NOTE: Analog inputs are not yet implemented.

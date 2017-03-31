https://shop.pimoroni.com/products/automation-hat  

Automation HAT is a home monitoring and automation controller featuring relays, analog channels, powered outputs, and buffered inputs (all 24V tolerant).

## Goal

This project has ambition to provide a C# library for this HAT usable on Windows 10 IoT Core

## Documentation & Support

* Guides and tutorials  
https://learn.pimoroni.com/automation-hat  
https://learn.pimoroni.com/automation-phat  
* Function reference  
https://github.com/pimoroni/automation-hat/tree/master/documentation
* GPIO Pinout  
https://pinout.xyz/pinout/automation_hat  
https://pinout.xyz/pinout/automation_phat  
* Get help  
http://forums.pimoroni.com/c/support

## Automation HAT / pHAT Function Reference

```c#
using Pimoroni.MsIot
```


### Analog

Three of the four analog inputs on Automation HAT are 24V tolerant, with a forth 3.3V input in the breakout header.

You can read an analog input like so:

```c#
int value = AutomationHat.Analog.One.Value;
```

### Inputs

The three inputs on Automation HAT are 24V tolerant, switching on at 3V and off at 1V. Behaviour at voltages between 1V and 3V is undefined.

You can read an input like so:

```c#
bool state = AutomationHat.Analog.One.State;
```

### Outputs

The three outputs on Automation HAT are 24V tolerant, sinking outputs. That means you should connect them between your load and ground. They act like a switch down to ground, toggling your load on and off.

You can turn an output on like so:

```c#
AutomationHat.Analog.One.State = true;
```

### Relays

The three relays on Automation HAT supply both NO (Normally Open) and NC (Normally Closed) terminals. You can use them to switch a single load, or alternate between two. The relays should be placed between the voltage supply and your load.

You can turn a relay on like so:

```c#
AutomationHat.Relay.One.State = true;
```

Or off:

```c#
AutomationHat.Relay.One.State = false;
```

Toggle it from its previous state:

```c#
AutomationHat.Relay.One.Toggle()
```

Or write a specific value:

```c#
AutomationHat.Relay.One.State = true;
AutomationHat.Relay.One.State = false;
```

### Lights

Automation HAT includes three user-controllable lights: Power, Comms and Warn. You can take control of these lights to turn them on/off or write a brightness value:

```c#
AutomationHat.Light.Comms.State = true;
```

```c#
AutomationHat.Light.Comms.State = false;
```

Note: lights use the same methods as relays and outputs: `on`, `off`, `toggle` and `write`.

Lights associated with Inputs, Outputs, Relays and Analog are automatic by default, but you can switch them to manual if you want. First turn off the automation:

```c#
AutomationHat.Analog.One.IsAutoLight = false;
```

Then toggle the light:

```python
AutomationHat.Analog.One.Light.State = true;
AutomationHat.Analog.One.Light.State = false;
```

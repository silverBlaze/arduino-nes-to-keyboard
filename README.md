arduino-nes-to-keyboard
=======================

Classic NES Controller-To-USB Interface (using an Arduino and DotNet)

Use an Arduino to connect a classic NES controller to your PC and emulate keyboard events on button-up and button-down events. Includes entire code for the required Arduino program and CSharp console application.

About ArduinoNESToKeyboard
==========================
This open-source project allows you to use an Arduino UNO to connect a classic NES controller to your PC and emulate keyboard events on button-up and button-down events. Includes fully-commented code for the Arduino program (with no dependencies) and a CSharp console application (.NET Framework 4 Client Profile.)

The included Arduino sketch polls a connected NES controller for the state of its buttons, and sends a single byte to the serial port. The CSharp program listens on the serial port and raises events when new controller data is available. Then, based on the .config file's mapping, keyboard key-down/up events are emulated to match the corresponding button-down/up event.

After hooking up the NES Controller to your Arduino UNO and uploading the provided sketch, update the CSharp application's .config file with your Arduino's serial port (e.g.- "COM1") and the baud-rate of its connection (default used in the provided Arduino code is 57600.) Then provide the desired button-to-key mappings in the same format as the example provided. See the included keys.txt file to see all available Keyboard key config values.

Note: Make sure you update the .config file in the \bin directory. The .config example included is used to map keyboard controls for DuckTales Remastered (http://store.steampowered.com/app/237630/) and has been tested extensively with no issues.

Once the Arduino is plugged into USB and running its sketch (and the NES controller is hooked up,) you can start the CSharp application. It will open in a console window, and as long as it is running, NES controller button presses will execute the configured keyboard emulation.

Resulting keyboard emulation should occur in any window that has focus, so you should then be able to start up your game (or whatever application) after starting the CSharp application. You can exit the application by pressing Q after focusing on the console window.


NES Controller Pinout
=====================

```
While looking at the end of the controller plug:
                      ____
                     /  0 | <-- Ground (white)
   +5V (orange) --> | 0 0 | <-- Clock (blue)
 (*zapper only) --> | 0 0 | <-- Strobe (green)
 (*zapper only) --> | 0 0 | <-- Data (brown)
                    |_____|
```

*Zapper connections are not used in this project.

Note: Connections are deep in the controller plug and may require long leads to reach. In case you are altering the controller wires, wire colors have been provided. Wire colors may actually differ on various controllers, but the actual pinout shouldn't be different.


Connecting the NES Controller to the Arduino UNO
================================================
The provided sketch has only been tested with an Arduino UNO and an Arduino Nano w/ ATmega328. Other Arduino versions may work too, but I have not had any opportunity to test them.

Note: You may use whatever digital pins you have available. Ones provided in this guide are just examples which I used and worked fine in my application.

Using the above Pinout as a guide, connect the given NES Controller pin to the provided Arduino pin:

| NES Controller | Arduino Pin          |
|----------------|----------------------|
|Ground (white)	 |GND (Ground Pin)|
|+5v (orange)	 |5V (5-volt Output Pin)|
|Strobe (green)	 |D2 (Digital Pin 2)|
|Clock (blue)	 |D3 (Digital Pin 3)|
|Data (brown)	 |D4 (Digital Pin 4)|


Copyright and Licensing
=======================

Copyright 2014 Jason Wells

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

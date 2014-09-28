using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ArduinoNESToKeyboard {
    /// <summary>
    /// Console application object
    /// </summary>
    /// <remarks>
    /// See end of this file for copyright and licensing information
    /// </remarks>
    class Program {
        private static NESController _controller; //object representing the NES controller
        private static List<NESToKeyboardMapping> _nes2KeyMap; //list of button-to-key maps
        private static string _serialPort = "COM1"; //default serial port name
        private static int _serialBaud = 57600; //default baud rate in Arduino code

        /// <summary>
        /// Main program function; contains the main run-loop
        /// </summary>
        static void Main() {
            bool run = true;
            ConsoleKeyInfo keyRead;
            try {
                Console.WriteLine("{0}\nArduinoNESToKeyboard\nAn Arduino-powered NES Controller-To-Keyboard Mapper Utility\n", new string('-', 55));
                WriteCopyRightLicenseInfo();
                Console.WriteLine("Edit the ArduinoNESToKeyboard.exe.config file to change this utility's behavior.");
                
                //try and load config, exit program if failed
                if (!LoadConfig()) {
                    return;
                }
                Console.WriteLine("Configuration loaded...");
                
                //try and connect to the arduino via serial and setup read-event
                using (var asi = new ArduinoSerialInterface(_serialPort, _serialBaud)) {
                    asi.SerialIntRead += new ArduinoSerialInterface.ReadHandler(OnSerialIntRead);
                    asi.Start();
                    Console.WriteLine("Arduino connection open...");

                    //load up the controller object and events
                    _controller = new NESController();
                    _controller.ButtonDown += new NESController.ButtonHandler(OnControllerButtonDown);
                    _controller.ButtonUp += new NESController.ButtonHandler(OnControllerButtonUp);

                    //main run-loop
                    Console.WriteLine("Button-To-Keyboard map is now up and running...");
                    Console.WriteLine("\nPress Q to quit");
                    while (run) {
                        keyRead = Console.ReadKey(true);
                        if (keyRead.Key == ConsoleKey.Q) {
                            asi.Stop();
                            run = false;
                        }
                    }
                }
            } catch (Exception ex) {
                LogError(ex.ToString());
            }
        }

        /// <summary>
        /// Event handler for when new int value is read from the serial port
        /// </summary>
        /// <param name="e">Event argument containing the value read from the serial port</param>
        static void OnSerialIntRead(SerialReadEventArgs e) {
            //invert bit values to create a comparison mask (11011011 becomes 00100100)
            int invertedCtrlrState = ~(int)e.Value;
            //notify controller of new button-state
            _controller.UpdatePressedButtons((NESButtons)invertedCtrlrState);
        }

        /// <summary>
        /// Event handler for when a button is pressed on the controller
        /// </summary>
        /// <param name="e">Event argument containing the button pressed</param>
        static void OnControllerButtonDown(NESController.ButtonEventArgs e) {
            try {
                //use linq to get the pressed buttons' mappings
                var mappings = from maps in _nes2KeyMap
                               where maps.NESButton == e.Button
                               select maps;
                //execute each matching key-down
                foreach (var mapping in mappings) {
                    KeyInputEmulator.EmulateKeyDown(mapping.ScanKey, mapping.VKey);
                }
            } catch (Exception ex) {
                LogError(ex.ToString());
            }
        }

        /// <summary>
        /// Event handler for when a button is released on the controller
        /// </summary>
        /// <param name="e">Event argument containing the button released</param>
        static void OnControllerButtonUp(NESController.ButtonEventArgs e) {
            try {
                //use linq to get the released buttons' mappings
                var mappings = from maps in _nes2KeyMap
                               where maps.NESButton == e.Button
                               select maps;
                //execute each matching key-up
                foreach (var mapping in mappings) {
                    KeyInputEmulator.EmulateKeyUp(mapping.ScanKey, mapping.VKey);
                }
            } catch (Exception ex) {
                LogError(ex.ToString());
            }
        }

        /// <summary>
        /// Loading of the configuration, getting serial settings and button-to-key mappings
        /// </summary>
        /// <returns>False if it fails during load, otherwise true.</returns>
        private static bool LoadConfig() {
            NESButtons nesButton;
            _nes2KeyMap = new List<NESToKeyboardMapping>();
            string mapVal;
            string[] mapVals;
            ScanCodeShort scanCode;
            VirtualKeyShort virtualCode;
            try {
                //load arduino serial connection settings
                if (ConfigurationManager.AppSettings["arduinoPortName"] != null) {
                    _serialPort = ConfigurationManager.AppSettings["arduinoPortName"];
                }
                if (ConfigurationManager.AppSettings["arduinoBaudRate"] != null) {
                    string baudVal = ConfigurationManager.AppSettings["arduinoBaudRate"];
                    Int32.TryParse(baudVal, out _serialBaud);
                }
                //look for nesToKeyboardMapping settings
                foreach (var appSetting in ConfigurationManager.AppSettings.AllKeys) {
                    if (appSetting.StartsWith("nesToKeyboardMapping")) {
                        //parse the enums from config value strings and save as a button-to-key mapping
                        mapVal = ConfigurationManager.AppSettings[appSetting];
                        mapVals = mapVal.Split(new char[] { ',' });
                        if(!Enum.TryParse<NESButtons>(mapVals[0], out nesButton))
                            throw new Exception("Invalid NESButton Name value");
                        if(!Enum.TryParse<ScanCodeShort>(mapVals[1], out scanCode)) 
                            throw new Exception("Invalid ScanCode KeyName value");
                        if(!Enum.TryParse<VirtualKeyShort>(mapVals[1], out virtualCode)) 
                            throw new Exception("Invalid Virtual KeyName value");
                        _nes2KeyMap.Add(new NESToKeyboardMapping() { ScanKey = scanCode, VKey = virtualCode, NESButton = nesButton });
                    }
                }
            } catch (Exception ex) {
                LogError(ex.ToString());
                return false;
            }
            return true;
        }

        private static void LogError(string p) {
            Console.WriteLine("[{0:MM/dd/yyyy HH:mm:ss}] {1}", DateTime.Now, p);
        }

        /// <summary>
        /// Class to represent each button-to-key mapping
        /// </summary>
        private class NESToKeyboardMapping {
            public NESButtons NESButton { get; set; }
            public VirtualKeyShort VKey { get; set; }
            public ScanCodeShort ScanKey { get; set; }
        }

        private static void WriteCopyRightLicenseInfo() {
            Console.WriteLine("Copyright (c) 2014  Jason Wells");
            Console.WriteLine("Licensed under the Apache License, Version 2.0 (the \"License\");\na copy of the License is provided as license.txt.");
            Console.WriteLine("Unless required by applicable law or agreed to in writing, software\ndistributed under the License is distributed on an \"AS IS\" BASIS,");
            Console.WriteLine("WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.");
            Console.WriteLine("See the License for the specific language governing permissions and\nlimitations under the License.\n{0}\n", new string('-', 55));
        }
    }
}
/*
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
*/
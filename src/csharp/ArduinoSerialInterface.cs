using System;
using System.IO.Ports;
using System.Threading;

namespace ArduinoNESToKeyboard {
    /// <summary>
    /// Object for communicating with an Arduino over a serial port
    /// </summary>
    /// <remarks>
    /// See end of this file for copyright and licensing information
    /// </remarks>
    public class ArduinoSerialInterface : IDisposable {
        private bool _run;
        private SerialPort _serialPort = null;
        private Thread _readThread = null;

        /// <summary>
        /// Event that fires when a new integer value is received from the Arduino
        /// </summary>
        public event ReadHandler SerialIntRead;

        /// <summary>
        /// Delegate for Serial port read event handling
        /// </summary>
        /// <param name="e">Arguments containing the read value from the serial port</param>
        public delegate void ReadHandler(SerialReadEventArgs e);

        /// <summary>
        /// Create a new interface for reading values sent from the Arduino
        /// </summary>
        /// <param name="portName">The name of the serial port to which the Arduino is connected</param>
        /// <param name="baudRate">The baud rate of serial communications with the Arduino</param>
        public ArduinoSerialInterface(string portName, int baudRate) {
            StringComparer noCaseComp = StringComparer.OrdinalIgnoreCase;
            _readThread = new Thread(ReadSerial);
            _serialPort = new SerialPort();
            _serialPort.BaudRate = baudRate;
            _serialPort.PortName = portName;
            _serialPort.ReadTimeout = Int32.MaxValue; //since communication isn't constant, max out timeout value
        }

        /// <summary>
        /// Start listening on the serial interface
        /// </summary>
        public void Start() {
            _run = true;
            _serialPort.Open();
            _readThread.Start();
        }

        /// <summary>
        /// Stop listening on the serial interface
        /// </summary>
        public void Stop() {
            _run = false;
            if(_serialPort != null)
                _serialPort.Close();
            if(_readThread != null)
                _readThread.Join();
        }

        /// <summary>
        /// Thread method for reading the serial port and raising Read events
        /// </summary>
        private void ReadSerial() {
            int val;
            while (_run) {
                try {
                    string message = _serialPort.ReadLine();
                    if (Int32.TryParse(message, out val)) {
                        //raise event saying new int value was read
                        SerialIntRead(new SerialReadEventArgs() { Value = val });
                    }
                } catch (System.IO.IOException) {
                    //HACK: Ignoring these exceptions because of IOException thrown when exiting
                    //TODO: Determine why IOException being thrown when exiting
                } catch (Exception ex) {
                    LogError(ex.ToString());
                }
            }
        }

        private void LogError(string p) {
            Console.WriteLine("[{0:MM/dd/yyyy HH:mm:ss}] {1}", DateTime.Now, p);
        }

        public void Dispose() {
            Stop();
            if (_serialPort != null) {
                _serialPort.Dispose();
            }
        }
    }

    /// <summary>
    /// Event arguments for reading a value sent over serial
    /// </summary>
    public class SerialReadEventArgs : EventArgs {
        public object Value { get; set; }
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
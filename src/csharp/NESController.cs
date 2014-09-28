using System;

namespace ArduinoNESToKeyboard {
    /// <summary>
    /// Object used to represent the NESController
    /// </summary>
    /// <remarks>
    /// See end of this file for copyright and licensing information
    /// </remarks>
    public class NESController {
        private NESButtons _buttonState;

        /// <summary>
        /// Event that fires when a button is pressed
        /// </summary>
        public event ButtonHandler ButtonUp;

        /// <summary>
        /// Event that fires when a button is released
        /// </summary>
        public event ButtonHandler ButtonDown;

        /// <summary>
        /// Delegate for button press/release event handling
        /// </summary>
        /// <param name="e"></param>
        public delegate void ButtonHandler(ButtonEventArgs e);
        
        /// <summary>
        /// Contructor requiring the current pressed button state
        /// </summary>
        /// <param name="pressedBtns">Current pressed button state (default to none-pressed)</param>
        public NESController(int pressedBtns = (int)NESButtons.None) {
            _buttonState = (NESButtons)pressedBtns;
        }

        /// <summary>
        /// Instance method for comparing current instance's button state to the provided mask.
        /// </summary>
        /// <param name="buttonMask">An integer bitmask representing a query for whether a the button is pressed (as an NESButton enum)</param>
        /// <returns>True if the current state indicates the button represented by buttonMask is pressed</returns>
        public bool IsPressed(NESButtons buttonMask) {
            return NESController.IsPressed(_buttonState, buttonMask);
        }
        
        /// <summary>
        /// Static method for comparing a given pressed-button state with a provided mask
        /// </summary>
        /// <param name="buttonsPressed">An integer bitmask representing the pressed-button state (as an NESButton enum)</param>
        /// <param name="buttonMask">An integer bitmask representing a query for whether a button is pressed (as an NESButton enum)</param>
        /// <returns>True if the provided pressed-state indicates the button represented by buttonMask is pressed</returns>
        public static bool IsPressed(NESButtons buttonsPressed, NESButtons buttonMask) {
            return (buttonsPressed & buttonMask) == buttonMask;
        }

        /// <summary>
        /// Method for another interface to update the current state of the controller buttons, and fire resulting events
        /// </summary>
        /// <param name="btnsNow">An integer bitmask representing the new pressed-button state (as an NESButton enum)</param>
        internal void UpdatePressedButtons(NESButtons btnsNow) {
            ButtonEventArgs args = new ButtonEventArgs();
            NESButtons btnsBefore;

            if (_buttonState != btnsNow) {
                btnsBefore = _buttonState;
                _buttonState = btnsNow;
                foreach (var btn in _allButtons) {
                    if (IsPressed(btnsBefore, btn) != IsPressed(btnsNow, btn)) {
                        args.Button = btn;
                        if (IsPressed(btnsBefore, args.Button))
                            ButtonUp(args);
                        else
                            ButtonDown(args);
                    }
                }
            }
        }
        
        /// <summary>
        /// A static array of all the possible button values (for internal use in UpdatePressedButtons)
        /// </summary>
        private static NESButtons[] _allButtons = new NESButtons[] { 
            NESButtons.A, 
            NESButtons.B, 
            NESButtons.Up, 
            NESButtons.Down, 
            NESButtons.Left, 
            NESButtons.Right,
            NESButtons.Select,
            NESButtons.Start
        };
    
        /// <summary>
        /// Event arguments for NES controller button events
        /// </summary>
        public class ButtonEventArgs : EventArgs {
            public NESButtons Button { get; set; }
        }
    }

    [Flags]
    public enum NESButtons {
        None = 0,
        Right = 1,
        Left = 1 << 1,
        Down = 1 << 2,
        Up = 1 << 3,
        Start = 1 << 4,
        Select = 1 << 5,
        B = 1 << 6,
        A = 1 << 7
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
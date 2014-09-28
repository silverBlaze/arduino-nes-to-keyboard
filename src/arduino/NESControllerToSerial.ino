////
// Arduino program for polling a NES controller's interface and communicating its state changes over a PC serial port
/* 
  This code is a lightly modified version of the code available at https://code.google.com/p/nes-controller-arduino-serial/, licensed under GNU GPL v3 (http://www.gnu.org/licenses/gpl.html)
  See end of this file for its copyright and licensing information.
*/

int baud = 57600; //The baud rate for the PC serial connection
int strobe = 2; //The digital pin which is hooked to the NES controller strobe wire
int clock = 3; //The digital pin which is hooked to the NES controller clock wire
int data = 4; //The digital pin which is hooked to the NES controller data wire
byte controller_data = 0; //The current state of the controller buttons
byte last_data = 0; //The previous state of the controller buttons

////
// The setup method which sets up the serial connection and digital pins
void setup() {
  Serial.begin(baud);
  pinMode(strobe, OUTPUT);
  pinMode(clock, OUTPUT);
  pinMode(data, INPUT);
  
  //start the strobe and clock with a high signal
  digitalWrite(strobe, HIGH);
  digitalWrite(clock,HIGH);
}

////
// The program loop which polls the controller for new button data every 100ms
void loop() {
  controllerRead(); //get current controller state
  if(controller_data != last_data) { //if state has changed
    Serial.println(controller_data);  //send data over serial to the PC
  }
  last_data = controller_data; //save current state over previous state
  delay(100);
}

////
// The polling method which sets the controller_data byte to current button state
void controllerRead() {
  controller_data = 0;
  
  //start with strobe/clock on low
  digitalWrite(strobe, LOW);
  digitalWrite(clock, LOW);
  
  //do a 3ms pulse on the strobe wire to signal we want the button state
  digitalWrite(strobe, HIGH);
  delayMicroseconds(3);
  digitalWrite(strobe, LOW);
  
  //read the state of the data wire for the first button state
  controller_data = digitalRead(data);
  
  //loop for each of the remaining 7 buttons
  for(int i = 1; i <= 7; i++) {
    //do a pulse on the clock wire to signal we want the next button
    digitalWrite(clock,HIGH);
    delayMicroseconds(2); //2ms into pulse, read the data wire
    
    //shift current button data to the lef one bit
    controller_data = controller_data << 1;
    
    //push the current data wire state on right-most bit
    controller_data = controller_data + digitalRead(data); //note: low = button down, high = up

    //finish out the clock wire pulse
    delayMicroseconds(4); //4 more ms in pulse before going back to low
    digitalWrite(clock, LOW);
  } //end result is controller_data is now a bitmask showing state of each 8 buttons
}

/*
   Copyright 2014 Jason Wells
 
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   A copy of the License is provided as license.txt.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

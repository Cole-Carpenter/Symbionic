#define RED_LED 6
#define BLUE_LED 5
#define GREEN_LED 9

enum colorStates{ // 0, 1, 2
    COLOR_,
    SINGLE_FS,
    MULTI_FS
};
enum colorStates colorState;

int brightness = 255;

int gBright = 255;
int rBright = 255;
int bBright = 255;

int fadeSpeed = 10;

unsigned long last_time = 0;

bool Color = false;
bool SingleFlashingColor = false;

String Commands = "";
String ledCommands = "";
/*
 * Q - Turn Left -- Tech   ===== LEFT STATE
W - Turn Right -- Organic  ===== RIGHT STATE
Combos
Q + W HELD - walk
Q + W TAP - dash

      3. A - Crouch -- Tech     ==== CROUCH STATE
      4. A, then S - Jump -- Organic   === JUMP STATE
Combos
    Press jump immediately after releasing crouch - Perfect jump
    Glide - Hold Jump
    Dive-Bomb -- Crouch in the air

5. N -press and hold - Nibble -- Organic  ==== NIBBLE STATE
6. M press and hold - Interact- squeak -- Tech    ===== INTERACTSQUEAK STATE

7. X - Organic Squeak, Dig, Blood Transfusion    === ORGANIC STATE
8. Z - Radar, Magnet, Oil Change      ==== MECHANICAL STATE

COMBO
    Run forward, turn completely around, crouch, perfect jump, crouch, perfect jump -- backflip
 */
// variables will change:
unsigned long previousMillis = 0;

int LEDSTATE = 0;
void setup() {
  //START THE BAUD RATE
  Serial.begin(9600);
  
  // initialize the LED pin as an output:
  pinMode(LED_BUILTIN, OUTPUT);
  // initialize the pushbutton pin as an input:
 
  
   pinMode(GREEN_LED, OUTPUT);
   pinMode(RED_LED, OUTPUT);
   pinMode(BLUE_LED, OUTPUT);

}

void loop() {
  /*
   * 
   */
  ledCommands = Serial.read();
    switch(ledCommands[0]){
      default:
        rBright = 255;
        gBright = 255;
        bBright = 255;
        break;
      case 'R':
        rBright = 255;
        gBright = 0;
        bBright =0;
        break;
      case 'G':
        rBright = 0;
        gBright = 255;
        bBright = 0;
      case 'B':
        rBright = 0;
        gBright = 0;
        bBright = 255;
        break;        
    }
    if(ledCommands.length() == 2){
      switch(ledCommands[1]){
        case 'C':
          colorState = COLOR_;
        break;
        case 'T':
          colorState = MULTI_FS;
        break;
        case 'S':
          colorState = SINGLE_FS;
         
      }
    }
   //Normal Setting: TurnOn(rBright, gBright, bBright);
  //Crazy Setting
  if(colorState == 0){
    TurnOn(rBright, gBright, bBright);
  }else if (colorState == 1){
    OneLight(rBright, gBright, bBright, millis());
  }else if(colorState == 2){
    TripleLight(millis());
  }
  TripleLight(millis());
  //OneLight(rBright, gBright
  // check if the pushbutton is pressed. If it is, the buttonState is HIGH:
 /* if (buttonState == HIGH) {
    // turn LED on:
    digitalWrite(LED_BUILTIN, HIGH);
  } else {
    // turn LED off:
    digitalWrite(LED_BUILTIN, LOW);
  }*/
}
void OneLight(int R, int G, int B, unsigned int current){
  unsigned int currentMillis = current;
  if(currentMillis - previousMillis >= 250){
    TurnOn(R, G, B);
    previousMillis = currentMillis;
  }
}
void TripleLight(unsigned int current){
  unsigned int currentMillis = current;
  if(currentMillis - previousMillis >= 250){
    LEDSTATE++;
    if(LEDSTATE % 3 == 0){
      TurnOn(random(0,256),0, 0);
    }else if (LEDSTATE % 3 == 1){
      TurnOn(0, random(0,256), 0);
    }else{
      TurnOn(0, 0, random(0, 256));
    }
    previousMillis = currentMillis;
  }
}
void TurnOn( int R, int G, int B) { 
       analogWrite(RED_LED, R);
       analogWrite(GREEN_LED, G);
       analogWrite(BLUE_LED, B);

}
void TurnOff(int Color){
   for (int i = 0; i < 256; i++) {
       analogWrite(Color, brightness);
       brightness -= 1;
       //delay(fadeSpeed);
   }
}
void TurnOffALL() {
   for (int i = 0; i < 256; i++) {
       analogWrite(GREEN_LED, brightness);
       analogWrite(RED_LED, brightness);
       analogWrite(BLUE_LED, brightness);
 
       brightness -= 1;
       delay(fadeSpeed);
   }
}

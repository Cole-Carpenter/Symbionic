#define RED_LED 6
#define BLUE_LED 5
#define GREEN_LED 9

int brightness = 255;

int gBright = 255;
int rBright = 255;
int bBright = 255;

int fadeSpeed = 10;

unsigned long last_time = 0;
// constants won't change. They're used here to set pin numbers:
const int buttonPin = 8;     // the number of the pushbutton pin
const int buttonPin2 = 7;
String Commands = "";
// variables will change:
int buttonState = 0;         // variable for reading the pushbutton status
int buttonState2 = 0;
void setup() {
  //START THE BAUD RATE
  Serial.begin(9600);
  
  // initialize the LED pin as an output:
  pinMode(LED_BUILTIN, OUTPUT);
  // initialize the pushbutton pin as an input:
  pinMode(buttonPin, INPUT);
  pinMode(buttonPin2, INPUT);

  pinMode(GREEN_LED, OUTPUT);
   pinMode(RED_LED, OUTPUT);
   pinMode(BLUE_LED, OUTPUT);

}

void loop() {
      Commands = "";
  // read the state of the pushbutton value:
  buttonState = digitalRead(buttonPin);
  buttonState2 = digitalRead(buttonPin2);
  if (buttonState == LOW){
    Commands += "L,";
  }
  else{
    Commands += "W,";
  }
  if(buttonState2 == LOW){
    Commands += "R";
  }
  else{
    Commands += "W";
  }
    Serial.println(Commands);

    switch(Serial.read()){
      case 'H':
        rBright = 255;
        gBright = 0;
        bBright =0;
        break;
      case 'B':
        rBright = 128;
        gBright = 128;
        bBright = 128;
        break;
      case 'A':
        rBright = 255;
        gBright = 255;
        bBright = 255;
        break;        
    }
    TurnOn(rBright, gBright, bBright);
  //Crazy Setting
  //TurnOn(random(0,256),random(0,256),random(0,256));
  // check if the pushbutton is pressed. If it is, the buttonState is HIGH:
 /* if (buttonState == HIGH) {
    // turn LED on:
    digitalWrite(LED_BUILTIN, HIGH);
  } else {
    // turn LED off:
    digitalWrite(LED_BUILTIN, LOW);
  }*/
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

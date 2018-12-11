using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class MyMessageListener : MonoBehaviour {

	//ARDUINO CONNECTION STUFF
    private bool Connected;

    public GameObject L;
    public GameObject R;
	public GameObject T;
	public Text CodeT;

	//USB CONNECTION STUFF
	private List<String> USBS;

	private String TextFileName = "TestingUSB.txt";
	private String FileCommands;

	public GameObject Heal;
	public GameObject Fix;

	[HideInInspector]
	public bool Q;
	[HideInInspector]
	public bool W;
    /*
		The Driver letter assignment in windows works as such:

		When you connect a usb it will automatically connect it to D:\ and E:\ with every subsequent one
		being the next letter.

		For this project we will be dealing with three usbs therefore:
		D:\, E:\, and F:\
	
	 */

    /*
       We could make it so that the order of connecting USBs matter, which would override the port issue


    */

    public bool MOVEMENT = false;
    private bool MovementCooldown = false;
    public float coolDownTime = 3f;
    private float coolDownTimer = 0f;
    private float buttonDouble = 0f;
    private string COMMANDS = "WW";
	// Use this for initialization
	void Start () {

		//ARDUINO CONNECTION
        Connected = false;	

		//USB CONNECTION
		USBS = new List<String>();
		USBS.Add("D:\\");
		USBS.Add("E:\\");
		USBS.Add("F:\\");
        USBS.Add("G:\\");

        string[] allDrives = Directory.GetLogicalDrives();
        foreach (string  d in allDrives)
        {
            	//Debug.Log(d);
        }

		/*string path = @"c:\Users\carlsm4\Test.txt";
		// Open the file to read from.
        using (StreamReader sr = File.OpenText(path))
        {
            string s;
            while ((s = sr.ReadLine()) != null)
            {
				if("Testing to see if we can open this in unity;" == s){
					GetComponent<Renderer>().material.color = Color.yellow;
				}
                text_.text = s;
            }
        }*/
	}
	
	// Update is called once per frame
	void Update () {
		//USB CONNECTION STUFF
		string[] allDrives = Directory.GetLogicalDrives();
		string Drives = "";
        foreach (string  d in allDrives)
        {
            Drives += d;
        }
		foreach(string usb in USBS){
			if(Drives.Contains(usb)){
				//This is to test out to see if we can access the specific txt file
				string path = usb+"\\"+TextFileName;
				try{
					using (StreamReader sr = File.OpenText(path))
        			{
            			string s;
						while ((s = sr.ReadLine()) != null)
						{
							FileCommands += s + " ";
						}
					}
				}catch (Exception e){
					//Debug.Log("Nice try loser");
					//Debug.Log(e.Message);
				}
			}
		}
		PowerUp(FileCommands);
		FileCommands = "";

	}
	/*
		USB SECTION, ALL COMMANDS HERE REGARDED TO THE USB ONLY

	*/
  void PowerUp(String FileCommand){
			if(FileCommand != null){
				if(FileCommand.Contains("Command1")){
					float x = Input.GetAxis("Horizontal");
					Rigidbody rb = Heal.GetComponent<Rigidbody>();
					rb.velocity = new Vector3(x * 10.0f, rb.velocity.y, rb.velocity.z);
					Heal.GetComponent<Renderer>().material.color = Color.red;
                    GetComponent<SerialController>().SendSerialMessage("H");
                    return;
				}else{
					Heal.GetComponent<Renderer>().material.color = Color.white;
				}
				if(FileCommand.Contains("Command2")){
					float x = Input.GetAxis("Vertical");
					Rigidbody rb = Heal.GetComponent<Rigidbody>();
					rb.velocity = new Vector3(rb.velocity.x, x * 10.0f, rb.velocity.z);
					Fix.GetComponent<Renderer>().material.color = Color.black;
                    GetComponent<SerialController>().SendSerialMessage("B");
                    return;
                }
                else{
					Fix.GetComponent<Renderer>().material.color = Color.white;
				}
                GetComponent<SerialController>().SendSerialMessage("A");
        }
	}

  
  /*

	ARDUINO SECTION, ALL COMMANDS THAT HAVE TO DO WITH THE ARDUINO

  */
  
  //called when Arduion sends a line of data

    void OnMessageArrived(string m)
    {
        TestCommands(m);
        Debug.Log("Arrived: " + m);

    }
    void OnConnectionEvent(bool s)
    {
        Connected = s;
        Debug.Log(s ? "Device Connected" : "Device Disconnected");
    }

    public bool GetConnection()
    {
        return Connected;
    }

    bool ContainsCommand(string c, char Command){
        foreach(char x in c){
            if(x == Command){
                return true;
            }
        }
        return false;
    }


    void TestCommands(string CList)
    {
        string[] SS = CList.Split(new string[] { ","}, StringSplitOptions.None);
        String commands = "";
        foreach(String x in SS){
            commands += x;
        }
        if (commands.Contains("L"))
        {
            L.GetComponent<Renderer>().material.color = Color.yellow;
			Q = true;
        }
        else
        {
            L.GetComponent<Renderer>().material.color = Color.white;
			Q = false;
        }
        if(commands.Contains("R"))
        {
			Heal.transform.Translate(Vector3.up * 3 * Time.deltaTime, Space.World);
            R.GetComponent<Renderer>().material.color = Color.yellow;
			W = true;
        }
        else
        {
            R.GetComponent<Renderer>().material.color = Color.white;
			W = false;
        }
		
		//NUMPAD STUFF you have to use Keycode.Keypad# example Keycode.Keypad7 
		//or you can use Input.GetKeyDown("[7]") specifically for the keypad
		

		//I HAVE NO IDEA HOW TO MAKE THIS BETTER SO BARE WITH ME
		if(Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Insert)){
			CodeT.text += "0";
		}
		if(Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.End)){
			CodeT.text += "1";
		}
		if(Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.DownArrow)){
			CodeT.text += "2";
		}
		if(Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.PageDown)){
			CodeT.text += "3";
		}
		if(Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.LeftArrow)){
			CodeT.text += "4";
		}
		//Keypad 5 is weird, doesn't have an alternative unless it's blank
		if(Input.GetKeyDown(KeyCode.Keypad5)){
			CodeT.text += "5";
		}
		if(Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.RightArrow)){
			CodeT.text += "6";
		}
		if(Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Home)){
			CodeT.text += "7";
		}
		if(Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.UpArrow)){
			CodeT.text += "8";
		}
		if(Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.PageUp)){
			CodeT.text += "9";
		}
		if(CodeT.text.Length == 7){
			if(CodeT.text == "1234567"){
				T.GetComponent<Renderer>().material.color = Color.yellow;
			}else if(CodeT.text == "7654321"){
				T.GetComponent<Renderer>().material.color = Color.white;
			}
			CodeT.text = "";
		}
        //Checking Command schemes
       /* CheckDoubleTap(Time.time, commands);
        if (MOVEMENT)
        {
            L.GetComponent<Renderer>().material.color = Color.yellow;
        }
        else
        {
            L.GetComponent<Renderer>().material.color = Color.white;
        }*/
    }

    private void CheckDoubleTap(float time, string c)
    {   
        if(buttonDouble == 0)
        {
            buttonDouble = time;
        }

        if(time - coolDownTimer >= coolDownTime)
            if(time - buttonDouble >= coolDownTime)
            {
                Debug.Log("SHOULD SHOW UP EVERY SECOND");
                if ((c[0] != COMMANDS[0] /*&& c[1] != COMMANDS[1]*/))
                {
                    MOVEMENT = true;
                    coolDownTimer = Time.time;
                }
                else
                    MOVEMENT = false;

                COMMANDS = c;
            }
    }
}

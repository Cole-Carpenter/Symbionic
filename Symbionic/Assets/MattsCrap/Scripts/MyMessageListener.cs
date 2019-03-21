using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class MyMessageListener : MonoBehaviour {

	//ARDUINO CONNECTION STUFF
    private bool Connected;

	public Text CodeT;

	//USB CONNECTION STUFF
	private List<String> USBS;

	private String TextFileName = "USB.txt";
	private String FileCommands;

	public GameObject PlayerController;
	[HideInInspector]
	public bool Q = false;
	[HideInInspector]
	public bool W = false;

	private float prevTime = 0f;

	public AudioClip[] Audio;
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
		KeypadControls();
		FileCommands = "";

	}
	/*
		USB SECTION, ALL COMMANDS HERE REGARDED TO THE USB ONLY

	*/
  void PowerUp(String FileCommand){
			if(FileCommand != null){
                if (FileCommand.Contains("Dig") && FileCommand.Contains("Magnet")){
                    PlayerController.GetComponent<PlayerController>().usbState = States.canRadar;
                }
				else if(FileCommand.Contains("Dig")){
					PlayerController.GetComponent<PlayerController>().usbState = States.canDig;
				}
				else if(FileCommand.Contains("Magnet")){
					PlayerController.GetComponent<PlayerController>().usbState = States.canMagnet;
                }
        }
	}

  
  /*

	ARDUINO SECTION, ALL COMMANDS THAT HAVE TO DO WITH THE ARDUINO

  */
  
  //called when Arduion sends a line of data

    void OnMessageArrived(string m)
    {

       // Debug.Log("Arrived: " + m);

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



    void KeypadControls()
    {
		
		//NUMPAD STUFF you have to use Keycode.Keypad# example Keycode.Keypad7 
		//or you can use Input.GetKeyDown("[7]") specifically for the keypad
		
		float currentTime = Time.time;
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
		AudioSource audioS = PlayerController.GetComponent<AudioSource>();
		if(CodeT.text == "5889"){
			audioS.clip = Audio[0];

		}else if(CodeT.text == "4816"){
			audioS.clip = Audio[1];
			audioS.Play();
		}else if (CodeT.text == "1829"){
			audioS.clip = Audio[2];
			audioS.Play();
		}else if(CodeT.text == "0318"){
			audioS.clip = Audio[3];
			audioS.Play();
		}else if(CodeT.text == "8840"){
			audioS.clip = Audio[4];
			audioS.Play();
		}else if(CodeT.text == "8446"){
			audioS.clip = Audio[5];
			audioS.Play();
		}else if(CodeT.text == "0888"){
			audioS.clip = Audio[6];
			audioS.Play();
		}else if(CodeT.text == "7680"){
			audioS.clip = Audio[7];
			audioS.Play();
		}else if(CodeT.text == "5898"){
			audioS.clip = Audio[8];
			audioS.Play();
		}else if(CodeT.text == "7783"){
			audioS.clip = Audio[9];
			audioS.Play();

		}

		else if(CodeT.text == "6444"){
			Debug.Log("Ability to Glide");
			PlayerController.GetComponent<PlayerController>().canGlide = true;
		}else if(CodeT.text == "7285"){
			Debug.Log("Glide updraft (Second Jump)");
			PlayerController.GetComponent<PlayerController>().canUpdraft = true;
		}else if(CodeT.text == "6432"){
			Debug.Log("Jump Higher");
			//PlayerController.GetComponent<PlayerController>().
		}else if(CodeT.text == "4512"){
			Debug.Log("Dive Bomb");
			PlayerController.GetComponent<PlayerController>().canDiveBomb = true;
		}
		if(CodeT.text.Length > 4){
			
			CodeT.text = "";
		}
    }

}

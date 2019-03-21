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

	[HideInInspector]
	public bool Q = false;
	[HideInInspector]
	public bool W = false;

	private float prevTime = 0f;

	public AudioClip[] Audio;

    private AudioSource[] audioS;
    private GameObject Camera;
    private PlayerController _controller;
    private float currentTime;

    private bool fadingIn;
    private bool fadingOut;

    private bool IsPlayingClip;
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
        Camera = GameObject.Find("FreeLookCameraRig");
        audioS = Camera.GetComponents<AudioSource>();
        PlayOriginalTrack();
        _controller = GetComponent<PlayerController>();

        IsPlayingClip = false;
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
                    _controller.usbState = States.canRadar;
                }
				else if(FileCommand.Contains("Dig")){
					_controller.usbState = States.canDig;
				}
				else if(FileCommand.Contains("Magnet")){
					_controller.usbState = States.canMagnet;
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
		


		
		//I HAVE NO IDEA HOW TO MAKE THIS BETTER SO BARE WITH ME
		if(Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Insert)){
			CodeT.text += "0";
            currentTime = Time.time;
        }
		if(Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.End)){
			CodeT.text += "1";
            currentTime = Time.time;
        }
		if(Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.DownArrow)){
			CodeT.text += "2";
            currentTime = Time.time;
        }
		if(Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.PageDown)){
			CodeT.text += "3";
            currentTime = Time.time;
        }
		if(Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.LeftArrow)){
			CodeT.text += "4";
            currentTime = Time.time;
        }
		//Keypad 5 is weird, doesn't have an alternative unless it's blank
		if(Input.GetKeyDown(KeyCode.Keypad5)){
			CodeT.text += "5";
            currentTime = Time.time;
        }
		if(Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.RightArrow)){
			CodeT.text += "6";
            currentTime = Time.time;
        }
		if(Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Home)){
			CodeT.text += "7";
            currentTime = Time.time;
        }
		if(Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.UpArrow)){
			CodeT.text += "8";
            currentTime = Time.time;
        }
		if(Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.PageUp)){
			CodeT.text += "9";
            currentTime = Time.time;
        }
        if (!IsPlayingClip)
        {
            if (CodeT.text == "5889")
            {
                SetTracks(0.1f, 1, 0, 1);
                Invoke("PlayOriginalTrack", audioS[1].clip.length - 2f);
            }
            else if (CodeT.text == "4816")
            {
                SetTracks(0.1f, 2, 0, 1);
                Invoke("PlayOriginalTrack", audioS[1].clip.length - 2f);
            }
            else if (CodeT.text == "1829")
            {
                SetTracks(0.1f, 3, 0, 1);
                Invoke("PlayOriginalTrack", audioS[1].clip.length - 2f);
            }
            else if (CodeT.text == "0318")
            {
                SetTracks(0.1f, 4, 0, 1);
                Invoke("PlayOriginalTrack", audioS[1].clip.length - 2f);
            }
            else if (CodeT.text == "8840")
            {
                SetTracks(0.1f, 5, 0, 1);
                Invoke("PlayOriginalTrack", audioS[1].clip.length - 2f);
            }
            else if (CodeT.text == "8446")
            {
                SetTracks(0.1f, 6, 0, 1);
                Invoke("PlayOriginalTrack", audioS[1].clip.length - 2f);
            }
            else if (CodeT.text == "0888")
            {
                SetTracks(0.1f, 7, 0, 1);
                Invoke("PlayOriginalTrack", audioS[1].clip.length - 2f);
            }
            else if (CodeT.text == "7680")
            {
                SetTracks(0.1f, 8, 0, 1);
                Invoke("PlayOriginalTrack", audioS[1].clip.length - 2f);
            }
            else if (CodeT.text == "5898")
            {
                SetTracks(0.1f, 9, 0, 1);
                Invoke("PlayOriginalTrack", audioS[1].clip.length - 2f);
            }
            else if (CodeT.text == "7783")
            {
                SetTracks(0.1f, 10, 0, 1);
                Invoke("PlayOriginalTrack", audioS[1].clip.length - 2f);
            }
        }
        else if (CodeT.text == "6444")
        {
            Debug.Log("Ability to Glide");
            _controller.canGlide = true;
        }
        else if (CodeT.text == "7285")
        {
            Debug.Log("Glide updraft (Second Jump)");
            _controller.canUpdraft = true;
        }
        else if (CodeT.text == "6432")
        {
            Debug.Log("Jump Higher");
        }
        else if (CodeT.text == "4512")
        {
            Debug.Log("Dive Bomb");
            _controller.canDiveBomb = true;
        }
		if(CodeT.text.Length >= 4){
			
			CodeT.text = "";
		}
        if(Time.time - currentTime > 2.0f)
        {
            CodeT.text = "";
        }
    }

    private void PlayOriginalTrack()
    {
        IsPlayingClip = false;
        audioS[0].clip = Audio[0];
        StartCoroutine(FadeOut(0.1f, 1));
        StartCoroutine(FadeIn(0.1f, 0));
    }
    private void SetTracks(float speed, int clip, int a1, int a2)
    {
        IsPlayingClip = true;
        audioS[a2].clip = Audio[clip];

        StartCoroutine(FadeOut(speed, a1));
        StartCoroutine(FadeIn(speed, a2));
    }

    /*IEnumerator FadeIn(int track, float speed, float volume)
    {
        fadingIn = true;
        fadingOut = false;

        float aVolume = audioS.volume;

        
    }*/

        //a1 fades out 
        //a2 fades in
    IEnumerator FadeOut(float speed, int a1)
    {
        float aVolume = audioS[a1].volume;
        while (audioS[a1].volume >= speed)
        {
            aVolume -= speed;
            audioS[a1].volume = aVolume;
            yield return new WaitForSeconds(0.5f);
        }
        audioS[a1].Stop();
    }
    IEnumerator FadeIn(float speed, int a2)
    {
        audioS[a2].Play();
        float aVolume = audioS[a2].volume;
        while (audioS[a2].volume != 1.0f)
        {
            aVolume += speed;
            audioS[a2].volume = aVolume;
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator FadeOutFadeIn(float speed, int clip, int a1, int a2)
    {

        float aVolume = audioS[a1].volume;
        while(audioS[a1].volume >= speed)
        {
            aVolume -= speed;
            audioS[a1].volume = aVolume;
            yield return new WaitForSeconds(0.5f);
        }
        audioS[a1].Stop();

       audioS[a2].clip = Audio[clip];
        audioS[a2].Play();

        aVolume = audioS[a2].volume;
        while (audioS[a2].volume != 1.0f)
        {
            aVolume += speed;
            audioS[a2].volume = aVolume;
            yield return new WaitForSeconds(0.5f);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour {

    public enum PauseStates {Codes, Quit};

    public GameObject pauseMenuUI;
    public GameObject settingsOb;
    public GameObject codeList;
    public Button codeButton;
    public Button settingsButton;
    public Button quitButton;
    public Slider fx;
    public Slider music;
    public AudioMixer am;

    private List<string> codes;
    private GameObject currSubMenu;
    private PlayerController playerController;

    private bool paused = false;
    private bool subMenu = false;



    // Use this for initialization
    void Start () {
        //codeButton = pauseMenuUI.transform.GetChild(1).GetComponent<Button>();
        //quitButton = pauseMenuUI.transform.GetChild(2).GetComponent<Button>();
        playerController = GetComponent<PlayerController>();
        codes = new List<string>();

        pauseMenuUI = GameObject.Find("PauseMenu");
        settingsOb = GameObject.Find("SettingsList");
        codeList = GameObject.Find("CodeList");

        codeButton = GameObject.Find("Codes").GetComponent<Button>();
        settingsButton = GameObject.Find("Settings").GetComponent<Button>();
        quitButton = GameObject.Find("Quit").GetComponent<Button>();
        

    }
	
	// Update is called once per frame
	void Update () {
        //get out of submenu
        if(Input.GetButtonDown("Pause") && subMenu){
            currSubMenu.SetActive(false);
            FlipButtons();
            subMenu = false;
            codeButton.Select();
        }
        //pause
        else if (Input.GetButtonDown("Pause"))
        {
            Pause(paused);
        }
    }

    private void Pause(bool state)
    {
        if (state)
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            paused = false;
            playerController.enabled = true;
        }
        else
        {
            playerController.enabled = false;
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            paused = true;
        }
    }

    private void FlipButtons()
    {
        codeButton.gameObject.SetActive(!codeButton.gameObject.activeSelf);
        settingsButton.gameObject.SetActive(!settingsButton.gameObject.activeSelf);
        quitButton.gameObject.SetActive(!quitButton.gameObject.activeSelf);
    }

    public void Codes()
    {
        FlipButtons();
        codeList.SetActive(true);
        Text t = codeList.GetComponentInChildren<Text>();
        t.text = "";
        for(int i = 0; i < codes.Count; i += 2)
        {
            string newl = i > 0 ? "\n" : ""; 
            t.text += newl + codes[i] + ": " + codes[i + 1];
        }
        currSubMenu = codeList;
        subMenu = true;
    }

    public void Settings()
    {
        FlipButtons();
        settingsOb.SetActive(true);
        settingsOb.GetComponentInChildren<Slider>().Select();
        currSubMenu = settingsOb;
        subMenu = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void AddCode(string code, string description)
    {
        foreach(string phrase in codes)
        {
            if (code == phrase.Trim(' '))
            {
                return;
            }
        }
        codes.Add(code);
        codes.Add(description);
    }

    public void FxChange()
    {
        am.SetFloat("FX", Mathf.Log(fx.value) * 20);
    }

    public void MusicChange()
    {
        am.SetFloat("Music", Mathf.Log(music.value) * 20);
    }
}

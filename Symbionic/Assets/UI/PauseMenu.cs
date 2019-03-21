using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    public enum PauseStates {Codes, Quit};

    
    public GameObject pauseMenuUI;
    public GameObject settingsOb;
    public GameObject codeList;
    public Button codeButton;
    public Button settingsButton;
    public Button quitButton;

    private GameObject currSubMenu;
    private PlayerController playerController;

    private PauseStates pauseSelect = 0;
    private bool paused = false;
    private bool subMenu = false;



    // Use this for initialization
    void Start () {
        //codeButton = pauseMenuUI.transform.GetChild(1).GetComponent<Button>();
        //quitButton = pauseMenuUI.transform.GetChild(2).GetComponent<Button>();
        playerController = GetComponent<PlayerController>();
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
            if (paused)
            {
                pauseSelect = 0;
            }
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    public enum PauseStates {Codes, Quit};

    private PauseStates pauseSelect = 0;
    private bool paused = false;
    public GameObject pauseMenuUI;
    private PlayerController playerController;
    public Button codeButton;
    public Button quitButton;
    public Image codeList;

    // Use this for initialization
    void Start () {
        //codeButton = pauseMenuUI.transform.GetChild(1).GetComponent<Button>();
        //quitButton = pauseMenuUI.transform.GetChild(2).GetComponent<Button>();
        playerController = GetComponent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {
        //pause
        if (Input.GetButtonDown("Pause"))
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

    public void Codes()
    {
        codeButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        codeList.enabled = true;
    }

    public void Quit()
    {
        Application.Quit();
    }
}

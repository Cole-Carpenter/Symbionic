using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    public enum PauseStates {Codes, Quit};

    private PauseStates pauseSelect = 0;
    private bool paused = false;
    public GameObject pauseMenuUI;
    public Button codeButton;
    public Button quitButton;

    // Use this for initialization
    void Start () {
        codeButton = pauseMenuUI.transform.GetChild(1).GetComponent<Button>();
        quitButton = pauseMenuUI.transform.GetChild(2).GetComponent<Button>();
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

        if(paused && Input.GetButtonDown("Vertical_key"))
        {
            pauseSelect = (PauseStates)(((int)pauseSelect + 1) % 2);
        }
        
        if (paused && paused && pauseSelect == PauseStates.Codes)
        {
            codeButton.Select();
            codeButton.OnSelect(null);
        }

        if (paused && paused && pauseSelect == PauseStates.Quit)
        {
            quitButton.Select();
        }
    }

    private void Pause(bool state)
    {
        if (state)
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            paused = false;
        }
        else
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            paused = true;
        }
    }

    public void Quit()
    {
        Application.Quit();
        print("quitting");
    }
}

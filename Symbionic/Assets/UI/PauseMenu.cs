using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    private bool paused = false;
    public GameObject pauseMenuUI;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //pause
        if (Input.GetButtonDown("Pause"))
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
        }
        else
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            paused = true;
        }
    }
}

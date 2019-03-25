﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	
	
    

    private float fadeout = 0;

    //Canvas Scene objects
    private UIScene uis = SymApp.instance.scene.ui;
    private PauseScene ps = SymApp.instance.scene.p;
    private SymStatus status = SymApp.instance.status;

    // Update is called once per frame
    void Update () {
        if(fadeout > 0)
        {
            fadeout -= Time.deltaTime;
        }
        else
        {
            if(uis.IsTransparent())
            {
                uis.SetTransparency(uis.GetTransparency() - .05f);
            }
        }

        //Pause Menu stuff
        //get out of submenu
        if (Input.GetButtonDown("Pause") && status.subMenu)
        {
            status.currSubMenu.SetActive(false);
            ps.FlipButtons();
            status.subMenu = false;
            ps.codeButton.Select();
        }
        //pause
        else if (Input.GetButtonDown("Pause"))
        {
            Pause(status.paused);
        }
    }

	public void SendCode(string code, string description){
        uis.SetText(code, description);
        fadeout = 20f;
        ps.AddCode(code, description);
    }

    public void Pause(bool state)
    {
        if (state)
        {
            ps.ToggleUI(false);
            Time.timeScale = 1f;
            status.paused = false;
            SymApp.instance.manager.playerManager.enabled = true;
        }
        else
        {
            SymApp.instance.manager.playerManager.enabled = false;
            ps.ToggleUI(true);
            Time.timeScale = 0f;
            status.paused = true;
        }
    }

    public void Code()
    {
        ps.CodeCanvas();
        status.currSubMenu = ps.codeList;
        status.subMenu = true;
    }

    public void Settings()
    {
        ps.SettingsCanvas();
        status.currSubMenu = ps.settingsOb;
        status.subMenu = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void AddCode(string code, string description)
    {
        foreach (string phrase in codes)
        {
            if (code == phrase.Trim(' '))
            {
                return;
            }
        }
        codes.Add(code);
        codes.Add(description);
    }
}

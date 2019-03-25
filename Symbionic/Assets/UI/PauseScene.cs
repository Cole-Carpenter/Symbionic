using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseScene : MonoBehaviour {

    public enum PauseStates {Codes, Quit};

    public GameObject pauseMenuUI;
    public GameObject settingsOb;
    public GameObject codeList;
    public Button codeButton;
    public Button settingsButton;
    public Button quitButton;
    public Slider fx;
    public Slider music;

    public SymStatus status = SymApp.instance.status;

    public void FlipButtons()
    {
        codeButton.gameObject.SetActive(!codeButton.gameObject.activeSelf);
        settingsButton.gameObject.SetActive(!settingsButton.gameObject.activeSelf);
        quitButton.gameObject.SetActive(!quitButton.gameObject.activeSelf);
    }

    public void CodeCanvas()
    {
        FlipButtons();
        codeList.SetActive(true);
        Text t = codeList.GetComponentInChildren<Text>();
        t.text = "";
        for (int i = 0; i < status.GetCodeLength(); i += 2)
        {
            string newl = i > 0 ? "\n" : "";
            t.text += newl + status.GetCodeAtIndex(i) + ": " + status.GetCodeAtIndex(i+1);
        }
    }

    public void SettingsCanvas()
    {
        FlipButtons();
        settingsOb.SetActive(true);
        settingsOb.GetComponentInChildren<Slider>().Select();
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

    public void ToggleUI(bool toggle)
    {
        pauseMenuUI.SetActive(toggle);
    }
}

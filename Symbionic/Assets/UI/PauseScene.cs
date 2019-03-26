using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseScene : MonoBehaviour {

    public enum PauseStates {Codes, Quit};

    private GameObject settingsOb;
    private GameObject codeList;
    private Button codeButton;
    private Button settingsButton;
    private Button quitButton;
    private Slider fx;
    private Slider music;

    public SymStatus status;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.001f);
        status = SymApp.Instance.status;
        codeButton = transform.Find("Codes").GetComponent<Button>();
        settingsButton = transform.Find("Settings").GetComponent<Button>();
        quitButton = transform.Find("Quit").GetComponent<Button>();
        codeList = transform.Find("CodeList").gameObject;
        settingsOb = transform.Find("SettingsList").gameObject;
        fx = settingsOb.transform.Find("FX Slider").GetComponent<Slider>();
        music = settingsOb.transform.Find("Music Slider").GetComponent<Slider>();
        codeButton.onClick.AddListener(SymApp.Instance.manager.ui.Code);
        settingsButton.onClick.AddListener(SymApp.Instance.manager.ui.Settings);
        quitButton.onClick.AddListener(SymApp.Instance.manager.ui.Quit);
    }

    public void Update()
    {
        if (status.paused)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (status.currSubMenu == settingsOb && status.subMenu)
                    music.Select();
                else
                    codeButton.Select();
            }
        }
    }

    public void FlipButtons()
    {
        codeButton.gameObject.SetActive(!codeButton.gameObject.activeSelf);
        settingsButton.gameObject.SetActive(!settingsButton.gameObject.activeSelf);
        quitButton.gameObject.SetActive(!quitButton.gameObject.activeSelf);
    }

    public void CodeCanvas()
    {
        status.currSubMenu = codeList;
        EventSystem.current.SetSelectedGameObject(null);
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
        status.currSubMenu = settingsOb;
        FlipButtons();
        settingsOb.SetActive(true);
        settingsOb.GetComponentInChildren<Slider>().Select();
    }

    public void ToggleUI(bool toggle)
    {
        GetComponent<Image>().enabled = toggle;
        transform.Find("Paused").gameObject.SetActive(toggle);
        codeButton.gameObject.SetActive(toggle);
        settingsButton.gameObject.SetActive(toggle);
        quitButton.gameObject.SetActive(toggle);
        transform.Find("EventSystem").gameObject.SetActive(toggle);
    }

    public IEnumerator OnPauseStart()
    {
        yield return 0;
        codeButton.Select();
    }

    public float GetSliderValue(string mixer)
    {
        if(mixer == "fx")
        {
            return fx.value;
        }
        else if(mixer == "music")
        {
            return music.value;
        }
        return 0;
    }
}

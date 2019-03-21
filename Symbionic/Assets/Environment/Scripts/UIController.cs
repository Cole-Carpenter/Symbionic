﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
	
	public Text codeboard;
    public Text descboard;
    private PauseMenu p;

    private float fadeout = 0;

	// Use this for initialization
	void Start () {
        p = GetComponent<PauseMenu>();
        codeboard = GameObject.Find("Code").GetComponent<Text>();
        descboard = GameObject.Find("Description").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if(fadeout > 0)
        {
            fadeout -= Time.deltaTime;
        }
        else
        {
            if(codeboard.color.a != 0)
            {
                float newA = codeboard.color.a - .05f;
                codeboard.color = new Color(255, 255, 255, newA);
                descboard.color = new Color(255, 255, 255, newA);
            }
        }
	}

	public void SendCode(string code, string description){
		codeboard.text = code;
        descboard.text = description;
        fadeout = 20f;
        codeboard.color = Color.white;
        descboard.color = Color.white;
        p.AddCode(code,description);
	}
}

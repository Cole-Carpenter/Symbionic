using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScene : MonoBehaviour
{
    private Text codeboard;
    private Text descboard;

    // Start is called before the first frame update
    void Start()
    {
        codeboard = GameObject.Find("Code").GetComponent<Text>();
        descboard = GameObject.Find("Description").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsTransparent()
    {
        return codeboard.color.a != 0;
    }

    public float GetTransparency()
    {
        return codeboard.color.a;
    }

    public void SetTransparency(float newA)
    {
        codeboard.color = new Color(255, 255, 255, newA);
        descboard.color = new Color(255, 255, 255, newA);
    }

    public void SetText(string code, string description)
    {
        codeboard.text = code;
        descboard.text = description;
        codeboard.color = Color.white;
        descboard.color = Color.white;
    }
}

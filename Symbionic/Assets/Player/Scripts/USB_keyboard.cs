using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USB_keyboard : MonoBehaviour {

    private PlayerController controller;
    
    
    private int count = 0;

	// Use this for initialization
	void Start () {
        controller = gameObject.GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            count++;
            count = count % 3;
        }
        controller.usbState = (States)count;
	}
}

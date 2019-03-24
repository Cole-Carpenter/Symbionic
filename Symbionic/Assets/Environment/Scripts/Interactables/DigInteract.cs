using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigInteract : Interactable {

	public string code = "";

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override string Interact(Transform players)
    {
		gameObject.GetComponent<Renderer>().material.color = Color.black;
		return code;
	}
}

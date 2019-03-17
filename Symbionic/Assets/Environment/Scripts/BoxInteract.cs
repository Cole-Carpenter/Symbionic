using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxInteract : Interactable {

	public string code;
	private Animator aic;

	// Use this for initialization
	void Start () {
		aic = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override string Interact(Transform player)
    {
		aic.SetTrigger("open");
		return code;
	}
}

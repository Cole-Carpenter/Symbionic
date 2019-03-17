using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundStoneInteract : Interactable {

    private AudioSource auds;

	// Use this for initialization
	void Start () {
        auds = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override string Interact(Transform Player)
    {
        auds.Play(100000);
        return "";
    }
}

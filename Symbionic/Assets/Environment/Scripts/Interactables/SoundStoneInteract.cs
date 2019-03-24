using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundStoneInteract : Interactable {

    private AudioSource auds;
    private Animator ac;

	// Use this for initialization
	void Start () {
        auds = GetComponent<AudioSource>();
        ac = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override string Interact(Transform Player)
    {
        auds.PlayDelayed(2f);
        ac.SetTrigger("Activate");
        return "";
    }
}

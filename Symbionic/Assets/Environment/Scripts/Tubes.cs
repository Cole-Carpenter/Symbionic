using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tubes : Interactable {

    public List<Transform> Nodes;
    public Transform player;
    private Transform nextNode;
    private bool activated = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (activated)
        {
            player.position = Vector3.Lerp(player.position, nextNode.position, 1f);
        }
	}

    public override string Interact()
    {
        return base.Interact();
    }
}

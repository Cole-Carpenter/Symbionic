using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tubes : Interactable {

    public List<Transform> Nodes;
    public Transform playerT;
    private Transform nextNode;
    private bool activated = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (activated)
        {
            playerT.position = Vector3.Lerp(playerT.position, nextNode.position, 1f);
        }

        print("");
	}

    public override string Interact(Transform player)
    {
        activated = true;
        playerT = player;
        return"";
    }
}

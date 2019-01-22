using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour {
	
	private List<GameObject> boxes;

	private float timer = 0;
	private float queue = 0;

    private AudioSource aso;

	// Use this for initialization
	void Start () {
		boxes = new List<GameObject>();
        aso = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;

		if(timer < 0){
			timer = 3f;
			if(queue > 0){
                aso.Play();
				queue--;
			}
		}
	}

	private void OnTriggerEnter(Collider other){
		if(other.tag == "box"){
			boxes.Add(other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other){
		if(other.tag == "box"){
			boxes.Remove(other.gameObject);
		}
	}

	public void Ping(){
		foreach(GameObject box in boxes){
			queue++;
		}
	}
}

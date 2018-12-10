using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float rotateSpeed = 1f;
    public float walkSpeed = 1f;
    public float runSpeed = 1f;
    private bool runActive = false;
    private bool running = false;
    private float runStart = 0f;
    private float leftStart = 0f;
    private float rightStart = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        runStart -= Time.deltaTime;
        leftStart -= Time.deltaTime;
        rightStart -= Time.deltaTime;
        print(running);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            leftStart = .1f;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            rightStart = .1f;
        }

        if (runActive && rightStart > 0 && leftStart > 0)
        {
            running = true;
        }

        else if (rightStart > 0 && leftStart > 0)
        {
            runActive = true;
            runStart = 2.5f;
        }

        else if (runStart <= 0)
        {
            runActive = false;
            running = false;
        }

        if (running)
        {
            transform.position += transform.forward * runSpeed * Time.deltaTime;
        }

        if (!running && Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * walkSpeed * Time.deltaTime;
        }

        else if (!running && Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.down * rotateSpeed * Time.deltaTime);
        }

        else if (!running && Input.GetKey(KeyCode.W))
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }

        
    }
}

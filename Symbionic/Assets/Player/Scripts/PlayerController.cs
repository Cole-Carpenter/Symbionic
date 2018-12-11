using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	//speeds
    public float rotateSpeed = 1f;
    public float walkSpeed = 1f;
    public float runSpeed = 1f;
	public float jumpSpeed = 1f;

	//components
	private Rigidbody rb;

	//states
    private bool runActive = false;
    private bool running = false;
	private bool crouching = false;

	//timers
    private float runStart = 0f;
    private float leftStart = 0f;
    private float rightStart = 0f;

	private MyMessageListener SerialControllerM;
	// Use this for initialization
	void Start () {
		SerialControllerM = GameObject.Find("SerialController").GetComponent<MyMessageListener>();
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		//running timers
        runStart -= Time.deltaTime;
        leftStart -= Time.deltaTime;
        rightStart -= Time.deltaTime;

		Debug.Log(SerialControllerM.Q);

		
		//Running
        if (Input.GetKeyDown(KeyCode.Q) || SerialControllerM.Q)
        {
            leftStart = .275f;
        }

        if (Input.GetKeyDown(KeyCode.W) || SerialControllerM.W)
        {
            rightStart = .275f;
        }

        if (runActive && rightStart > 0 && leftStart > 0)
        {
            running = true;
        }

        else if (rightStart > 0 && leftStart > 0)
        {
            runActive = true;
            runStart = .7f;
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

		//walking and turning
        if (!running && (Input.GetKey(KeyCode.Q) ||  SerialControllerM.Q) && (Input.GetKey(KeyCode.W) || SerialControllerM.W))
        {
            transform.position += transform.forward * walkSpeed * Time.deltaTime;
        }

        else if (!running && (Input.GetKey(KeyCode.Q) ||  SerialControllerM.Q))
        {
            transform.Rotate(Vector3.down * rotateSpeed * Time.deltaTime);
        }

        else if (!running && (Input.GetKey(KeyCode.W) || SerialControllerM.W))
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }

		//crouching and jumping
		if(Input.GetKey(KeyCode.A)){
			crouching = true;
		}
		else{
			crouching = false;
		}

		if(Input.GetKey(KeyCode.S) && crouching){
			Jump();
		}

        
    }

	private void Jump(){
		 float DisstanceToTheGround = GetComponent<Collider>().bounds.extents.y;
 
         bool IsGrounded = Physics.Raycast(transform.position, Vector3.down, DisstanceToTheGround + 0.1f);

		 if(IsGrounded){
		 	 rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
		 }
	}
}

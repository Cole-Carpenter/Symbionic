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
	private Animator ac;
	private Animator sac;
	private ConstantForce f;
	public GameObject spring;
	
	//boxes
	private GameObject [] boxes;

	//states
    private bool runActive = false;
    private bool running = false;
	private bool crouching = false;
	private bool grounded = true;

	//abilities
	public bool canGlide = true;
	public bool canDiveBomb = false;

	//timers
    private float runStart = 0f;
    private float leftStart = 0f;
    private float rightStart = 0f;
	private float pJumpStart = 0f;

	private MyMessageListener SerialControllerM;
	// Use this for initialization
	void Start () {
		SerialControllerM = GameObject.Find("SerialController").GetComponent<MyMessageListener>();
		rb = GetComponent<Rigidbody>();
		ac = GetComponent<Animator>();
		sac = spring.GetComponent<Animator>();
		f = GetComponent<ConstantForce>();
	}
	
	// Update is called once per frame
	void Update () {
		//running timers
        runStart -= Time.deltaTime;
        leftStart -= Time.deltaTime;
        rightStart -= Time.deltaTime;
		pJumpStart -= Time.deltaTime;

		grounded = CheckGrounded();

		
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
			ac.SetBool("running",true);
            running = true;
        }

        else if (rightStart > 0 && leftStart > 0)
        {
            runActive = true;
			leftStart = 0;
			rightStart = 0;
            runStart = .7f;
        }

        else if (runStart <= 0)
        {
            runActive = false;
            running = false;
			ac.SetBool("running",false);
        }

        if (running)
        {
            transform.position += transform.forward * runSpeed * Time.deltaTime;
        }

		//walking and turning
        if (!running && (Input.GetKey(KeyCode.Q) ||  SerialControllerM.Q) && (Input.GetKey(KeyCode.W) || SerialControllerM.W))
        {
			ac.SetBool("walking", true);
            transform.position += transform.forward * walkSpeed * Time.deltaTime;
        }

        else if (!running && (Input.GetKey(KeyCode.Q) ||  SerialControllerM.Q))
        {
			ac.SetBool("walking", true);
            transform.Rotate(Vector3.down * rotateSpeed * Time.deltaTime);
        }

        else if (!running && (Input.GetKey(KeyCode.W) || SerialControllerM.W))
        {
			ac.SetBool("walking", true);
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
		else{
			ac.SetBool("walking", false);
		}

		//crouching and jumping
		if(Input.GetKey(KeyCode.A)){
			crouching = true;
		}
		else{
			crouching = false;
		}

		if(Input.GetKeyUp(KeyCode.A)){
			pJumpStart = .1f;
		}

		if(pJumpStart > 0 && Input.GetKeyDown(KeyCode.S)){
			Jump(true);
			canDiveBomb = true;
		}

		else if(Input.GetKeyDown(KeyCode.S) && crouching){
			Jump(false);
			canDiveBomb = true;
		}

		//gliding
		else if(!grounded && Input.GetKey(KeyCode.S) && canGlide){
			Glide();
		}

		else{
			EndGlide();
		}

		//DiveBomb
		canDiveBomb = !grounded;

		if(canDiveBomb && Input.GetKeyDown(KeyCode.A)){
			DiveBomb();
		}

		else if (grounded){
			EndDiveBomb();
		}
        
    }

	private void Jump(bool super){
		 
		 float height;

		 if(super){
		 	 height = jumpSpeed * 2;
		 }
		 else{
		 	 height = jumpSpeed;
		 }

		 if(grounded){
			ac.SetTrigger("jump");
			sac.SetTrigger("jump");
		 	rb.AddForce(Vector3.up * height, ForceMode.Impulse);
		 }
	}

	private void Glide(){
		walkSpeed = runSpeed;
		rb.drag = .75f;
		f.force = Vector3.up * 8.5f;
	}

	private void EndGlide(){
		walkSpeed = 4;
		rb.drag = 0;
		if(f.force.y > 0){
			f.force = new Vector3(0,0,0);
		}
	}

	private void DiveBomb(){
		f.force = -Vector3.up * 50f;
	}

	private void EndDiveBomb(){
		if(f.force.y < 0){
			f.force = new Vector3(0,0,0);
		}
	}

	private bool CheckGrounded(){
		float DistanceToTheGround = GetComponent<Collider>().bounds.extents.y;
		bool IsGrounded = Physics.Raycast(transform.position, Vector3.down, DistanceToTheGround + 0.1f);
		return IsGrounded;
	}

	private void OnTriggerEnter(Collider other){
	
	}
}

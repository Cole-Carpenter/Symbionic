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
    private AudioSource aso;
    private ConstantForce f;
    private Collider magnetMask;
    public GameObject radar;
    public Renderer track;
    public Collider boxCollider;
    private UIController uic;

    //lists of interactables
    private List<GameObject> boxes;
    private List<Rigidbody> magnetics;

    //states
    private bool gliding;
    private bool runActive = false;
    private bool running = false;
    private bool crouching = false;
    private bool grounded = true;
    private bool updraftActive = false;

    //abilities Unlockable
    public bool canGlide = false;
    public bool canDiveBomb = false;
    public bool canUpdraft = false;

    //abilities USB
    //Organic
    public bool canSqueak = true;
    public States usbState = (States)0;

    //timers
    private float runStart = 0f;
    private float leftStart = 0f;
    private float rightStart = 0f;
    private float pJumpStart = 0f;
    private float updraftTimer = 0f;

    private MyMessageListener SerialControllerM;
    // Use this for initialization
    void Start() {
        //SerialControllerM = GameObject.Find("SerialController").GetComponent<MyMessageListener>();
        rb = GetComponent<Rigidbody>();
        ac = GetComponentInChildren<Animator>();
        aso = GetComponent<AudioSource>();
        uic = GetComponent<UIController>();
        f = GetComponent<ConstantForce>();
        boxes = new List<GameObject>();
        magnetMask = transform.Find("MagnetMask").GetComponent<SphereCollider>();
        magnetics = new List<Rigidbody>();
    }
    /*
	 0f - left
	 2 - crouch
	 4 - Interact
	 6 - Radar / magnet

	 1 - right
	 3 - Jump
	 5 - Nibble
	 7 - Squeak / Dig

	*/
    // Update is called once per frame
    void Update() {
        //running timers
        runStart -= Time.deltaTime;
        leftStart -= Time.deltaTime;
        rightStart -= Time.deltaTime;
        pJumpStart -= Time.deltaTime;
        updraftTimer -= Time.deltaTime;

        if (CheckGrounded() == true) {
            grounded = true;
            ac.SetBool("grounded", true);
            updraftActive = true;
        }
        else {
            grounded = false;
            ac.SetBool("grounded", false);
        }



        //Running
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            leftStart = .275f;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            rightStart = .275f;
        }

        if (runActive && rightStart > 0 && leftStart > 0)
        {
            ac.SetBool("running", true);
            track.material.SetFloat("_Speed2", 12f);
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
            ac.SetBool("running", false);
            track.material.SetFloat("_Speed2", 0f);
        }

        if (running)
        {
            rb.MovePosition(rb.position + transform.forward * runSpeed * Time.deltaTime);
        }

        //walking and turning
        if (!running && (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.JoystickButton0)) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.JoystickButton1)))
        {
            ac.SetBool("walking", true);
            track.material.SetFloat("_Speed2", 6f);
            rb.MovePosition(rb.position + transform.forward * walkSpeed * Time.deltaTime);
        }

        else if (!running && (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.JoystickButton0)))
        {
            ac.SetBool("walking", true);
            track.material.SetFloat("_Speed2", 6f);
            rb.MovePosition(rb.position + transform.forward * walkSpeed * Time.deltaTime);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.down * rotateSpeed * Time.deltaTime));
        }

        else if (!running && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.JoystickButton1)))
        {
            ac.SetBool("walking", true);
            track.material.SetFloat("_Speed2", 6f);
            rb.MovePosition(rb.position + transform.forward * walkSpeed * Time.deltaTime);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * rotateSpeed * Time.deltaTime));
        }
        else if (running)
        {
            ac.SetBool("walking", false);
        }
        else {
            ac.SetBool("walking", false);
            track.material.SetFloat("_Speed2", 0f);
        }

        //crouching and jumping
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.JoystickButton2)) {
            pJumpStart = 3f;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.JoystickButton2)) {
            crouching = true;
            ac.SetBool("crouch", true);
        }
        else {
            crouching = false;
            ac.SetBool("crouch", false);
        }
        if ((pJumpStart < 0 && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.JoystickButton3))) && crouching)
        {
            Jump(true);
            pJumpStart = 3f;
        }
        else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.JoystickButton3)) && crouching) {
            Jump(false);
            pJumpStart = 3f;
        }

        //gliding
        if (!grounded && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.JoystickButton3)) && canUpdraft && updraftActive && canGlide) {
            updraftActive = false;
            Updraft();
        }

        if (!grounded && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.JoystickButton3)) && canGlide) {
            Glide();
        }

        else if ((updraftTimer < 0 && Input.GetKeyUp(KeyCode.S)) || grounded){
			EndGlide();
		}

		//DiveBomb

		if(canDiveBomb && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.JoystickButton2) && !grounded)){
			DiveBomb();
		}
        
		//squeak
		if(canSqueak && (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.JoystickButton7))){
            aso.Play();
            foreach (GameObject box in boxes){
				uic.SendCode(box.GetComponent<BoxInteract>().Interact(null));
			}
		}

		//Interact
		if(Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.JoystickButton4)){
			Interact();
		}

		//nibble
		if(Input.GetKeyDown(KeyCode.Z)|| Input.GetKeyDown(KeyCode.JoystickButton5)){
			Nibble();
		}

		//dig
		if(usbState == (States)2 && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton7))){
			Dig();
		}

		//radar
		if(usbState == (States)0 && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton8))){
			radar.GetComponent<Radar>().Ping();
		}

        //magnet
        if (usbState == (States)1 && Input.GetKey(KeyCode.X))
        {
            foreach (Rigidbody magnetic in magnetics)
            {
                if(magnetic.gameObject.transform.parent == transform)
                {
                    magnetic.transform.rotation = magnetic.transform.rotation * Quaternion.Euler(new Vector3(50f, 50f, 50f) * Time.deltaTime);
                }
                else if (magnetic.gameObject.transform.parent != transform && magnetMask.bounds.Contains(magnetic.position))
                {
                    magnetic.gameObject.transform.parent = transform;
                    magnetic.isKinematic = true;
                }
                else if (magnetic.gameObject.transform.parent != transform && !magnetMask.bounds.Contains(magnetic.position))
                {
                    magnetic.MovePosition(Vector3.MoveTowards(magnetic.position, rb.position, .25f));
                }
            }
        }
        else if(!Input.GetKey(KeyCode.X))
        {
            foreach (Rigidbody magnetic in magnetics)
            {
                magnetic.isKinematic = false;
                magnetic.gameObject.transform.parent = null;
            }
        }
    }

	private void Dig(){
		RaycastHit hit;

		if (Physics.SphereCast(transform.position, 2f, transform.forward, out hit, 2f)){
			if(hit.transform.tag == "diggable"){
				uic.SendCode(hit.transform.gameObject.GetComponent<Interactable>().Interact(transform));
			}
		}
	}

	private void Nibble(){
		
		RaycastHit hit;

		if (Physics.SphereCast(transform.position, 4f, transform.forward, out hit, 4f)){
			if(hit.transform.tag == "bitable"){
				hit.transform.gameObject.GetComponent<Interactable>().Interact(transform);
			}
		}
	}

	private void Interact(){
		
		RaycastHit hit;

		if (Physics.SphereCast(transform.position, 4f, transform.forward, out hit, 4f)){
			if(hit.transform.tag == "interactable"){
				hit.transform.gameObject.GetComponent<Interactable>().Interact(transform);
			}
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
		 	rb.AddForce(Vector3.up * height, ForceMode.Impulse);
		 }
	}

	private void Glide(){
        gliding = true;
		ac.SetBool("glide",true);
		walkSpeed = runSpeed;
		f.force = Vector3.up * 4.9f;
	}

	private void EndGlide(){
        gliding = false;
		ac.SetBool("glide",false);
		walkSpeed = 4;
        f.force = new Vector3(0, 0, 0);
    }

	private void Updraft(){
		ac.SetBool("glide",true);
		walkSpeed = runSpeed;
        rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }

	private void DiveBomb(){
		f.force = -Vector3.up * 50f;
	}

	private bool CheckGrounded(){
		float DistanceToTheGround = boxCollider.bounds.extents.y;
        float err;
        if (gliding)
        {
            err = 5f;
        }
        else
        {
            err = 0f;
        }
        Color rayColor;
		bool IsGrounded = Physics.Raycast(transform.position, Vector3.down, DistanceToTheGround + err);
        if (IsGrounded)
        {
            rayColor = Color.black;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(transform.position, Vector3.down.normalized * (DistanceToTheGround + err), rayColor, Time.deltaTime);
        return IsGrounded;
	}

	private void OnTriggerEnter(Collider other){
		if(other.tag == "box"){
			boxes.Add(other.gameObject);
		}
		if(other.tag == "magnetic"){
			magnetics.Add(other.gameObject.GetComponent<Rigidbody>());
		}
	}

	private void OnTriggerExit(Collider other){
		if(other.tag == "box"){
			boxes.Remove(other.gameObject);
		}
		if(other.tag == "magnetic"){
			magnetics.Remove(other.gameObject.GetComponent<Rigidbody>());
		}
	}
}

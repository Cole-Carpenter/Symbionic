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
	private Animator bac;
    private AudioSource aso;
	private ConstantForce f;
	public GameObject spring;
	public GameObject radar;
	public GameObject blades;
	public Collider boxCollider;
	private UIController uic;
	
	//lists of interactables
	private List<GameObject> boxes;
	private List<GameObject> magnetics;

	//states
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
	public bool canDig = false;
	public bool canBlood = false; // Not added yet
		//Mechanical
	public bool canRadar = true;
	public bool canMagnet = false;
	public bool canOil = false; //Not added yet

	//timers
    private float runStart = 0f;
    private float leftStart = 0f;
    private float rightStart = 0f;
	private float pJumpStart = 0f;
	private float updraftTimer = 0f;

	private MyMessageListener SerialControllerM;
	// Use this for initialization
	void Start () {
		//SerialControllerM = GameObject.Find("SerialController").GetComponent<MyMessageListener>();
		rb = GetComponent<Rigidbody>();
		ac = GetComponent<Animator>();
        aso = GetComponent<AudioSource>();
		sac = spring.GetComponent<Animator>();
		bac = blades.GetComponent<Animator>();
		uic = GetComponent<UIController>();
		f = GetComponent<ConstantForce>();
		boxes = new List<GameObject>();
		magnetics = new List<GameObject>();
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
	void Update () {
		//running timers
        runStart -= Time.deltaTime;
        leftStart -= Time.deltaTime;
        rightStart -= Time.deltaTime;
		pJumpStart -= Time.deltaTime;
		updraftTimer -= Time.deltaTime;

		if(CheckGrounded() == true){
			grounded = true;
			updraftActive = true;
		}
		else{
			grounded = false;
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
        if (!running && (Input.GetKey(KeyCode.Q) ||  Input.GetKey(KeyCode.JoystickButton0)) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.JoystickButton1)))
        {
			ac.SetBool("walking", true);
            transform.position += transform.forward * walkSpeed * Time.deltaTime;
        }

        else if (!running && (Input.GetKey(KeyCode.Q) ||  Input.GetKey(KeyCode.JoystickButton0)))
        {
			ac.SetBool("walking", true);
            transform.Rotate(Vector3.down * rotateSpeed * Time.deltaTime);
        }

        else if (!running && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.JoystickButton1)))
        {
			ac.SetBool("walking", true);
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
		else{
			ac.SetBool("walking", false);
		}

		//crouching and jumping
		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.JoystickButton2)){
			crouching = true;
			ac.SetBool("crouch",true);
		}
		else{
			crouching = false;
			ac.SetBool("crouch",false);
		}

		if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.JoystickButton2)){
			pJumpStart = .1f;
		}

		if(pJumpStart > 0 && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.JoystickButton3))){
			Jump(true);
			canDiveBomb = true;
		}

		else if((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.JoystickButton3)) && crouching){
			Jump(false);
			canDiveBomb = true;
		}

		//gliding
		else if(!grounded && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.JoystickButton3)) && canGlide && canUpdraft && updraftActive){
			updraftActive = false;
			Updraft();
		}

		else if(!grounded && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.JoystickButton3)) && canGlide && updraftTimer < 0){
			Glide();
		}

		else if(updraftTimer < 0){
			EndGlide();
		}

		//DiveBomb
		canDiveBomb = !grounded;

		if(canDiveBomb && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.JoystickButton2))){
			DiveBomb();
		}

		else if (grounded){
			EndDiveBomb();
		}
        
		//squeak
		if(canSqueak && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton7))){
            aso.Play();
            foreach (GameObject box in boxes){
				uic.SendCode(box.GetComponent<BoxInteract>().Interact());
			}
		}

		//Interact
		if(Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.JoystickButton4)){
			Interact();
		}

		//nibble
		if(Input.GetKeyDown(KeyCode.N)|| Input.GetKeyDown(KeyCode.JoystickButton5)){
			Nibble();
		}

		//dig
		if(canDig && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton7))){
			Dig();
		}

		//radar
		if(canRadar && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.JoystickButton8))){
			radar.GetComponent<Radar>().Ping();
		}

		//magnet
		if(canMagnet && Input.GetKey(KeyCode.Z)){
			foreach(GameObject magnetic in magnetics){
				magnetic.transform.position = Vector3.MoveTowards(magnetic.transform.position, transform.position, .1f);
			}
		}
    }
	private void Dig(){
		RaycastHit hit;

		if (Physics.SphereCast(transform.position, 2f, transform.forward, out hit, 2f)){
			if(hit.transform.tag == "diggable"){
				uic.SendCode(hit.transform.gameObject.GetComponent<Interactable>().Interact());
			}
		}
	}

	private void Nibble(){
		
		RaycastHit hit;

		if (Physics.SphereCast(transform.position, 4f, transform.forward, out hit, 4f)){
			if(hit.transform.tag == "bitable"){
				hit.transform.gameObject.GetComponent<Interactable>().Interact();
			}
		}
	}

	private void Interact(){
		
		RaycastHit hit;

		if (Physics.SphereCast(transform.position, 4f, transform.forward, out hit, 4f)){
			if(hit.transform.tag == "interactable"){
				hit.transform.gameObject.GetComponent<Interactable>().Interact();
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
			sac.SetTrigger("jump");
		 	rb.AddForce(Vector3.up * height, ForceMode.Impulse);
		 }
	}

	private void Glide(){
		blades.SetActive(true);
		bac.SetBool("glide",true);
		ac.SetBool("glide",true);
		walkSpeed = runSpeed;
		rb.drag = .75f;
		f.force = Vector3.up * 8.5f;
	}

	private void EndGlide(){
		blades.SetActive(false);
		bac.SetBool("glide",false);
		ac.SetBool("glide",false);
		walkSpeed = 4;
		rb.drag = 0;
		if(f.force.y > 0){
			f.force = new Vector3(0,0,0);
		}
	}

	private void Updraft(){
		blades.SetActive(true);
		bac.SetBool("glide",true);
		ac.SetBool("glide",true);
		updraftTimer = 3f;
		walkSpeed = runSpeed;
		rb.drag = .75f;
		f.force = Vector3.up * 20f;
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
		float DistanceToTheGround = boxCollider.bounds.extents.y;
		bool IsGrounded = Physics.Raycast(transform.position, Vector3.down, DistanceToTheGround + 0.1f);
		return IsGrounded;
	}

	private void OnTriggerEnter(Collider other){
		if(other.tag == "box"){
			boxes.Add(other.gameObject);
		}
		if(other.tag == "magnetic"){
			magnetics.Add(other.gameObject);
		}
		//print(magnetics);
	}

	private void OnTriggerExit(Collider other){
		if(other.tag == "box"){
			boxes.Remove(other.gameObject);
		}
		if(other.tag == "magnetic"){
			magnetics.Remove(other.gameObject);
		}
		//print(magnetics);
	}
}

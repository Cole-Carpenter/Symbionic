using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //speeds
    public float rotateSpeed = 4f;
    public float walkSpeed = 1f;
    private float defaultWalkSpeed;
    public float runSpeed = 1f;
    public float sqrMaxVelocity = 1f;
    public float jumpSpeed = 1f;
    private float glideSpeed;

    //components
    private Rigidbody rb;
    private Animator ac;
    private AudioSource aso;
    private AudioSource gaso;
    private AudioSource maso;
    private ConstantForce f;
    private Collider magnetMask;
    private MeshRenderer mMMeshRenderer;
    public GameObject radar;
    public Renderer track;
    public Collider boxCollider;
    private UIController uic;

    //audio clips
    public AudioClip squeakClip;
    public AudioClip pJumpClip;
    public AudioClip jumpClip;
    public AudioClip diveBombClip;
    public AudioClip glideClip;
    public AudioClip nibbleClip;
    public AudioClip updraftClip;
    public AudioClip magnetClip;
    public AudioClip digClip;

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
    private bool pJumpStartSoundActive = false;

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
        gaso = transform.Find("SymbionicIdle").GetComponent<AudioSource>();
        maso = transform.Find("MagnetMask").GetComponent<AudioSource>();
        uic = GetComponent<UIController>();
        f = GetComponent<ConstantForce>();
        boxes = new List<GameObject>();
        magnetMask = transform.Find("MagnetMask").GetComponent<SphereCollider>();
        mMMeshRenderer = magnetMask.GetComponent<MeshRenderer>();
        magnetics = new List<Rigidbody>();
        walkSpeed *= 10f;
        defaultWalkSpeed = walkSpeed;
        runSpeed *= 1000f;
        glideSpeed = walkSpeed * 2f;
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

        if (CheckGrounded(.001f) == true) {
            grounded = true;
            ac.SetBool("grounded", true);
            updraftActive = true;
        }
        else {
            grounded = false;
            ac.SetBool("grounded", false);
        }



        //Running
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton10))
        {
            leftStart = 15f * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            rightStart = 15f * Time.deltaTime;
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
            runStart = .3f;
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
            rb.drag = 1;
            if(rb.velocity.sqrMagnitude < sqrMaxVelocity)
            {
                rb.AddForce(transform.forward * runSpeed * Time.deltaTime, ForceMode.Acceleration);
            }   
        }

        //walking and turning
        else if (!running && (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.JoystickButton10)) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.JoystickButton3)))
        {
            rb.drag = 4f;
            ac.SetBool("walking", true);
            track.material.SetFloat("_Speed2", 6f);
            rb.AddForce(transform.forward * walkSpeed * Time.deltaTime, ForceMode.VelocityChange);
        }

        else if (!running && (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.JoystickButton10)))
        {
            rb.drag = 4f;
            ac.SetBool("walking", true);
            track.material.SetFloat("_Speed2", 6f);
            rb.AddForce(0.5f * transform.forward * walkSpeed * Time.deltaTime, ForceMode.VelocityChange);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.down * rotateSpeed * Time.deltaTime));
        }

        else if (!running && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.JoystickButton3)))
        {
            rb.drag = 4f;
            ac.SetBool("walking", true);
            track.material.SetFloat("_Speed2", 6f);
            rb.AddForce(0.5f * transform.forward * walkSpeed * Time.deltaTime, ForceMode.VelocityChange);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * rotateSpeed * Time.deltaTime));
        }
        else {
            ac.SetBool("walking", false);
            track.material.SetFloat("_Speed2", 0f);
            if(rb.velocity != new Vector3(0, 0, 0) && grounded)
            {
                rb.drag = 4f;
            }
        }

        //crouching and jumping
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.JoystickButton11)) {
            pJumpStart = 3f;
            pJumpStartSoundActive = true;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.JoystickButton11)) {
            crouching = true;
            ac.SetBool("crouch", true);
        }
        else {
            crouching = false;
            ac.SetBool("crouch", false);
        }

        if(pJumpStart < 0f && pJumpStartSoundActive && crouching)
        {
            pJumpStartSoundActive = false;
            aso.clip = pJumpClip;
            aso.Play();
        }
        if ((pJumpStart < 0 && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.JoystickButton0))) && crouching)
        {
            Jump(true);
            pJumpStart = 3f;
        }
        else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.JoystickButton0)) && crouching) {
            Jump(false);
            pJumpStart = 3f;
        }

        //gliding
        if (!CheckGrounded(5f) && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.JoystickButton0)) && canUpdraft && updraftActive && canGlide) {
            updraftActive = false;
            Updraft();
        }

        if (!CheckGrounded(5f) && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.JoystickButton0)) && canGlide && !gliding) {
            Glide();
        }

        else if ((updraftTimer < 0 && Input.GetKeyUp(KeyCode.S)) || CheckGrounded(5f) && gliding){
			EndGlide();
		}

		//DiveBomb

		if(canDiveBomb && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.JoystickButton11)) && !grounded){
			DiveBomb();
		}
        
		//squeak
		if(canSqueak && (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.JoystickButton9))){
            aso.clip = squeakClip;
            aso.Play();
            foreach (GameObject box in boxes){
                string code = box.GetComponent<Interactable>().Interact(null);
                string[] pToScreen = code.Split('-');
                if (code != "")
                {
                    uic.SendCode(pToScreen[0],pToScreen[1]);
                }
			}
		}

		//Interact -- but not any more cause it's super redundant
		if(Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.JoystickButton1)){
			Interact();
		}

		//nibble
		if(Input.GetKeyDown(KeyCode.Z)|| Input.GetKeyDown(KeyCode.JoystickButton8) && grounded){
			Nibble();
		}

		//dig
		if(usbState == (States)2 && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton2)) && grounded){
            Dig();
		}

		//radar
		if(usbState == (States)0 && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton2))){
			radar.GetComponent<Radar>().Ping();
		}

        //magnet
        if(usbState == (States)1 && Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            maso.clip = magnetClip;
            maso.Play();
            mMMeshRenderer.enabled = true;
        }
        if (usbState == (States)1 && Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.JoystickButton2))
        {
            foreach (Rigidbody magnetic in magnetics)
            {
                if (magnetMask.bounds.Contains(magnetic.position))
                {
                    magnetic.drag = 4f;
                }
                else if (magnetic.gameObject.transform.parent != transform && !magnetMask.bounds.Contains(magnetic.position))
                {
					magnetic.drag = 1f;
                    magnetic.MovePosition(Vector3.MoveTowards(magnetic.position, rb.position, .25f));
                }
            }
        }
        if(Input.GetKeyUp(KeyCode.X) || Input.GetKeyUp(KeyCode.JoystickButton2))
        {
            maso.Stop();
            mMMeshRenderer.enabled = false;
            foreach (Rigidbody magnetic in magnetics)
            {
                magnetic.isKinematic = false;
                magnetic.gameObject.transform.parent = null;
            }
        }
    }

	private void Dig(){
        aso.clip = digClip;
        aso.Play();

        RaycastHit hit;

		if (Physics.SphereCast(transform.position, 2f, transform.forward, out hit, 2f)){
			if(hit.transform.tag == "diggable"){
                string code = hit.transform.gameObject.GetComponent<Interactable>().Interact(transform);
                string[] pToScreen = code.Split('-');
                uic.SendCode(pToScreen[0],pToScreen[1]);
			}
		}
	}

	private void Nibble(){
        aso.clip = nibbleClip;
        aso.Play();

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
            aso.clip = jumpClip;
            aso.Play();
			ac.SetTrigger("jump");
		 	rb.AddForce(Vector3.up * height, ForceMode.Impulse);
		 }
	}

	private void Glide(){
        gliding = true;
        gaso.clip = glideClip;
        gaso.Play();
		ac.SetBool("glide",true);
		walkSpeed = glideSpeed;
		f.force = Vector3.up * 20f;
	}

	private void EndGlide(){
        gliding = false;
        gaso.Stop();
		ac.SetBool("glide",false);
		walkSpeed = defaultWalkSpeed;
        f.force = new Vector3(0, 0, 0);
    }

	private void Updraft(){
		ac.SetBool("glide",true);
        aso.clip = updraftClip;
        aso.Play();
		walkSpeed = runSpeed;
        rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }

	private void DiveBomb(){
        aso.clip = diveBombClip;
        aso.Play();
        f.force = -Vector3.up * 50f;
	}

	private bool CheckGrounded(float err){
		float DistanceToTheGround = boxCollider.bounds.extents.y;
        Color rayColor;
		bool IsGrounded = Physics.Raycast(transform.position, Vector3.down, DistanceToTheGround + err);

        /*
		if (IsGrounded)
        {
            rayColor = Color.black;
        }
        else
        {
            rayColor = Color.red;
        }
        //Debug.DrawRay(transform.position, Vector3.down.normalized * (DistanceToTheGround + err), rayColor, Time.deltaTime);
		*/

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

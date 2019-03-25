using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    //temp-states
    public bool updraftActive = false;
    public bool runActive = false;
    public bool pJumpStartSoundActive = false;

    //timers
    private float runStart = 0f;
    private float leftStart = 0f;
    private float rightStart = 0f;
    private float pJumpStart = 0f;
    private float updraftTimer = 0f;
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
    void Update()
    {
        //running timers
        runStart -= Time.deltaTime;
        leftStart -= Time.deltaTime;
        rightStart -= Time.deltaTime;
        pJumpStart -= Time.deltaTime;
        updraftTimer -= Time.deltaTime;

        if (CheckGrounded(.001f) == true)
        {
            SymApp.instance.status.grounded = true;
            SymApp.instance.scene.player.ac.SetBool("grounded", true);
            updraftActive = true;
        }
        else
        {
            SymApp.instance.status.grounded = false;
            SymApp.instance.scene.player.ac.SetBool("grounded", false);
        }



        //Running
        if (Input.GetButtonDown("Left"))
        {
            print("here");
            leftStart = 15f * Time.deltaTime;
        }

        if (Input.GetButtonDown("Right"))
        {
            rightStart = 15f * Time.deltaTime;
        }

        if (runActive && rightStart > 0 && leftStart > 0)
        {
            SymApp.instance.scene.player.StartRun();
            SymApp.instance.status.running = true;
            runStart = 3f * Time.deltaTime;
        }

        else if (rightStart > 0 && leftStart > 0)
        {
            runActive = true;
            leftStart = 0;
            rightStart = 0;
        }

        else if (SymApp.instance.status.running && runStart < 0)
        {
            runActive = false;
            SymApp.instance.status.running = false;
            SymApp.instance.scene.player.StopRun();
        }

        //walking and turning
        else if (!SymApp.instance.status.running && Input.GetButton("Left") && Input.GetButton("Right"))
        {
            SymApp.instance.scene.player.Walk();
        }

        else if (!SymApp.instance.status.running && Input.GetButton("Left"))
        {
            SymApp.instance.scene.player.Row(-1);
        }

        else if (!SymApp.instance.status.running && Input.GetButton("Right"))
        {
            SymApp.instance.scene.player.Row(1);
        }

        else
        {
            SymApp.instance.scene.player.StopWalk();
        }

        //crouching and jumping
        if (Input.GetButtonDown("Crouch"))
        {
            pJumpStart = 3f;
            pJumpStartSoundActive = true;
        }
        if (Input.GetButton("Crouch"))
        {
            SymApp.instance.status.crouching = true;
            SymApp.instance.scene.player.ac.SetBool("crouch", true);
        }
        else
        {
            SymApp.instance.status.crouching = false;
            SymApp.instance.scene.player.ac.SetBool("crouch", false);
        }

        if (pJumpStart < 0f && pJumpStartSoundActive && SymApp.instance.status.crouching)
        {
            pJumpStartSoundActive = false;
            SymApp.instance.scene.player.PerfectJumpAntic();
        }

        if (pJumpStart < 0 && Input.GetButtonDown("Jump") && SymApp.instance.status.crouching)
        {
            SymApp.instance.scene.player.Jump(true);
            pJumpStart = 3f;
        }
        else if (Input.GetButtonDown("Jump") && SymApp.instance.status.crouching)
        {
            SymApp.instance.scene.player.Jump(false);
            pJumpStart = 3f;
        }

        //gliding
        if (!CheckGrounded(5f) && Input.GetButtonDown("Jump") && SymApp.instance.status.canUpdraft && updraftActive && SymApp.instance.status.canGlide)
        {
            updraftActive = false;
            SymApp.instance.status.walkSpeed = SymApp.instance.status.runSpeed;
            SymApp.instance.scene.player.Updraft();
        }

        if (!CheckGrounded(5f) && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.JoystickButton0)) && SymApp.instance.status.canGlide && !SymApp.instance.status.gliding)
        {
            SymApp.instance.status.gliding = true;
            SymApp.instance.status.walkSpeed = SymApp.instance.status.glideSpeed;
            SymApp.instance.scene.player.Glide();
        }

        else if ((updraftTimer < 0 && Input.GetKeyUp(KeyCode.S)) || CheckGrounded(5f) && SymApp.instance.status.gliding)
        {
            SymApp.instance.scene.player.EndGlide();
            SymApp.instance.status.gliding = false;
            SymApp.instance.status.ResetWalkSpeed();
        }

        //DiveBomb

        if (SymApp.instance.status.canDiveBomb && Input.GetButtonDown("Crouch") && !SymApp.instance.status.grounded)
        {
            SymApp.instance.scene.player.DiveBomb();
        }

        //squeak
        if (SymApp.instance.status.canSqueak && Input.GetButtonDown("Squeak"))
        {
            SymApp.instance.scene.player.Squeak();
            foreach (GameObject box in SymApp.instance.status.boxes)
            {
                string code = box.GetComponent<Interactable>().Interact(null);
                string[] pToScreen = code.Split('-');
                if (code != "")
                {
                    SymApp.instance.manager.ui.SendCode(pToScreen[0], pToScreen[1]);
                }
            }
        }

        //Interact -- but not any more cause it's super redundant
        if (Input.GetButtonDown("Interact"))
        {
            SymApp.instance.scene.player.Interact();
        }

        //nibble
        if (Input.GetButtonDown("Nibble") && SymApp.instance.status.grounded)
        {
            SymApp.instance.scene.player.Nibble();
        }

        //dig
        if (SymApp.instance.status.usbState == (States)2 && Input.GetKeyDown("USB_Button") && SymApp.instance.status.grounded)
        {
            SymApp.instance.scene.player.Dig();
        }

        //radar
        if (SymApp.instance.status.usbState == (States)0 && Input.GetKeyDown("USB_Button"))
        {
            SymApp.instance.scene.player.radar.GetComponent<Radar>().Ping();
        }

        //magnet
        if (SymApp.instance.status.usbState == (States)1 && Input.GetKeyDown("USB_Button"))
        {
            SymApp.instance.scene.player.Magnet();
            
        }
        if (SymApp.instance.status.usbState == (States)1 && Input.GetButton("USB_Button"))
        {
            SymApp.instance.scene.player.MagnetPull();
        }
        if (Input.GetButtonUp("USB_Button"))
        {
            SymApp.instance.scene.player.MagnetEnd();
        }
    }

	private bool CheckGrounded(float err){
		float DistanceToTheGround = SymApp.instance.scene.player.boxCollider.bounds.extents.y;
		bool IsGrounded = Physics.Raycast(transform.position, Vector3.down, DistanceToTheGround + err);

        /*
        Color rayColor;
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
			SymApp.instance.status.boxes.Add(other.gameObject);
		}
		if(other.tag == "magnetic"){
			SymApp.instance.status.magnetics.Add(other.gameObject.GetComponent<Rigidbody>());
		}
	}

	private void OnTriggerExit(Collider other){
		if(other.tag == "box"){
			SymApp.instance.status.boxes.Remove(other.gameObject);
		}
		if(other.tag == "magnetic"){
			SymApp.instance.status.magnetics.Remove(other.gameObject.GetComponent<Rigidbody>());
		}
	}
}

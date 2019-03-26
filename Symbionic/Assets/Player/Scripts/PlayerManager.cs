using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    //temp-states
    public bool updraftActive = false;
    public bool runActive = false;
    public bool pJumpStartSoundActive = false;

    private int usb_count = 0;

    //timers
    private float runStart = 0f;
    private float leftStart = 0f;
    private float rightStart = 0f;
    private float pJumpStart = 0f;
    private float updraftTimer = 0f;

    //scene and status
    SymStatus status;
    PlayerScene player;

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

    IEnumerator Start()
    {
        yield return new WaitForSeconds(.001f);
        status = SymApp.Instance.status;
        player = SymApp.Instance.scene.player;
    }

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
            status.grounded = true;
            player.ac.SetBool("grounded", true);
            updraftActive = true;
        }
        else
        {
            status.grounded = false;
            player.ac.SetBool("grounded", false);
        }



        //Running
        if (Input.GetButtonDown("Left"))
        {
            leftStart = 15f * Time.deltaTime;
        }

        if (Input.GetButtonDown("Right"))
        {
            rightStart = 15f * Time.deltaTime;
        }

        if (runActive && rightStart > 0 && leftStart > 0)
        {
            player.StartRun();
            status.running = true;
            runStart = 3f * Time.deltaTime;
        }

        else if (rightStart > 0 && leftStart > 0)
        {
            runActive = true;
            leftStart = 0;
            rightStart = 0;
        }

        else if (status.running && runStart < 0)
        {
            runActive = false;
            status.running = false;
            player.StopRun();
        }

        //walking and turning
        else if (!status.running && Input.GetButton("Left") && Input.GetButton("Right"))
        {
            player.Walk();
        }

        else if (!status.running && Input.GetButton("Left"))
        {
            player.Row(-1);
        }

        else if (!status.running && Input.GetButton("Right"))
        {
            player.Row(1);
        }

        else
        {
            player.StopWalk();
        }

        //crouching and jumping
        if (Input.GetButtonDown("Crouch"))
        {
            pJumpStart = 3f;
            pJumpStartSoundActive = true;
        }
        if (Input.GetButton("Crouch"))
        {
            status.crouching = true;
            player.ac.SetBool("crouch", true);
        }
        else
        {
            status.crouching = false;
            player.ac.SetBool("crouch", false);
        }

        if (pJumpStart < 0f && pJumpStartSoundActive && status.crouching)
        {
            pJumpStartSoundActive = false;
            player.PerfectJumpAntic();
        }

        if (pJumpStart < 0 && Input.GetButtonDown("Jump") && status.crouching)
        {
            player.Jump(true);
            pJumpStart = 3f;
        }
        else if (Input.GetButtonDown("Jump") && status.crouching)
        {
            player.Jump(false);
            pJumpStart = 3f;
        }

        //gliding
        if (!CheckGrounded(.001f) && Input.GetButtonDown("Jump") && status.canUpdraft && updraftActive && status.canGlide)
        {
            updraftActive = false;
            status.walkSpeed = status.runSpeed;
            player.Updraft();
        }

        if (!CheckGrounded(.001f) && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.JoystickButton0)) && status.canGlide && !status.gliding)
        {
            status.gliding = true;
            status.walkSpeed = status.glideSpeed;
            player.Glide();
        }

        else if ((updraftTimer < 0 && Input.GetKeyUp(KeyCode.S)) || CheckGrounded(.001f) && status.gliding)
        {
            player.EndGlide();
            status.gliding = false;
            status.ResetWalkSpeed();
        }

        //DiveBomb

        if (status.canDiveBomb && Input.GetButtonDown("Crouch") && !status.grounded)
        {
            player.DiveBomb();
        }

        //squeak
        if (status.canSqueak && Input.GetButtonDown("Squeak"))
        {
            player.Squeak();
            foreach (GameObject box in status.boxes)
            {
                string code = box.GetComponent<Interactable>().Interact(null);
                string[] pToScreen = code.Split('-');
                if (code != "")
                {
                    SymApp.Instance.manager.ui.SendCode(pToScreen[0], pToScreen[1]);
                }
            }
        }

        //Interact -- but not any more cause it's super redundant
        if (Input.GetButtonDown("Interact"))
        {
            player.Interact();
        }

        //nibble
        if (Input.GetButtonDown("Nibble") && status.grounded)
        {
            player.Nibble();
        }

        //dig
        if (status.usbState == (States)2 && Input.GetButtonDown("USBButton") && status.grounded)
        {
            player.Dig();
        }

        //radar
        if (status.usbState == (States)0 && Input.GetButtonDown("USBButton"))
        {
            player.radar.GetComponent<Radar>().Ping();
        }

        //magnet
        if (status.usbState == (States)1 && Input.GetButtonDown("USBButton"))
        {
            player.Magnet();
            
        }
        if (status.usbState == (States)1 && Input.GetButton("USBButton"))
        {
            player.MagnetPull();
        }
        if (Input.GetButtonUp("USBButton"))
        {
            player.MagnetEnd();
        }

        //swap usb states manually
        if (Input.GetKeyDown(KeyCode.Space))
        {
            usb_count++;
            usb_count = usb_count % 3;
        }
        status.usbState = (States)usb_count;
    }

	private bool CheckGrounded(float err){
		float DistanceToTheGround = player.boxCollider.bounds.extents.y;
		bool IsGrounded = Physics.Raycast(player.transform.position, Vector3.down, DistanceToTheGround + err);

        /*
         * If I ever need the ray drawn out to debug
        Color rayColor;
		if (IsGrounded)
        {
            rayColor = Color.black;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(player.transform.position, Vector3.down.normalized * (DistanceToTheGround + err), rayColor, Time.deltaTime);
		*/

        return IsGrounded;
	}
}

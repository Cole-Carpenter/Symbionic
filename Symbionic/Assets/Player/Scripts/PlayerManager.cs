using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    //temp-states
    public bool updraftActive = false;
    public bool runActive = false;
    public bool pJumpStartSoundActive = false;

    private int usb_count = 0;

    //toggles
    private bool runStart = false;
    private bool leftStart = false;
    private bool rightStart = false;
    private bool pJumpActive = false;

    //coroutine containers
    Coroutine lastLeft;
    Coroutine lastRight;
    Coroutine lastRun;
    Coroutine lastPjump;

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
            if (lastLeft != null)
                StopCoroutine(lastLeft);
            lastLeft = StartCoroutine(StartLeft(2f));
        }
        if (Input.GetButtonDown("Right"))
        {
            if(lastRight != null)
                StopCoroutine(lastRight);
            lastRight = StartCoroutine(StartRight(2f));
        }

        if (runActive && Input.GetButtonDown("Right") && Input.GetButtonDown("Left"))
        {
            if(lastRun != null)
                StopCoroutine(lastRun);
            lastRun = StartCoroutine(StartRun(1f));
        }

        else if (Input.GetButtonDown("Right") && Input.GetButtonDown("Left") && leftStart && rightStart)
        {
            runActive = true;
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
            lastPjump = StartCoroutine(PJump());
        }
        if (Input.GetButton("Crouch"))
        {
            status.crouching = true;
            player.ac.SetBool("crouch", true);
        }
        else
        {
            if (lastPjump != null)
                StopCoroutine(lastPjump);
            pJumpActive = false;
            status.crouching = false;
            player.ac.SetBool("crouch", false);
        }

        if (pJumpActive && Input.GetButtonDown("Jump") && status.crouching)
        {
            player.Jump(true);
        }
        else if (Input.GetButtonDown("Jump") && status.crouching)
        {
            player.Jump(false);
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

        else if ((Input.GetButtonUp("Jump")) || CheckGrounded(.001f) && status.gliding)
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
            player.Ping();
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

    //coroutines for running
    private IEnumerator StartLeft(float time)
    {
        leftStart = true;
        yield return new WaitForSeconds(time);
        leftStart = false;
    }
    private IEnumerator StartRight(float time)
    {
        rightStart = true;
        yield return new WaitForSeconds(time);
        
        rightStart = false;
    }
    private IEnumerator StartRun(float time)
    {
        status.running = true;
        player.StartRun();
        yield return new WaitForSeconds(time);
        player.StopRun();
        runActive = false;
        status.running = false;
    }

    //coroutine for perfect jump
    private IEnumerator PJump()
    {
        pJumpStartSoundActive = true;
        yield return new WaitForSeconds(3f);
        if (Input.GetButton("Crouch"))
        {
            pJumpActive = true;
            pJumpStartSoundActive = false;
            player.PerfectJumpAntic();
        }
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

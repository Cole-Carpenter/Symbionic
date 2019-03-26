using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//script to model the current play data

public enum States { canRadar, canMagnet, canDig };

public class SymStatus : MonoBehaviour
{
    //speeds
    public float rotateSpeed = 4f;
    public float walkSpeed = 1f;
    private float defaultWalkSpeed;
    public float runSpeed = 1f;
    public float sqrMaxVelocity = 1f;
    public float jumpSpeed = 1f;
    public float glideSpeed;
    public float tubeSpeed = 1f;

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
    public AudioClip radarClip;

    //states
    public bool gliding = false;
    public bool crouching = false;
    public bool grounded = true;
    public bool running = false;

    //lists of interactables
    public List<GameObject> boxes;
    public List<Rigidbody> magnetics;

    //abilities Unlockable
    public bool canGlide = false;
    public bool canDiveBomb = false;
    public bool canUpdraft = false;

    //abilities USB
    //Organic
    public bool canSqueak = true;
    public States usbState = (States)0;

    //Menu States
    public GameObject currSubMenu;
    public bool paused = false;
    public bool subMenu = false;

    //Menu Data
    public List<string> codes;
    public AudioMixer am;

    // Start is called before the first frame update
    void Start()
    {
        boxes = new List<GameObject>();
        magnetics = new List<Rigidbody>();
        codes = new List<string>();
        walkSpeed *= 10f;
        defaultWalkSpeed = walkSpeed;
        runSpeed *= 1000f;
        glideSpeed = walkSpeed * 2f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetWalkSpeed()
    {
        walkSpeed = defaultWalkSpeed;
    }

    public int GetCodeLength()
    {
        return codes.Count;
    }

    public string GetCodeAtIndex(int i)
    {
        return codes[i];
    }
    
    public void SetVolume(string group, float val)
    {
        am.SetFloat(group, val);
    }

    private bool ContainsCode(string check)
    {
        return codes.Contains(check);
    }

    public void AddCode(string toAdd)
    {
        if(!ContainsCode(toAdd))
            codes.Add(toAdd);
    }
}

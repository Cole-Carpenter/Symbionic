using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMouse : MonoBehaviour {
    private Animator ac;
    public Transform[] target;
    private Rigidbody rb;

    private bool moving = true;
    public float walkSpeed;
    public float tolerance;
    public float rotSpeed;

    private int index;
    // Use this for initialization
    void Start () {
        ac = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        index = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (moving)
        {
            ac.SetBool("walking", true);
            // The step size is equal to speed times frame time.
            float step = walkSpeed * Time.deltaTime;

            // Move our position a step closer to the target.
            rb.MovePosition(Vector3.MoveTowards(transform.position, target[index].position, step));
            Vector3 _dir = (transform.position - target[index].position).normalized;

            Quaternion _look = Quaternion.LookRotation(-_dir);
            rb.rotation = Quaternion.Slerp(transform.rotation, _look, Time.deltaTime * rotSpeed);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "chain")
        {
            index++;
            if(index == target.Length)
            {
                moving = false;
                ac.SetBool("walking", false);
                ac.SetBool("idle", true);
            }
        }
    }
    }

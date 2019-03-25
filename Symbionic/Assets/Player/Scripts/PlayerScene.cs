using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScene : MonoBehaviour
{
    //Components
    public Animator ac;
    public GameObject radar;
    public Renderer track;
    public Collider boxCollider;
    private Rigidbody rb;
    private AudioSource aso;
    private AudioSource gaso;
    private AudioSource maso;
    private ConstantForce f;
    private Collider magnetMask;
    private MeshRenderer mMMeshRenderer;

    //Raycast Variables
    RaycastHit hit;
    float distToGround;


    void Start()
    {
        ac = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        aso = GetComponent<AudioSource>();
        gaso = transform.Find("SymbionicIdle").GetComponent<AudioSource>();
        maso = transform.Find("MagnetMask").GetComponent<AudioSource>();
        f = GetComponent<ConstantForce>();
        magnetMask = transform.Find("MagnetMask").GetComponent<SphereCollider>();
        mMMeshRenderer = magnetMask.GetComponent<MeshRenderer>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (SymApp.instance.status.running)
        {
            rb.drag = 1;
            if (rb.velocity.sqrMagnitude < SymApp.instance.status.sqrMaxVelocity)
            {
                rb.AddForce(transform.forward * SymApp.instance.status.runSpeed * Time.deltaTime, ForceMode.Acceleration);
            }
        }
    }

    public void StartRun()
    {
        track.material.SetFloat("_Speed2", 12f);
        ac.SetBool("running", true);
    }

    public void StopRun()
    {
        ac.SetBool("running", false);
        track.material.SetFloat("_Speed2", 0f);
    }

    public void Row(int s)
    {
        rb.drag = 4f;
        ac.SetBool("walking", true);
        track.material.SetFloat("_Speed2", 6f);
        rb.AddForce(0.5f * transform.forward * SymApp.instance.status.walkSpeed * Time.deltaTime, ForceMode.VelocityChange);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(s * Vector3.up * SymApp.instance.status.rotateSpeed * Time.deltaTime));
    }

    public void Walk()
    {
        rb.drag = 4f;
        ac.SetBool("walking", true);
        track.material.SetFloat("_Speed2", 6f);
        rb.AddForce(transform.forward * SymApp.instance.status.walkSpeed * Time.deltaTime, ForceMode.VelocityChange);
    }

    public void StopWalk()
    {
        ac.SetBool("walking", false);
        track.material.SetFloat("_Speed2", 0f);
        if (rb.velocity != new Vector3(0, 0, 0) && SymApp.instance.status.grounded)
        {
            rb.drag = 4f;
        }
    }

    public void PerfectJumpAntic()
    {
        aso.clip = SymApp.instance.status.pJumpClip;
        aso.Play();
    }

    public void Jump(bool super)
    {
        float height;

        if (super)
        {
            height = SymApp.instance.status.jumpSpeed * 2;
        }
        else
        {
            height = SymApp.instance.status.jumpSpeed;
        }

        if (SymApp.instance.status.grounded)
        {
            aso.clip = SymApp.instance.status.jumpClip;
            aso.Play();
            ac.SetTrigger("jump");
            rb.AddForce(Vector3.up * height, ForceMode.Impulse);
        }
    }

    public void Glide()
    {
        gaso.clip = SymApp.instance.status.glideClip;
        gaso.Play();
        ac.SetBool("glide", true);
        f.force = Vector3.up * 20f;
    }

    public void EndGlide()
    {
        gaso.Stop();
        ac.SetBool("glide", false);
        f.force = new Vector3(0, 0, 0);
    }

    public void Updraft()
    {
        ac.SetBool("glide", true);
        aso.clip = SymApp.instance.status.updraftClip;
        aso.Play();
        rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }

    public void Squeak()
    {
        aso.clip = SymApp.instance.status.squeakClip;
        aso.Play();
    }

    public void Dig()
    {
        aso.clip = SymApp.instance.status.digClip;
        aso.Play();

        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 2f, transform.forward, out hit, 2f))
        {
            if (hit.transform.tag == "diggable")
            {
                string code = hit.transform.gameObject.GetComponent<Interactable>().Interact(transform);
                string[] pToScreen = code.Split('-');
                SymApp.instance.manager.ui.SendCode(pToScreen[0], pToScreen[1]);
            }
        }
    }

    public void Nibble()
    {
        aso.clip = SymApp.instance.status.nibbleClip;
        aso.Play();

        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 4f, transform.forward, out hit, 4f))
        {
            if (hit.transform.tag == "bitable")
            {
                hit.transform.gameObject.GetComponent<Interactable>().Interact(transform);
            }
        }
    }

    public void Interact()
    {

        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 4f, transform.forward, out hit, 4f))
        {
            if (hit.transform.tag == "interactable")
            {
                hit.transform.gameObject.GetComponent<Interactable>().Interact(transform);
            }
        }
    }

    public void Magnet()
    {
        maso.clip = SymApp.instance.status.magnetClip;
        maso.Play();
        mMMeshRenderer.enabled = true;
    }

    public void MagnetPull()
    {
        foreach (Rigidbody magnetic in SymApp.instance.status.magnetics)
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

    public void MagnetEnd()
    {
        maso.Stop();
        mMMeshRenderer.enabled = false;
    }

    public void DiveBomb()
    {
        aso.clip = SymApp.instance.status.diveBombClip;
        aso.Play();
        f.force = -Vector3.up * 50f;
    }

    public Vector3 GroundCast()
    {
        
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 3f))
        {
            return hit.normal;
        }
        return Vector3.up;
        //Debug.DrawRay(transform.position, -Vector3.up * (distToGround + 10f), Color.black, Time.deltaTime);
    }

    public void RotateTowards(Quaternion sketch)
    {
        sketch = sketch * rb.rotation;
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, sketch, Time.deltaTime * 3f));
    }
}

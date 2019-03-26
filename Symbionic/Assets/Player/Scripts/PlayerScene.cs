using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScene : MonoBehaviour
{
    //Components
    public Animator ac;
    public Renderer track;
    public Collider boxCollider;
    private Rigidbody rb;
    private AudioSource aso;
    private AudioSource gaso;
    private AudioSource maso;
    private ConstantForce f;
    private MeshRenderer mMMeshRenderer;

    //Raycast Variables
    RaycastHit hit;
    float distToGround;

    //Coroutine containers
    Coroutine lastRadar;

    SymStatus status;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.001f);
        status = SymApp.Instance.status;
        ac = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        aso = GetComponent<AudioSource>();
        gaso = transform.Find("SymbionicIdle").GetComponent<AudioSource>();
        maso = transform.Find("MagnetMask").GetComponent<AudioSource>();
        f = GetComponent<ConstantForce>();
        mMMeshRenderer = transform.Find("MagnetMask").GetComponent<MeshRenderer>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (status.running)
        {
            rb.drag = 1;
            if (rb.velocity.sqrMagnitude < status.sqrMaxVelocity)
            {
                rb.AddForce(transform.forward * status.runSpeed * Time.deltaTime, ForceMode.Acceleration);
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
        rb.AddForce(0.5f * transform.forward * status.walkSpeed * Time.deltaTime, ForceMode.VelocityChange);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(s * Vector3.up * status.rotateSpeed * Time.deltaTime));
    }

    public void Walk()
    {
        rb.drag = 4f;
        ac.SetBool("walking", true);
        track.material.SetFloat("_Speed2", 6f);
        rb.AddForce(transform.forward * status.walkSpeed * Time.deltaTime, ForceMode.VelocityChange);
    }

    public void StopWalk()
    {
        ac.SetBool("walking", false);
        track.material.SetFloat("_Speed2", 0f);
        if (rb.velocity != new Vector3(0, 0, 0) && status.grounded)
        {
            rb.drag = 4f;
        }
    }

    public void PerfectJumpAntic()
    {
        aso.clip = status.pJumpClip;
        aso.Play();
    }

    public void Jump(bool super)
    {
        float height;

        if (super)
        {
            height = status.jumpSpeed * 2;
        }
        else
        {
            height = status.jumpSpeed;
        }

        if (status.grounded)
        {
            aso.clip = status.jumpClip;
            aso.Play();
            ac.SetTrigger("jump");
            rb.AddForce(Vector3.up * height, ForceMode.Impulse);
        }
    }

    public void Glide()
    {
        gaso.clip = status.glideClip;
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
        aso.clip = status.updraftClip;
        aso.Play();
        rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }

    public void Squeak()
    {
        aso.clip = status.squeakClip;
        aso.Play();
    }

    public void Dig()
    {
        aso.clip = status.digClip;
        aso.Play();

        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 2f, transform.forward, out hit, 2f))
        {
            if (hit.transform.tag == "diggable")
            {
                string code = hit.transform.gameObject.GetComponent<Interactable>().Interact(transform);
                string[] pToScreen = code.Split('-');
                SymApp.Instance.manager.ui.SendCode(pToScreen[0], pToScreen[1]);
            }
        }
    }

    public void Nibble()
    {
        aso.clip = status.nibbleClip;
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
        maso.clip = status.magnetClip;
        maso.Play();
        mMMeshRenderer.enabled = true;
    }

    public void MagnetPull()
    {
        Collider[] magnets = Physics.OverlapSphere(transform.position, 80f, LayerMask.GetMask("Magnetic"));
        Collider[] innerMagnets = Physics.OverlapSphere(transform.position, 5f, LayerMask.GetMask("Magnetic"));
        print("magnets: " + magnets.Length + " innerMagnets: " + innerMagnets.Length);
        foreach (Collider magnetic in magnets)
        {
            bool magnetFound = false;
            foreach(Collider innerMagnet in innerMagnets)
            {
                if (innerMagnet == magnetic)
                    magnetFound = true;
            }
            if(magnetFound)
            {
                magnetic.GetComponent<Rigidbody>().drag = 4f;
            }
            else
            {
                magnetic.GetComponent<Rigidbody>().drag = 1f;
                magnetic.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(magnetic.transform.position, rb.position, .8f / Mathf.Pow(Mathf.Log((magnetic.transform.position - rb.position).sqrMagnitude), 2)));
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
        aso.clip = status.diveBombClip;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "box")
        {
            status.boxes.Add(other.gameObject);
        }
        if (other.tag == "magnetic")
        {
            status.magnetics.Add(other.gameObject.GetComponent<Rigidbody>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "box")
        {
            status.boxes.Remove(other.gameObject);
        }
        if (other.tag == "magnetic")
        {
            status.magnetics.Remove(other.gameObject.GetComponent<Rigidbody>());
        }
    }

    public void Ping()
    {
        if (lastRadar != null)
            StopCoroutine(lastRadar);
        Collider[] boxesToPing = Physics.OverlapSphere(transform.position,200f,LayerMask.GetMask("Boxes"));
        print(boxesToPing.Length);
        lastRadar = StartCoroutine(PingSound(boxesToPing.Length));
    }

    private IEnumerator PingSound(float n)
    {
        yield return new WaitForSeconds(3f);
        for (int i = 0; i < n; i++)
        {
            aso.clip = status.radarClip;
            aso.Play();
            yield return new WaitForSeconds(3f);
        }
    }
}

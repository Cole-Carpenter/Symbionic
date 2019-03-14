using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tubes : Interactable {

    public List<Transform> Nodes;
    public Transform playerT;
    public Transform camPoint;
    public GameObject camRig;
    public Spline spline;

    public float speed = 1f;

    private UnityStandardAssets.Cameras.FreeLookCam camScript;
    private PlayerController playerScript;
    private Camera cam;

    private bool awake = false;
    private float realSpeed;
    private int nPoints;
    private float t = 0;
    private float distance = 0;
    private bool activated = false;


    // Use this for initialization
    void Start () {
        spline = GetComponentInChildren<Spline>();
        camScript = camRig.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>();
        cam = Camera.main;
        playerScript = playerT.GetComponent<PlayerController>();
        realSpeed = speed / 10;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Interact(collision.gameObject.transform);
        }
    }

    // Update is called once per frame
    void Update () {
        if(activated && !awake)
        {
            camScript.SetTarget(null);
            camScript.enabled = false;
            playerScript.enabled = false;
            awake = true;
        }
        if (activated)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, camPoint.position, 2f * Time.deltaTime);
            Vector3 relativePos = playerT.position - cam.transform.position;
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), 3f * Time.deltaTime);
            Vector3 newPos = spline.SplineMove(t, playerT);
            if(newPos == playerT.position)
            {
                print("here");
                activated = false;
            }
            else
            {
                playerT.position = newPos;
                playerT.LookAt(spline.GetNextPoint());
                t += realSpeed;
            }
        }
        if (!activated && awake)
        {
            cam.transform.localPosition = new Vector3(0, 0, -15);
            cam.transform.localRotation = Quaternion.identity;
            camScript.enabled = true;
            camScript.SetTarget(playerT);
            playerScript.enabled = true;
            awake = false;
        }

	}

    public override string Interact(Transform player)
    {
        activated = true;
        playerT = player;
        return"";
    }
}

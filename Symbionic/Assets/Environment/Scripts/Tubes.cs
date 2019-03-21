using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tubes : Interactable {


    private Transform playerT;
    private Rigidbody playerRB;
    public List<Transform> Nodes;
    public Transform camPoint;
    public GameObject camRig;
    public Spline spline;

    public float speed = 1f;

    private UnityStandardAssets.Cameras.FreeLookCam camScript;
    private UnityStandardAssets.Cameras.ProtectCameraFromWallClip camScript2;
    private PlayerController playerScript;
    private Camera cam;

    private bool awake = false;
    private float realSpeed;
    private int nPoints;
    private float distance = 0;
    private bool activated = false;
    private bool firstPoint;


    // Use this for initialization
    void Start () {
        spline = GetComponentInChildren<Spline>();
        camScript = camRig.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>();
        camScript2 = camRig.GetComponent<UnityStandardAssets.Cameras.ProtectCameraFromWallClip>();
        cam = Camera.main;
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
            camScript2.enabled = false;
            playerScript.enabled = false;
            playerRB.isKinematic = true;
            playerRB.constraints = RigidbodyConstraints.None;
            awake = true;
        }
        if(activated && !firstPoint)
        {
            playerRB.MovePosition(Vector3.Lerp(playerRB.position, spline.controlPoints[1].position , 3f * Time.deltaTime));
            if ((playerRB.position - spline.controlPoints[1].position).magnitude < 0.8f)
            {
                firstPoint = true;
            }
        }
        else if (activated && firstPoint)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, camPoint.position, 2f * Time.deltaTime);
            Vector3 relativePos = playerT.position - cam.transform.position;
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), 2f * Time.deltaTime);
            Vector3 newPos = spline.SplineMove(realSpeed, playerT);
            if(newPos == playerT.position)
            {
                activated = false;
            }
            else
            {
                playerT.LookAt(spline.GetNextPoint(), playerT.up);
                playerT.position = newPos;
            }
        }
        if (!activated && awake)
        {
            cam.transform.localPosition = new Vector3(0, 0, -15);
            cam.transform.localRotation = Quaternion.identity;
            camScript.enabled = true;
            camScript2.enabled = true;
            camScript.SetTarget(playerT);
            playerScript.enabled = true;
            playerRB.isKinematic = false;
            playerRB.constraints = RigidbodyConstraints.FreezeRotation;
            firstPoint = false;
            spline.ResetSpline();
            awake = false;
        }

	}

    public override string Interact(Transform player)
    {
        activated = true;
        playerT = player;
        playerScript = playerT.GetComponent<PlayerController>();
        playerRB = player.GetComponent<Rigidbody>();
        return"";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeScene : MonoBehaviour
{
    private UnityStandardAssets.Cameras.FreeLookCam camScript;
    private UnityStandardAssets.Cameras.ProtectCameraFromWallClip camScript2;
    private Camera cam;
    private Transform playerT;
    private Rigidbody playerRB;
    public List<Transform> Nodes;
    public Transform camPoint;
    public GameObject camRig;
    public Spline spline;
    public Transform warpPoint;

    private int nPoints;

    private TubeManager tubes = SymApp.instance.manager.tubes;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        spline = GetComponentInChildren<Spline>();
        camScript = camRig.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>();
        camScript2 = camRig.GetComponent<UnityStandardAssets.Cameras.ProtectCameraFromWallClip>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Activate(collision.gameObject.transform);
        }
    }

    public void Activate(Transform player)
    {
        camScript.SetTarget(null);
        camScript.enabled = false;
        camScript2.enabled = false;
        SymApp.instance.manager.playerManager.enabled = false;
        playerRB.isKinematic = true;
        playerRB.constraints = RigidbodyConstraints.None;
        tubes.activated = true;
        playerT = player;
        playerRB = player.GetComponent<Rigidbody>();
        tubes.SetTube(this);
    }

    public void MoveToFirstPoint()
    {
        playerRB.MovePosition(Vector3.Lerp(playerRB.position, spline.controlPoints[1].position, 3f * Time.deltaTime));
        if ((playerRB.position - spline.controlPoints[1].position).magnitude < 0.8f)
        {
            tubes.firstPoint = true;
        }
    }

    public void MoveAlongSpline()
    {
        cam.transform.position = Vector3.Lerp(cam.transform.position, camPoint.position, 2f * Time.deltaTime);
        Vector3 relativePos = playerT.position - cam.transform.position;
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), 2f * Time.deltaTime);
        Vector3 newPos = spline.SplineMove(tubes.realSpeed, playerT);
        if (newPos == Vector3.zero)
        {
            playerT.position = warpPoint.position;
            tubes.activated = false;
        }
        else
        {
            playerT.LookAt(spline.GetNextPoint(), playerT.up);
            playerT.position = newPos;
        }
    }

    public void Deactivate()
    {
        cam.transform.localPosition = new Vector3(0, 0, -15);
        cam.transform.localRotation = Quaternion.identity;
        camScript.enabled = true;
        camScript2.enabled = true;
        camScript.SetTarget(playerT);
        SymApp.instance.manager.playerManager.enabled = true;
        playerRB.isKinematic = false;
        playerRB.constraints = RigidbodyConstraints.FreezeRotation;
        spline.ResetSpline();
    }
}

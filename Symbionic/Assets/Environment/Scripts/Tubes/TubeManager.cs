using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeManager : Interactable {
    private bool awake = false;
    public float realSpeed;
    private float distance = 0;
    public bool activated = false;
    public bool firstPoint;

    private SymStatus status;
    private TubeScene tube;

    // Use this for initialization
    IEnumerator Start () {
        yield return new WaitForSeconds(0.001f);
        status = SymApp.Instance.status;
    }

    // Update is called once per frame
    void Update () {
        if(activated && !firstPoint)
        {
            tube.MoveToFirstPoint();
        }
        else if (activated && firstPoint)
        {
            tube.MoveAlongSpline();
        }
        if (!activated && awake)
        {
            tube.Deactivate();
            awake = false;
        }

	}

    public void SetTube(TubeScene t)
    {
        awake = true;
        tube = t;
        firstPoint = false;
    }

    
}

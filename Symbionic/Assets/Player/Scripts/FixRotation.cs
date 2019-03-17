using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRotation : MonoBehaviour {

	float distToGround;
	private Vector3 normal = Vector3.up;
    private Rigidbody rb;
	RaycastHit hit;
    Quaternion sketch;

	void Start(){
        rb = GetComponent<Rigidbody>();
		distToGround = GetComponent<Collider>().bounds.extents.y;
	}

	void Update()
	{
		if (Physics.Raycast(transform.position, -Vector3.up, out hit,  distToGround + 3f)){
			normal = hit.normal;
		}
        //Debug.DrawRay(transform.position, -Vector3.up * (distToGround + 10f), Color.black, Time.deltaTime);
        sketch = Quaternion.FromToRotation(transform.up, normal);
        sketch = sketch * rb.rotation;

        rb.MoveRotation(Quaternion.Lerp(rb.rotation, sketch, Time.deltaTime * 3f));  
    }

}

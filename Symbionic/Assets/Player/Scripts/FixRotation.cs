using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRotation : MonoBehaviour {

	float distToGround;
	private Vector3 normal = Vector3.up;
	private Vector3 lastNormal = Vector3.up;
	RaycastHit hit;

	void Start(){
		distToGround = GetComponent<Collider>().bounds.extents.y;
	}

	void Update()
	{
		if (Physics.Raycast(transform.position, -transform.up, out hit,  distToGround + 0.2f)){
			normal = hit.normal;
		}

		if(normal != lastNormal){
			transform.rotation = Quaternion.FromToRotation (transform.rotation.eulerAngles, normal);
		}
		
		lastNormal = normal;
    }

}

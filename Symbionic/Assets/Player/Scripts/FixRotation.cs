using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRotation : MonoBehaviour {

	float distToGround;
	private Vector3 normal = Vector3.up;
	private Vector3 lastNormal = Vector3.up;
	RaycastHit hit;
    Quaternion sketch;

	void Start(){
		distToGround = GetComponent<Collider>().bounds.extents.y;
	}

	void Update()
	{
		if (Physics.Raycast(transform.position, -Vector3.up, out hit,  distToGround + 10f)){
			normal = hit.normal;
		}
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, normal) * transform.rotation, Time.deltaTime * 3f);
    }

}

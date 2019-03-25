using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationManager : MonoBehaviour {

	//temp-variables
    Vector3 normal = Vector3.up;
    Quaternion sketch;

	void Update()
	{
        Vector3 normal = SymApp.instance.scene.player.GroundCast();
        sketch = Quaternion.FromToRotation(transform.up, normal);
        SymApp.instance.scene.player.RotateTowards(sketch);  
    }

}

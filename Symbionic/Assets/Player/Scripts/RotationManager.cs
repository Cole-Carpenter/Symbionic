using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationManager : MonoBehaviour {

	//temp-variables
    Vector3 normal = Vector3.up;
    Vector3 up;
    Quaternion sketch;

	void Update()
	{
        
        normal = SymApp.Instance.scene.player.GroundCast();
        up = SymApp.Instance.scene.player.transform.up;
        sketch = Quaternion.FromToRotation(up, normal);
        SymApp.Instance.scene.player.RotateTowards(sketch);  
        
    }

}

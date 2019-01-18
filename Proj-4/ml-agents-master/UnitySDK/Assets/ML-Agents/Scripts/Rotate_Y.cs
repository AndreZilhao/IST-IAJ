using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Y : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * Time.deltaTime *10);
    }
}

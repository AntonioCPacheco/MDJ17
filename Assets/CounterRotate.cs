﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterRotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.RotateAround(this.transform.parent.position, Vector3.forward, -this.transform.parent.rotation.eulerAngles.z);
	}
}

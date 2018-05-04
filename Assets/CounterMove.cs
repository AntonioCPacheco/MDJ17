using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterMove : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.localPosition = -this.transform.parent.localPosition;
	}
}

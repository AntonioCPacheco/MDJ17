using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Behavior : MonoBehaviour {

    public Transform[] switches;
    public Material OpenM;
    public Material ClosedM;
    bool changed = false;
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        bool open = true;
        foreach (Transform t in switches)
        {
            if (!t.GetComponent<Switch_Behaviour>().triggered)
            {
                open = false;
                break;
            }
        }
        if (!changed && open) { 
            this.GetComponent<Renderer>().material = open ? OpenM : ClosedM;
            changed = true;
        }
	}
}

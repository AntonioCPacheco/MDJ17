using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_Behaviour : MonoBehaviour {
    
    public bool needsLight = false;
    public bool triggered;

	// Use this for initialization
	void Start () {
        triggered = !needsLight;
	}
	
	// Update is called once per frame
	void Update () {
        //TODO door related stuff
		//TODO animation related
	}

    public void getTriggered(bool mode)
    {
        triggered = mode ? needsLight : !needsLight;
    }

    /*
    void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log("ola");
        //TODO resolve multiple lights (LIST)
        if(LayerMask.Equals(coll.gameObject.layer, LayerMask.NameToLayer("Light")))
        {
            triggered = needsLight;
            Debug.Log("New Collision Enter with:" + coll.gameObject.name + " ; variable triggered set to " + triggered);
        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if (LayerMask.Equals(coll.gameObject.layer, LayerMask.NameToLayer("Light")))
        {
            triggered = !needsLight;
            Debug.Log("New Collision Exit with:" + coll.gameObject.name + " ; variable triggered set to " + triggered);
        }
    }*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        //Debug.Log("New Collision Entsdaer with:" + coll.gameObject.name);
        if (LayerMask.Equals(coll.gameObject.layer, LayerMask.NameToLayer("Switches")))
        {
            coll.gameObject.GetComponent<Switch_Behaviour>().setTriggered(true);
            //Debug.Log("New Collision Enter with:" + coll.gameObject.name);
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (LayerMask.Equals(coll.gameObject.layer, LayerMask.NameToLayer("Switches")))
        {
            coll.gameObject.GetComponent<Switch_Behaviour>().setTriggered(false);
            //Debug.Log("New Collision Exit with:" + coll.gameObject.name);
        }
    }
}

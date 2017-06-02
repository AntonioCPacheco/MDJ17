using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_Behaviour : MonoBehaviour {
    
    public bool needsLight = false;
    public bool triggered = false;

    float lastTrigger;
    float timeBetweenTriggers = 0.5f;

	// Use this for initialization
	void Start () {
        lastTrigger = -timeBetweenTriggers;
        triggered = !needsLight;
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void setTriggered(bool mode)
    {
        if (Time.realtimeSinceStartup > lastTrigger + timeBetweenTriggers)
        {
            triggered = mode ? needsLight : !needsLight;
            this.GetComponent<Animator>().SetBool("triggered", triggered);
            lastTrigger = Time.realtimeSinceStartup;
        }
    }
}

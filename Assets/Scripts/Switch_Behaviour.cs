using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_Behaviour : MonoBehaviour
{
    List<GameObject> lights;
    float lastTime = 0f;
    public bool needsLight = false;
    bool triggered = false;

    // Use this for initialization
    void Start()
    {
        lights = new List<GameObject>();
        this.GetComponent<Animator>().SetBool("activated", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (lights.Count > 0)
        {
            triggered = true;
            this.GetComponent<Animator>().SetBool("activated", needsLight);
            lastTime = Time.timeSinceLevelLoad;
        }
        else
        {
            triggered = false;
            this.GetComponent<Animator>().SetBool("activated", !needsLight);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger && LayerMask.LayerToName(other.gameObject.layer).Equals("Light"))
        {
            if (!lights.Contains(other.gameObject)) lights.Add(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.isTrigger && LayerMask.LayerToName(other.gameObject.layer).Equals("Light"))
        {
            print(this.name + " !  " + other.name);
            if (lights.Contains(other.gameObject)) lights.Remove(other.gameObject);
        }
    }

    public bool isActivated()
    {
        return triggered ? needsLight : !needsLight;
    }
}

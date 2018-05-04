using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerDeath : MonoBehaviour {

    List<GameObject> lights;
    public float timeTillDead = 1.0f;
    float lastTime = 0f;

	// Use this for initialization
	void Start () {
        lights = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (lights.Count > 0 || Time.timeSinceLevelLoad < 2f)
        {
            lastTime = Time.timeSinceLevelLoad;
            return;
        }
        if (Time.timeSinceLevelLoad - lastTime >= timeTillDead)
        {
            GameObject.Find("UIManager").GetComponent<UIManager>().Reload();
            return;
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
            if (lights.Contains(other.gameObject)) lights.Remove(other.gameObject);
        }
    }
}

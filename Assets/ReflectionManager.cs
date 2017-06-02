using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionManager : MonoBehaviour {

    public GameObject child;
    public float creationDelay = 0.5f;
    float lastCreated;


	// Use this for initialization
	void Start () {
        lastCreated = -creationDelay;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void Triggered(GameObject go, Vector2 source, Vector2 point)
    {

        string auxName = go.name + "-Mirror";

        foreach (Transform t in this.transform.GetComponentsInChildren<Transform>())
        {
            if (t.name.Equals(go.name)) return;
        }
        //Debug.Log(go.name + "-Mirror");
        if (this.transform.FindChild(auxName) == null)
        {
            if (Time.realtimeSinceStartup - lastCreated >= creationDelay)
            {
                lastCreated = Time.realtimeSinceStartup;
                //Debug.Log(go.name);
                GameObject c = Instantiate(child, this.transform);
                //Debug.Log(go.name);
                c.name = auxName;
                GameObject g = go;
                c.GetComponent<Mirror_Behaviour>().setSource(g);
            }
            else
            {
                return;
            }
        }
        this.transform.FindChild(auxName).GetComponent<Mirror_Behaviour>().Triggered(source, point);
        //Debug.Log(" -----   " + go.name);
    }

    public bool isLocked()
    {
        foreach(Mirror_Behaviour m in this.transform.GetComponentsInChildren<Mirror_Behaviour>())
        {
            if (m.isLocked()) return true;
        }
        return false;
    }
}

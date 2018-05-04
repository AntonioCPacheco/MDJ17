using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reflect : MonoBehaviour {
    public GameObject sourcePrefab;
    Dictionary<GameObject, List<Vector2[]>> lightQueue;

    // Use this for initialization
    void Start () {
        lightQueue = new Dictionary<GameObject, List<Vector2[]>>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        foreach(GameObject go in lightQueue.Keys)
        {
            List<Vector2[]> list;
            lightQueue.TryGetValue(go, out list);
            foreach (Vector2[] vector in list)
            {
                go.GetComponent<ProcessLightArea>().ReceiveLight(vector[0], vector[1], vector[2]);
            }
        }
        lightQueue.Clear();
    }

    
    public void ReceiveLight(Vector2 incoming, Vector2 collisionPoint, Vector2 surfaceNormal, GameObject source)
    {
        if (source != this.gameObject)
        {
            GameObject newSource;
            string sourceName = source.name + source.GetInstanceID();
            if (this.transform.parent.FindChild(sourceName) == null)
            {
                newSource = Instantiate(sourcePrefab, this.transform.parent);
                newSource.name = sourceName;
                newSource.GetComponent<ProcessLightArea>().lightSource = source;
            }
            else
            {
                newSource = this.transform.parent.FindChild(sourceName).gameObject;
            }
            if (!lightQueue.Keys.Contains(newSource)) lightQueue.Add(newSource, new List<Vector2[]>());
            Vector2[] arguments = new Vector2[] { incoming, collisionPoint, surfaceNormal };
            List<Vector2[]> list;
            lightQueue.TryGetValue(newSource, out list);
            list.Add(arguments);
            lightQueue[newSource] = list;
        }
    }

    public ProcessLightArea getCorrespondingSource(GameObject lightSource)
    {
        string sourceName = lightSource.name + lightSource.GetInstanceID();
        Transform child = this.transform.parent.FindChild(sourceName);
        if (child != null)
        {
            return child.GetComponent<ProcessLightArea>();
        }
        return null;
    }

    public void clearDictionaries()
    {
        ProcessLightArea[] pla = this.transform.parent.GetComponentsInChildren<ProcessLightArea>();
        foreach (ProcessLightArea p in pla)
        {
            p.clearDictionaries();
            p.clearReceivers();
        }
    }
}

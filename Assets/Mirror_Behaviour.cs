using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Mirror_Behaviour : MonoBehaviour {
    

    GameObject source;
    public bool triggered;
    private float epsilon = 1E-5f;
    bool started = false;
    public float updateDelay = 0.1f;
    float lastChanged;

    bool locked = false;

    public float maxDistance = 30.0f;

    public float auxMaxX, auxMaxY, auxMinX, auxMinY;

    public LayerMask layerMask;
    public LayerMask outterLimits;

    Dictionary<Vector2, Vector2> containingAreaDict;
    PolygonCollider2D containingArea;
    bool changed = false;
    public bool tChanged = false;
    List<Vector2> insideCorners;

    Dictionary<Vector2, Vector2> lightArea;
    Vector2 mirrorPosition;

    // Use this for initialization
    void Start () {
        if (!started)
        {
            started = true;
            //Debug.Log(name + " started");
            containingArea = this.transform.FindChild("AuxCollider").GetComponent<PolygonCollider2D>();
            insideCorners = new List<Vector2>();
            lightArea = new Dictionary<Vector2, Vector2>();
            containingAreaDict = new Dictionary<Vector2, Vector2>();
            lastChanged = -updateDelay;
        }
    }

    void Awake()
    {
        if (!started)
        {
            Start();
            started = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
        locked = true;
        try
        {
            mirrorPosition = this.transform.position;

            if (Time.realtimeSinceStartup - lastChanged >= updateDelay)
            {
                if (changed)
                {
                    //Debug.Log("changed");
                    changed = false;
                    processContainingArea();
                }

               //castToInsideCorners();
                processLightArea();
            }

            if (tChanged)
            {
                locked = false;
                Debug.Log("AAAH");
                Destroy(this.gameObject);


                lastChanged = Time.realtimeSinceStartup;
                containingAreaDict.Clear();
                lightArea.Clear();
                insideCorners.Clear();
                //this.transform.FindChild("LightArea").GetComponent<PolygonCollider2D>().SetPath(0, new Vector2[0]);
                //this.transform.FindChild("AuxCollider").GetComponent<PolygonCollider2D>().SetPath(0, new Vector2[0]);
                tChanged = false;
            }
        }
        catch (Exception e)
        {
            locked = false;
            Debug.Log(e.Message);
        }
        locked = false;
    }

    //Falta acrescentar alguma maneira de distinguir entre fontes de luz 
    
    public void Triggered(Vector2 source, Vector2 point)
    {
        locked = true;
        if (!lightArea.Keys.Contains(point - mirrorPosition) && !containingAreaDict.Keys.Contains(point - mirrorPosition))
        {
            Vector2 normal = this.transform.parent.up;

            Vector2 outgoing = Vector2.Reflect(point - source, normal);

            GameObject g = this.gameObject;
            RaycastHit2D hit2d = Physics2D.Raycast(point, outgoing, maxDistance, layerMask);
            if (hit2d.collider != null)
            {
                addToDict(lightArea, (point - mirrorPosition), (hit2d.point - mirrorPosition));

                if (hit2d.collider.GetComponent<ReflectionManager>() != null)
                {
                    hit2d.collider.GetComponent<ReflectionManager>().Triggered(g, point, hit2d.point);
                }
            }
            RaycastHit2D hit2dOut = Physics2D.Raycast(point, outgoing, maxDistance, outterLimits);
            if (hit2dOut.collider != null)
            {
                if (addToDict(containingAreaDict, (point - mirrorPosition), (hit2dOut.point - mirrorPosition)))
                {
                    changed = true;
                }
            }
        }
        locked = false;
    }

    void processLightArea()
    {
        Vector2[] lightAreaOrdered;
        sortDict(lightArea, out lightAreaOrdered);
        this.transform.FindChild("LightArea").GetComponent<PolygonCollider2D>().SetPath(0, lightAreaOrdered);
    }

    void processContainingArea() //Orders the containing area, and gets all the corners that are inside it
    {
        if (containingAreaDict.Count >= 2)
        {
            getCorners();
            Vector2[] containingAreaOrdered;
            sortDict(containingAreaDict, out containingAreaOrdered);
            containingArea.SetPath(0, containingAreaOrdered);

            insideCorners.Clear();
            Transform w = GameObject.Find("Walls").transform;
            for (int i = 0; i < w.childCount; i++)
            {
                Transform child = w.GetChild(i);
                PolygonCollider2D p = child.GetComponent<PolygonCollider2D>();
                if (child.name.StartsWith("Wall"))
                {
                    addToInsideCorners(p);
                }
                else if (child.name.StartsWith("Mirror") && !child.position.Equals(this.transform.position))
                {
                    addToInsideCorners(p);
                    p = child.FindChild("Mirror-OutterEdges").GetComponent<PolygonCollider2D>();
                    addToInsideCorners(p);
                }
            }
        }
    }

    //Casts 3 rays to every corner inside the containing area
    void castToInsideCorners()
    {
        if (insideCorners.Count > 0)
        {
            try
            {
                foreach (Vector2 v in insideCorners)
                {
                    Vector2 origin;
                    origin = /*this.transform.parent.position + this.transform.parent.right + (this.transform.parent.up * 0.1f);//*/calculateOrigin(v, this.source.transform.position) + (Vector2)(this.transform.parent.up*0.1f);
                    //Debug.Log(origin);

                    RaycastHit2D hitMain = Physics2D.Raycast(origin, v - origin, maxDistance, layerMask);
                    RaycastHit2D hitLeft = Physics2D.Raycast(origin, (Vector2)(Quaternion.AngleAxis(0.001f, Vector3.forward) * v) - origin, maxDistance, layerMask);
                    RaycastHit2D hitRight = Physics2D.Raycast(origin, (Vector2)(Quaternion.AngleAxis(-0.001f, Vector3.forward) * v) - origin, maxDistance, layerMask);

                    if (hitMain.collider.name.Equals(this.transform.parent.name)) Debug.Log("Self colliding");
                    processRaycastHit(hitMain, origin);
                    processRaycastHit(hitLeft, origin + (Vector2)(this.transform.parent.right * epsilon));
                    processRaycastHit(hitRight, origin - (Vector2)(this.transform.parent.right * epsilon));
                }
            }
            catch (InvalidOperationException ioe)
            {
                Debug.Log(ioe.Message);
            }
        }
    }

    Vector2 calculateOrigin(Vector2 v1, Vector2 v2)
    {

        v1 = this.transform.parent.InverseTransformPoint(v1);
        v2 = this.transform.parent.InverseTransformPoint(v2);

        float d1 = Mathf.Abs(v1.y);
        float d2 = Mathf.Abs(v2.y);

        float diffX = v1.x - v2.x;
        int s = (v1.x < v2.x) ? -1 : 1;
        diffX = Mathf.Abs(diffX);

        float c1 = diffX * (d1 / (d2+d1));
        
        float res = (v1.x - (s * c1));
        //Debug.Log(res);
        return this.transform.parent.TransformPoint(new Vector2(res, 0));// + this.transform.parent.position;
    }

    void getCorners()
    {
        float maxX = float.MinValue, maxY = maxX;
        float minX = float.MaxValue, minY = minX;
        foreach (Vector2 a in containingAreaDict.Values)
        {
            Vector2 v = a + mirrorPosition;
            maxX = (v.x > maxX) ? v.x : maxX;
            maxY = (v.y > maxY) ? v.y : maxY;
            minX = (v.x < minX) ? v.x : minX;
            minY = (v.y < minY) ? v.y : minY;
        }

        if (floatEquals(minX, maxX) || floatEquals(minY, maxY)) return;
        Vector2 toAdd;
        if (floatEquals(minX, auxMinX))
        {
            if(floatEquals(minY, auxMinY))
            {
                toAdd = new Vector2(minX, minY);
                Vector2 key = calculateOrigin(this.source.transform.position, toAdd);
                key = this.transform.TransformPoint(key);
                key -= mirrorPosition;
                addToDict(containingAreaDict, key, toAdd - mirrorPosition);
            }
            if (floatEquals(maxY, auxMaxY))
            {
                toAdd = new Vector2(minX, maxY);
                Vector2 key = calculateOrigin(this.source.transform.position, toAdd);
                key = this.transform.TransformPoint(key);
                key -= mirrorPosition;
                addToDict(containingAreaDict, key, toAdd - mirrorPosition);
            }
        }
        if (floatEquals(maxX, auxMaxX))
        {
            if (floatEquals(minY, auxMinY))
            {
                toAdd = new Vector2(maxX, minY);
                Vector2 key = calculateOrigin(this.source.transform.position, toAdd);
                key = this.transform.TransformPoint(key);
                key -= mirrorPosition;
                addToDict(containingAreaDict, key, toAdd - mirrorPosition);
            }
            if (floatEquals(maxY, auxMaxY))
            {
                toAdd = new Vector2(maxX, maxY);
                Vector2 key = calculateOrigin(this.source.transform.position, toAdd);
                key = this.transform.TransformPoint(key);
                key -= mirrorPosition;
                addToDict(containingAreaDict, key, toAdd - mirrorPosition);
            }
        }
    }

    void processRaycastHit(RaycastHit2D r, Vector2 origin)
    {
        Vector2 auxPos = this.transform.parent.position;
        GameObject g = this.gameObject;
        if (r.collider != null && !lightArea.ContainsKey(origin))
        {
            lightArea.Add(origin, r.point);
            
            if (r.transform.GetComponent<ReflectionManager>() != null)
            {
                //Debug.Log(r.transform.name);
                r.transform.GetComponent<ReflectionManager>().Triggered(g, origin, r.point);
            }
        }
    }

    void addToInsideCorners(PolygonCollider2D p)
    {
        Vector2 toAdd;
        for (int j = 0; j < p.points.Length; j++)
        {
            toAdd = p.transform.TransformPoint(p.points[j]);
            if (containingArea.OverlapPoint(toAdd))
            {
                //Debug.Log(toAdd);
                addToList(insideCorners, toAdd);
                //Debug.Log(toAdd);
            }
        }
    }

    void printArray(Vector2[] array)
    {
        for(int i=0; i < array.Length; i++)
        {
            Debug.Log(array[i]);
        }
    }

    void printList(List<Vector2> list)
    {
        foreach(Vector2 v in list)
        {
            Debug.Log(name + ", " +v);
        }
    }

    bool addToDict(Dictionary<Vector2, Vector2> d, Vector2 key, Vector2 value)
    {
        if (!d.ContainsKey(key))
        {
            d.Add(key, value);
            return true;
        }
        return false;
    }

    bool addToList(List<Vector2> l, Vector2 v)
    {
        if (!l.Contains(v))
        {
            l.Add(v);
            return true;
        }
        return false;
    }
    ////////////////////////////////////////
    ////////////SORTING/////////////////////
    ////////////////////////////////////////

    void sortDict(Dictionary<Vector2, Vector2> dict, out Vector2[] output)
    {
        output = new Vector2[dict.Count*2];
        List<Vector2> keysOrdered = dict.Keys.OrderByDescending(v => ((Vector2)this.transform.parent.InverseTransformPoint(v)).sqrMagnitude).ToList();
        int index = 0;
        foreach (Vector2 k in keysOrdered)
        {
            Vector2 aux = new Vector2();
            dict.TryGetValue(k, out aux);
            output[index++] = aux;
        }
        keysOrdered = dict.Keys.OrderBy(v => ((Vector2)this.transform.parent.InverseTransformPoint(v)).sqrMagnitude).ToList();
        foreach (Vector2 k in keysOrdered)
        {
            output[index++] = k;
        }
    }

    bool floatEquals(float f1, float f2)
    {
        return Mathf.Abs(f1 - f2) < epsilon;
    }

    void OnDrawGizmos()
    {
        if (lightArea != null && lightArea.Count > 0)
            foreach (Vector2 v in lightArea.Values.Union(lightArea.Keys))
                Gizmos.DrawSphere(v + (Vector2)this.gameObject.transform.position, .1f);
        Gizmos.DrawSphere(this.gameObject.transform.position, .2f);
    }

    public void setSource(GameObject tsource)
    {
        this.source = tsource;
    }

    public bool isLocked()
    {
        return locked;
    }
}
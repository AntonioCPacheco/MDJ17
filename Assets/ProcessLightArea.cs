using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProcessLightArea : MonoBehaviour {

    Dictionary<Vector2, Vector2> reflectedLightRays;
    Dictionary<Vector2, Vector2> containingArea;
    Dictionary<Vector2, Vector2> lightArea;

    PolygonCollider2D containingAreaCollider;
    PolygonCollider2D lightAreaCollider;

    List<Reflect> receivers;

    Vector2 sourcePosition;
    public GameObject lightSource;
    Transform mirrorTransform;

    public LayerMask outterLimits;
    public LayerMask sceneObjects;
    float maxDistance = 100.0f;

    private float epsilon = 1E-5f;
    public float auxMaxX, auxMaxY, auxMinX, auxMinY;

    bool started = false;

    // Use this for initialization
    void Start()
    {
        if (!started)
        {
            reflectedLightRays = new Dictionary<Vector2, Vector2>();
            containingArea = new Dictionary<Vector2, Vector2>();
            lightArea = new Dictionary<Vector2, Vector2>();

            containingAreaCollider = this.transform.FindChild("ContainingArea").GetComponent<PolygonCollider2D>();
            lightAreaCollider = this.transform.FindChild("LightArea").GetComponent<PolygonCollider2D>();

            receivers = new List<Reflect>();

            mirrorTransform = this.transform.parent.FindChild("Mirror").transform;
            if (lightSource != null)
                sourcePosition = lightSource.transform.position;

            calculateOutterLimits();
            started = true;
        }
    }

    void Awake()
    {
        if (!started)
        {
            Start();
        }
    }

    // Update is called once per frame
    void Update()
    {
        mirrorTransform = this.transform.parent.FindChild("Mirror").transform;
        processReflections();
        getMissingCorners();
        processLightArea();
    }

    //Get the point in the mirror where v2 reflects to v1
    Vector2 calculateOrigin(Vector2 v1, Vector2 v2)
    {
        v1 = mirrorTransform.InverseTransformPoint(v1);
        v2 = mirrorTransform.InverseTransformPoint(v2);

        float d1 = Mathf.Abs(v1.y);
        float d2 = Mathf.Abs(v2.y);

        float diffX = v1.x - v2.x;
        int s = (v1.x < v2.x) ? -1 : 1;
        diffX = Mathf.Abs(diffX);

        float c1 = diffX * (d1 / (d2 + d1));

        float res = (v1.x - (s * c1));

        return mirrorTransform.position + mirrorTransform.TransformDirection(new Vector2(res, 0));
    }

    //Get outter limits of the map
    void calculateOutterLimits()
    {
        auxMaxX = float.MinValue;
        auxMaxY = auxMaxX;
        auxMinX = float.MaxValue;
        auxMinY = auxMinX;
        foreach (Vector2 v in GameObject.Find("Outter Limits").GetComponent<EdgeCollider2D>().points)
        {
            auxMaxX = (v.x > auxMaxX) ? v.x : auxMaxX;
            auxMaxY = (v.y > auxMaxY) ? v.y : auxMaxY;
            auxMinX = (v.x < auxMinX) ? v.x : auxMinX;
            auxMinY = (v.y < auxMinY) ? v.y : auxMinY;
        }
    }

    //Get corners of the map that aren't being considered
    void getMissingCorners()
    {
        float maxX = float.MinValue, maxY = maxX;
        float minX = float.MaxValue, minY = minX;
        foreach (Vector2 v in containingArea.Values)
        {
            maxX = (v.x > maxX) ? v.x : maxX;
            maxY = (v.y > maxY) ? v.y : maxY;
            minX = (v.x < minX) ? v.x : minX;
            minY = (v.y < minY) ? v.y : minY;
        }

        //print(maxX + ", " + minX + ", " + maxY + ", " + minY);

        if (floatEquals(minX, maxX) || floatEquals(minY, maxY)) return;
        Vector2 toAdd;
        if (floatEquals(minX, auxMinX))
        {
            if (floatEquals(minY, auxMinY))
            {
                toAdd = new Vector2(minX, minY);
                Vector2 key = calculateOrigin(sourcePosition, toAdd);
                addToDictionary(containingArea, key, toAdd);
            }
            if (floatEquals(maxY, auxMaxY))
            {
                toAdd = new Vector2(minX, maxY);
                Vector2 key = calculateOrigin(sourcePosition, toAdd);
                addToDictionary(containingArea, key, toAdd);
            }
        }
        if (floatEquals(maxX, auxMaxX))
        {
            if (floatEquals(minY, auxMinY))
            {
                toAdd = new Vector2(maxX, minY);
                Vector2 key = calculateOrigin(sourcePosition, toAdd);
                addToDictionary(containingArea, key, toAdd);
            }
            if (floatEquals(maxY, auxMaxY))
            {
                toAdd = new Vector2(maxX, maxY);
                Vector2 key = calculateOrigin(sourcePosition, toAdd);
                addToDictionary(containingArea, key, toAdd);
            }
        }
    }

    //Raycast reflected rays to outter limits and walls
    void processReflections()
    {
        foreach (Vector2 k in reflectedLightRays.Keys)
        {
            Vector2 dir;
            reflectedLightRays.TryGetValue(k, out dir);

            ///////////////////////
            RaycastHit2D hit2D = Physics2D.Raycast(k, dir, maxDistance, outterLimits);
            if (containingArea.Keys.Contains(k))
            {
                containingArea.Remove(k);
            }
            containingArea.Add(k, hit2D.point);
            ///////////////////////
            RaycastHit2D hit2DObj = Physics2D.Raycast(k, dir, maxDistance, sceneObjects);

            if(hit2DObj.collider.GetComponent<Reflect>() != null && hit2DObj.collider.gameObject != mirrorTransform.gameObject)
            {
                Reflect receiver = hit2DObj.collider.GetComponent<Reflect>();
                receiver.ReceiveLight(dir, hit2DObj.point, hit2DObj.normal, mirrorTransform.gameObject);
                if (!receivers.Contains(receiver)) receivers.Add(receiver);
            }

            if (lightArea.Keys.Contains(k))
            {
                lightArea.Remove(k);
            }
            lightArea.Add(k, hit2DObj.point);
        }
        Vector2[] auxContainingAreaPoints;
        sortDict(containingArea, out auxContainingAreaPoints);
        containingAreaCollider.SetPath(0, auxContainingAreaPoints);
    }

    //Cast to inside corners and save in light area
    void processLightArea()
    {
        List<Vector2> insidePoints = getInsidePoints();
        foreach (Vector2 v in insidePoints)
        {
            Vector2 origin = calculateOrigin(v, sourcePosition);
            Vector2 toCastMain = v - origin;
            Vector2 toCastLeft = Quaternion.AngleAxis(0.001f, Vector3.forward) * toCastMain;
            Vector2 toCastRight = Quaternion.AngleAxis(-0.001f, Vector3.forward) * toCastMain;

            RaycastHit2D hit2DMain = Physics2D.Raycast(origin, toCastMain, maxDistance, sceneObjects);
            RaycastHit2D hit2DLeft = Physics2D.Raycast(origin, toCastLeft, maxDistance, sceneObjects);
            RaycastHit2D hit2DRight = Physics2D.Raycast(origin, toCastRight, maxDistance, sceneObjects);

            Vector2 originLeft = calculateOrigin(hit2DLeft.point, sourcePosition);
            Vector2 originRight = calculateOrigin(hit2DRight.point, sourcePosition);

            //Casting rays on mirrors
            if (hit2DMain.collider != null && hit2DMain.collider.GetComponent<Reflect>() != null)
            {
                hit2DMain.collider.GetComponent<Reflect>().ReceiveLight(toCastMain, hit2DMain.point, hit2DMain.normal, mirrorTransform.gameObject);
            }
            if (hit2DLeft.collider != null && hit2DLeft.collider.GetComponent<Reflect>() != null)
            {
                hit2DLeft.collider.GetComponent<Reflect>().ReceiveLight(toCastLeft, hit2DLeft.point, hit2DLeft.normal, mirrorTransform.gameObject);
            }
            if (hit2DRight.collider != null && hit2DRight.collider.GetComponent<Reflect>() != null)
            {
                hit2DRight.collider.GetComponent<Reflect>().ReceiveLight(toCastRight, hit2DRight.point, hit2DRight.normal, mirrorTransform.gameObject);
            }

            addToDictionary(lightArea, origin, hit2DMain.point);
            addToDictionary(lightArea, originLeft, hit2DLeft.point);
            addToDictionary(lightArea, originRight, hit2DRight.point);
        }
        Vector2[] auxLightAreaPoints;
        sortDict(lightArea, out auxLightAreaPoints);
        lightAreaCollider.SetPath(0, auxLightAreaPoints);
    }

    //Adds the (key,value) pair to the dictionary if there isn't one, and replaces it otherwise
    void addToDictionary(Dictionary<Vector2, Vector2> dic, Vector2 key, Vector2 value)
    {
        if (dic.Keys.Contains(key))
        {
            dic.Remove(key);
        }
        dic.Add(key, value);

    }

    //Returns all the corners in the scene that are inside the containing area 
    List<Vector2> getInsidePoints()
    {
        List<Vector2> output = new List<Vector2>();
        PolygonCollider2D[] pcs2D = GameObject.Find("Objects").GetComponentsInChildren<PolygonCollider2D>();
        foreach (PolygonCollider2D pc in pcs2D)
        {
            if (pc.transform.CompareTag("sceneObject") && pc.gameObject != mirrorTransform.gameObject)
            {
                foreach (Vector2 v in pc.points)
                {
                    Vector2 toCompare = (Vector2)(Quaternion.AngleAxis(pc.transform.rotation.eulerAngles.z, Vector3.forward) * v) + (Vector2)pc.transform.position;
                    if (containingAreaCollider.OverlapPoint(toCompare))
                    {
                        output.Add(toCompare);
                    }
                }
            }
        }
        return output;
    }

    //Sorts the given dictionary by distance to the left-most point of the mirror
    void sortDict(Dictionary<Vector2, Vector2> dict, out Vector2[] output)
    {
        output = new Vector2[dict.Count * 2];
        List<Vector2> keysOrdered = dict.Keys.OrderByDescending(v => v.sqrMagnitude).ToList();
        int index = 0;
        foreach (Vector2 k in keysOrdered)
        {
            Vector2 aux = new Vector2();
            dict.TryGetValue(k, out aux);
            output[index++] = aux;
        }
        keysOrdered = dict.Keys.OrderBy(v => v.sqrMagnitude).ToList();
        foreach (Vector2 k in keysOrdered)
        {
            output[index++] = k;
        }
    }

    //Called by other light sources. Receives new ray of light to be reflected
    public void ReceiveLight(Vector2 incoming, Vector2 collisionPoint, Vector2 surfaceNormal)
    {
        if (surfaceNormal != (Vector2)mirrorTransform.up)
        {
            return;
        }
        Vector2 outgoing = Vector2.Reflect(incoming, surfaceNormal);

        if (reflectedLightRays.Keys.Contains(collisionPoint))
        {
            reflectedLightRays.Remove(collisionPoint);
        }
        reflectedLightRays.Add(collisionPoint, outgoing);
    }

    //Clears all receivers
    public void clearReceivers()
    {
        sourcePosition = lightSource.transform.position;
        foreach (Reflect receiver in receivers)
        {
            ProcessLightArea pla = receiver.getCorrespondingSource(mirrorTransform.gameObject);
            
            pla.clearDictionaries();
            pla.clearReceivers();
        }
        
    }

    //Clears all dictionaries
    public void clearDictionaries()
    {
        sourcePosition = lightSource.transform.position;
        containingArea.Clear();
        reflectedLightRays.Clear();
        lightArea.Clear();
    }

    //Prints a given dictionary
    void printDictionary(Dictionary<Vector2, Vector2> dic)
    {
        foreach (Vector2 k in dic.Keys)
        {
            Vector2 v;
            dic.TryGetValue(k, out v);

            print(k + " -> " + v);
        }
    }

    //...
    void OnDrawGizmos()
    {
        if (lightArea != null && lightArea.Keys.Count > 0)
            foreach (Vector2 v in lightArea.Keys.Union(lightArea.Values))
                Gizmos.DrawSphere(v, .1f);
        Gizmos.DrawSphere(this.gameObject.transform.position, .2f);
    }

    //Compares equals with a margin of epsilon (1E-5f)
    bool floatEquals(float f1, float f2)
    {
        return Mathf.Abs(f1 - f2) < epsilon;
    }

}

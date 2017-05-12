using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]

public class LightRenderer : MonoBehaviour {
    
    //Update delay
    public float delay = 0.5f;
    //What Raycast can hit
    public LayerMask layerMask;

    public Vector2[] vectors;

    PolygonCollider2D pc2;
    Dictionary<KeyValuePair<float, Vector2>, Vector2> corners;
    PolygonCollider2D[] pcs;
    float lastUpdated = 0.0f;

	// Use this for initialization
	void Start () {
        corners = new Dictionary<KeyValuePair<float, Vector2>, Vector2>();
        pc2 = GetComponent<PolygonCollider2D>();
        Transform w = GameObject.Find("Walls").transform;
        List<PolygonCollider2D> v = new List<PolygonCollider2D>();
        for(int i=0; i<w.childCount; i++)
        {
            if (w.GetChild(i).name.StartsWith("Mirror") || w.GetChild(i).name.StartsWith("Wall")) {
                v.Add(w.GetChild(i).GetComponent<PolygonCollider2D>());
                if(w.GetChild(i).name.StartsWith("Mirror")) v.Add(w.GetChild(i).FindChild("Mirror-OutterEdges").GetComponent<PolygonCollider2D>());
            }
        }
        pcs = v.ToArray();
    }
	
	// Update is called once per frame
	void Update () {
        //TODO Change the Dictionary to a List and order on insertion instead of ordering at the end
        if (lastUpdated + delay < Time.realtimeSinceStartup)
        {
            Vector2 lightPosition = this.gameObject.transform.position;
            corners.Clear();
            foreach (PolygonCollider2D pc in pcs)
            {
                for (int i = 0; i < pc.points.Length; i++)
                {
                    Vector2 toCast = (Vector2)(Quaternion.AngleAxis(pc.transform.localRotation.eulerAngles.z, Vector3.forward) * pc.points[i]) - lightPosition + (Vector2)pc.transform.position;

                    RaycastHit2D hitMain = Physics2D.Raycast(lightPosition, toCast, 20, layerMask);
                    RaycastHit2D hitLeft = Physics2D.Raycast(lightPosition, Quaternion.AngleAxis(0.001f, Vector3.forward) * toCast, 20.0f, layerMask);
                    RaycastHit2D hitRight = Physics2D.Raycast(lightPosition, Quaternion.AngleAxis(-0.001f, Vector3.forward) * toCast, 20.0f, layerMask);

                    KeyValuePair<float, Vector2> hitKey = new KeyValuePair<float, Vector2>(realAngle(lightPosition, hitMain.point), hitMain.point);
                    if (hitMain.collider != null && !corners.ContainsKey(hitKey))
                    {
                        corners.Add(hitKey, hitMain.point - lightPosition);
                        if (hitMain.collider.GetComponent<Mirror_Behaviour>() != null)
                        {
                            hitMain.collider.GetComponent<Mirror_Behaviour>().Triggered(hitMain.point - lightPosition, hitMain.point);
                        }
                    }

                    hitKey = new KeyValuePair<float, Vector2>(realAngle(lightPosition, hitLeft.point), hitLeft.point);
                    if (hitLeft.collider != null && !corners.ContainsKey(hitKey))
                    {
                        corners.Add(hitKey, hitLeft.point - lightPosition);
                        if (hitLeft.collider.GetComponent<Mirror_Behaviour>() != null)
                        {
                            hitLeft.collider.GetComponent<Mirror_Behaviour>().Triggered(hitLeft.point - lightPosition, hitLeft.point);
                        }
                    }

                    hitKey = new KeyValuePair<float, Vector2>(realAngle(lightPosition, hitRight.point), hitRight.point);
                    if (hitRight.collider != null && !corners.ContainsKey(hitKey)) {
                        corners.Add(hitKey, hitRight.point - lightPosition);
                        if (hitRight.collider.GetComponent<Mirror_Behaviour>() != null)
                        {
                            hitRight.collider.GetComponent<Mirror_Behaviour>().Triggered(hitRight.point - lightPosition, hitRight.point);
                        }
                    }
                }
            }
            KeyValuePair<float, Vector2>[] keys = new KeyValuePair<float, Vector2>[corners.Keys.Count];
            int index = 0;
            foreach(KeyValuePair<float, Vector2> k in corners.Keys)
            {
                keys[index++] = k;
            }
            QuicksortAngles(keys, 0, keys.Length-1);
            SortOrder(keys);
            vectors = new Vector2[corners.Values.Count];
            for(int i=0; i < corners.Values.Count; i++)
            {
                corners.TryGetValue(keys[i], out vectors[i]);
            }
            pc2.SetPath(0, vectors);
        }

    }

    void addToList()
    {

    }

    float realAngle(Vector2 from, Vector2 to)
    {
        return Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg;
    }

    void OnDrawGizmos()
    {
        if(corners!=null)
        foreach(Vector2 v in corners.Values)
            Gizmos.DrawSphere(v + (Vector2)this.gameObject.transform.position, .1f);
        Gizmos.DrawSphere(this.gameObject.transform.position, .2f);
    }


    public void QuicksortAngles(KeyValuePair<float, Vector2>[] elements, int left, int right)
    {
        int i = left, j = right;
        KeyValuePair<float, Vector2> pivot = elements[(left + right) / 2];

        while (i <= j){
            while (elements[i].Key < pivot.Key){
                i++;
            }
            while (elements[j].Key > pivot.Key)
            {
                j--;
            }
            if (i <= j){
                // Swap
                KeyValuePair<float, Vector2> tmp = elements[i];
                elements[i] = elements[j];
                elements[j] = tmp;

                i++;
                j--;
            }
        }
        // Recursive calls
        if (left < j)
            QuicksortAngles(elements, left, j);

        if (i < right)
            QuicksortAngles(elements, i, right);
    }
    
    void SortOrder(KeyValuePair<float, Vector2>[] values)
    {
        bool changed = true;
        while (changed)
        {
            changed = false;
            for (int i = 0; i < values.Length - 1; i++)
            {
                if (values[i].Key == values[i + 1].Key && (values[i].Value - (Vector2)transform.position).sqrMagnitude > (values[i + 1].Value - (Vector2)transform.position).sqrMagnitude)
                {
                    changed = true;
                    KeyValuePair<float, Vector2> aux = values[i];
                    values[i] = values[i + 1];
                    values[i + 1] = aux;
                }
            }
        }
    }

    List<Vector2> toList()
    {
        List<Vector2> res = new List<Vector2>();
        for(int i=0; i<pc2.points.Length; i++)
        {
            res.Add(pc2.points[i]);
        }
        return res;
    }
    /*
    //Returns 1 if first comes before second in clockwise order.
    //Returns -1 if second comes before first.
    //Returns 0 if the points are identical.
    public static int IsClockwise(Vector2 first, Vector2 second, Vector2 origin)
    {
        if (first == second)
            return 0;

        Vector2 firstOffset = first - origin;
        Vector2 secondOffset = second - origin;
        float angle1 = Mathf.Atan2(firstOffset.x, firstOffset.y);
        float angle2 = Mathf.Atan2(secondOffset.x, secondOffset.y);

        if (angle1 < angle2)
            return -1;
        if (angle1 > angle2)
            return 1;

        // Check to see which point is closest
        return (firstOffset.sqrMagnitude < secondOffset.sqrMagnitude) ? -1 : 1;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        Debug.Log("New Collision Entsdaer with:" + coll.gameObject.name);
        if (LayerMask.Equals(coll.gameObject.layer, LayerMask.NameToLayer("Switches")))
        {
            coll.gameObject.GetComponent<Switch_Behaviour>().setTriggered(true);
            Debug.Log("New Collision Enter with:" + coll.gameObject.name);
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (LayerMask.Equals(coll.gameObject.layer, LayerMask.NameToLayer("Switches")))
        {
            coll.gameObject.GetComponent<Switch_Behaviour>().setTriggered(false);
            Debug.Log("New Collision Exit with:" + coll.gameObject.name);
        }
    }*/
}

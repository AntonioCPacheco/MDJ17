using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mirror_Behaviour : MonoBehaviour {

    public bool triggered;

    public LayerMask layerMask;
    public LayerMask outterLimits;

    List<Vector2> containingAreaList;
    PolygonCollider2D containingArea;
    bool changed = false;
    bool tChanged = false;
    List<Vector2> insideCorners;

    List<Vector2> lightArea;
    Vector2 mirrorPosition;

    // Use this for initialization
    void Start () {
        containingArea = this.transform.FindChild("AuxCollider").GetComponent<PolygonCollider2D>();
        insideCorners = new List<Vector2>();
        lightArea = new List<Vector2>();
        containingAreaList = new List<Vector2>();
    }
	
	// Update is called once per frame
	void Update () {

        if (changed)
        {
            Vector2[] auxV = containingAreaList.ToArray();
            QuicksortAngles(auxV, 0, auxV.Length-1);
            SortOrder(auxV);
            containingArea.points = auxV;
            changed = false;
        }
        mirrorPosition = this.transform.position;
        castToInsideCorners();

        if (lightArea.Count > 0)
        {
            Vector2[] vecs = new Vector2[lightArea.Count];
            int index = 0;
            foreach (Vector2 k in lightArea)
            {
                vecs[index++] = k;
            }
            QuicksortAngles(vecs, 0, index);
            SortOrder(vecs);
            this.transform.FindChild("MirrorLight").GetComponent<PolygonCollider2D>().SetPath(0, vecs);
        }
        if (this.GetComponent<Rigidbody2D>().velocity != Vector2.zero) tChanged = true;
        /*
            if (corners.Count != containingArea.points.Length) {
                //corners = new Vector2[containingArea.points.Length];
                Transform w = GameObject.Find("Walls").transform;
                for (int i = 0; i < w.childCount; i++)
                {
                    Transform child = w.GetChild(i);
                    if (child.name.StartsWith("Wall"))
                    {
                        Vector2[] p = child.GetComponent<PolygonCollider2D>().points;
                        for (int j = 0; j < p.Length; j++)
                        {
                            if (containingArea.bounds.Contains(p[j])) corners.Add(p[j]);
                        }
                    }
                    else if(child.name.StartsWith("Mirror") && !child.Equals(this))
                    {
                        Vector2[] p = child.GetComponent<PolygonCollider2D>().points;
                        for (int j = 0; j < p.Length; j++)
                        {
                            if (containingArea.bounds.Contains(p[j])) corners.Add(p[j]);
                        }
                        p = child.FindChild("Mirror-OutterEdges").GetComponent<PolygonCollider2D>().points;
                        for (int j = 0; j < p.Length; j++)
                        {
                            if (containingArea.bounds.Contains(p[j])) corners.Add(p[j]);
                        }
                    }
                }
            }
            
            foreach(Vector2 c in corners)
            {
                RaycastHit2D hitMain = Physics2D.Raycast(mirrorPosition, c, 20, layerMask);
                RaycastHit2D hitLeft = Physics2D.Raycast(mirrorPosition, Quaternion.AngleAxis(0.001f, Vector3.forward) * c, 20.0f, layerMask);
                RaycastHit2D hitRight = Physics2D.Raycast(mirrorPosition, Quaternion.AngleAxis(-0.001f, Vector3.forward) * c, 20.0f, layerMask);
                    
                if (hitMain.collider != null && !lightArea.Contains(hitMain.point))
                {
                    lightArea.Add(hitMain.point - mirrorPosition);
                    if (hitMain.collider.GetComponent<Mirror_Behaviour>() != null)
                    {
                        hitMain.collider.GetComponent<Mirror_Behaviour>().Triggered(hitMain.point - mirrorPosition, hitMain.point);
                    }
                }
                    
                if (hitLeft.collider != null && !lightArea.Contains(hitLeft.point))
                {
                    lightArea.Add(hitLeft.point - mirrorPosition);
                    if (hitLeft.collider.GetComponent<Mirror_Behaviour>() != null)
                    {
                        hitLeft.collider.GetComponent<Mirror_Behaviour>().Triggered(hitLeft.point - mirrorPosition, hitLeft.point);
                    }
                }
                    
                if (hitRight.collider != null && !lightArea.Contains(hitRight.point))
                {
                    lightArea.Add(hitRight.point - mirrorPosition);
                    if (hitRight.collider.GetComponent<Mirror_Behaviour>() != null)
                    {
                        hitRight.collider.GetComponent<Mirror_Behaviour>().Triggered(hitRight.point - mirrorPosition, hitRight.point);
                    }
                }
                
            }
            //QuicksortAngles(corners,);

        }


        /*
        if (!lastPosition.Equals(this.transform.position) || !lastRotation.Equals(this.transform.rotation))
        {
            points.Clear();
            lastPosition = this.transform.position;
            lastRotation = this.transform.rotation;
        }
        if (points.Keys.Count > 0)
        {
            KeyValuePair<float, Vector2>[] keys = new KeyValuePair<float, Vector2>[points.Keys.Count];
            int index = 0;
            foreach (KeyValuePair<float, Vector2> f in points.Keys)
            {
                keys[index++] = f;
            }
            QuicksortAngles(keys, 0, keys.Length - 1);
            SortOrder(keys);
            vectors = new Vector2[points.Values.Count];
            for (int i = 0; i < points.Values.Count; i++)
            {
                points.TryGetValue(keys[i], out vectors[i]);
            }
            this.transform.FindChild("Light-Mirror").GetComponent<PolygonCollider2D>().points = vectors;
        }*/
    }
    
    public void Triggered(Vector2 incoming, Vector2 point)
    {
        if (tChanged) containingAreaList.Clear();

        Vector2 normal = this.transform.up;
        Vector2 outgoing = Vector2.Reflect(incoming, normal);
        RaycastHit2D hit2d = Physics2D.Raycast(point, outgoing, 20.0f, outterLimits);
        if(hit2d.collider != null)
        {
            /*
            if (!lightArea.Contains(hit2d.point)) lightArea.Add(hit2d.point);
            if (!lightArea.Contains(point)) lightArea.Add(point);
            */
            if (!containingAreaList.Contains(hit2d.point)) {
                containingAreaList.Add(hit2d.point);
                changed = true;
            }
            if (!containingAreaList.Contains(point)) {
                containingAreaList.Add(point);
                changed = true;
            }
            if (changed)
            { //BBUBBBBBBBBBBBBBBBBBBBBBBBBBBBUUUUUUUUUUUUUUUUUUUUUUUUUUUUGGGGGGGGGGGGGGGGGGG

                insideCorners.Clear();
                if (containingAreaList.Count > 2)
                {

                    Vector2[] vecs = new Vector2[containingAreaList.Count];
                    int index = 0;
                    foreach (Vector2 k in containingAreaList)
                    {
                        vecs[index++] = k;
                    }
                    QuicksortAngles(vecs, 0, index);
                    SortOrder(vecs);
                    containingArea.SetPath(0, vecs);


                    Transform w = GameObject.Find("Walls").transform;
                    Vector2 toAdd;
                    for (int i = 0; i < w.childCount; i++)
                    {
                        Transform child = w.GetChild(i);
                        PolygonCollider2D p = child.GetComponent<PolygonCollider2D>();
                        if (child.name.StartsWith("Wall"))
                        {
                            for (int j = 0; j < p.points.Length; j++)
                            {
                                toAdd = (Vector2)(Quaternion.AngleAxis(p.transform.localRotation.eulerAngles.z, Vector3.forward) * p.points[j]) - mirrorPosition + (Vector2)p.transform.position;
                                if (containingArea.bounds.Contains(toAdd) && !insideCorners.Contains(toAdd)) insideCorners.Add(toAdd);
                            }
                        }
                        else if (child.name.StartsWith("Mirror") && !child.Equals(this.transform))
                        {
                            for (int j = 0; j < p.points.Length; j++)
                            {
                                toAdd = (Vector2)(Quaternion.AngleAxis(p.transform.localRotation.eulerAngles.z, Vector3.forward) * p.points[j]) - mirrorPosition + (Vector2)p.transform.position;
                                if (containingArea.bounds.Contains(toAdd) && !insideCorners.Contains(toAdd)) insideCorners.Add(toAdd);
                            }
                            p = child.FindChild("Mirror-OutterEdges").GetComponent<PolygonCollider2D>();
                            for (int j = 0; j < p.points.Length; j++)
                            {
                                toAdd = (Vector2)(Quaternion.AngleAxis(p.transform.localRotation.eulerAngles.z, Vector3.forward) * p.points[j]) - mirrorPosition + (Vector2)p.transform.position;
                                if (containingArea.bounds.Contains(toAdd) && !insideCorners.Contains(toAdd)) insideCorners.Add(toAdd);
                            }
                        }
                    }
                }
            }
            /*
                       KeyValuePair<float, Vector2> hitKey = new KeyValuePair<float, Vector2>(realAngle((Vector2)transform.position, hit2d.point), hit2d.point);
                       if(!points.ContainsKey(hitKey) && !points.ContainsValue(hit2d.point - (Vector2)transform.position)) points.Add(hitKey, hit2d.point - (Vector2)transform.position);
                       hitKey = new KeyValuePair<float, Vector2>(realAngle((Vector2)transform.position, point), point);
                       if (!points.ContainsKey(hitKey) && !points.ContainsValue(point - (Vector2)transform.position)) points.Add(hitKey, point - (Vector2)transform.position);*/
        }
        changed = false;
    }

    //Casts 3 rays to every corner inside the containing area
    void castToInsideCorners()
    {
        if (insideCorners.Count > 0)
        {
            foreach (Vector2 v in insideCorners)
            {
                RaycastHit2D hitMain = Physics2D.Raycast(mirrorPosition, v, 20, layerMask);
                RaycastHit2D hitLeft = Physics2D.Raycast(mirrorPosition, Quaternion.AngleAxis(0.001f, Vector3.forward) * v, 20.0f, layerMask);
                RaycastHit2D hitRight = Physics2D.Raycast(mirrorPosition, Quaternion.AngleAxis(-0.001f, Vector3.forward) * v, 20.0f, layerMask);

                if (hitMain.collider != null && !lightArea.Contains(hitMain.point))
                {
                    lightArea.Add(hitMain.point - mirrorPosition);
                    if (hitMain.collider.GetComponent<Mirror_Behaviour>() != null)
                    {
                        hitMain.collider.GetComponent<Mirror_Behaviour>().Triggered(hitMain.point - mirrorPosition, hitMain.point);
                    }
                }

                if (hitLeft.collider != null && !lightArea.Contains(hitMain.point))
                {
                    lightArea.Add(hitLeft.point - mirrorPosition);
                    if (hitLeft.collider.GetComponent<Mirror_Behaviour>() != null)
                    {
                        hitLeft.collider.GetComponent<Mirror_Behaviour>().Triggered(hitLeft.point - mirrorPosition, hitLeft.point);
                    }
                }

                if (hitRight.collider != null && !lightArea.Contains(hitMain.point))
                {
                    lightArea.Add(hitRight.point - mirrorPosition);
                    if (hitRight.collider.GetComponent<Mirror_Behaviour>() != null)
                    {
                        hitRight.collider.GetComponent<Mirror_Behaviour>().Triggered(hitRight.point - mirrorPosition, hitRight.point);
                    }
                }
            }
        }
    }

    //Calculates the real angle between 2 vectors
    float realAngle(Vector2 from, Vector2 to)
    {
        return Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg;
    }

    //Sorts given vector in counterclockwise order
    public void QuicksortAngles(Vector2[] elements, int left, int right)
    {
        int i = left, j = right;
        float pivot = realAngle(transform.position, elements[(left + right) / 2]);

        while (i <= j)
        {
            while (realAngle(transform.position, elements[i]) < pivot)
            {
                i++;
            }
            while (realAngle(transform.position, elements[j]) > pivot)
            {
                j--;
            }
            if (i <= j)
            {
                // Swap
                Vector2 tmp = elements[i];
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


    void SortOrder(Vector2[] values)
    {
        bool changed = true;
        while (changed)
        {
            changed = false;
            for (int i = 0; i < values.Length - 1; i++)
            {
                if (realAngle(transform.position, values[i]) == realAngle(transform.position, values[i + 1])
                    && (values[i] - (Vector2)transform.position).sqrMagnitude > (values[i + 1] - (Vector2)transform.position).sqrMagnitude)
                {
                    changed = true;
                    Vector2 aux = values[i];
                    values[i] = values[i + 1];
                    values[i + 1] = aux;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (lightArea != null && lightArea.Count > 0)
            foreach (Vector2 v in lightArea)
                Gizmos.DrawSphere(v + (Vector2)this.gameObject.transform.position, .1f);
        Gizmos.DrawSphere(this.gameObject.transform.position, .2f);
    }
}

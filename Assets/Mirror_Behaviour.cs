using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror_Behaviour : MonoBehaviour {

    public bool triggered;
    Dictionary<KeyValuePair<float, float>, Vector2> points;
    Vector2[] vectors;
    List<Vector2> corners;

    // Use this for initialization
    void Start () {
        points = new Dictionary<KeyValuePair<float, float>, Vector2>();
        corners = new List<Vector2>();
	}
	
	// Update is called once per frame
	void Update () {
        if (points.Keys.Count > 0)
        {
            KeyValuePair<float, float>[] keys = new KeyValuePair<float, float>[points.Keys.Count];
            int index = 0;
            foreach (KeyValuePair<float, float> f in points.Keys)
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
            this.transform.FindChild("MirrorLight").GetComponent<PolygonCollider2D>().points = vectors;
        }
    }
    
    public void Triggered(Vector2 incoming, Vector2 point, Vector2 normal)
    {
        Vector2 outgoing = Vector2.Reflect(incoming, normal);
        RaycastHit2D hit2d = Physics2D.Raycast(point, outgoing);
        if(hit2d.collider != null)
        {
            KeyValuePair<float, float> hitKey = new KeyValuePair<float, float>(realAngle((Vector2)transform.position, hit2d.point), (hit2d.point - (Vector2)transform.position).sqrMagnitude);
            if(!points.ContainsKey(hitKey) && !points.ContainsValue(hit2d.point - (Vector2)transform.position)) points.Add(hitKey, hit2d.point - (Vector2)transform.position);
            hitKey = new KeyValuePair<float, float>(realAngle((Vector2)transform.position, point), (hit2d.point - (Vector2)transform.position).sqrMagnitude);
            if (!points.ContainsKey(hitKey) && !points.ContainsValue(point - (Vector2)transform.position)) points.Add(hitKey, point - (Vector2)transform.position);
        }
    }

    void processCorner(Vector2 corner)
    {

    }

    float realAngle(Vector2 from, Vector2 to)
    {
        return Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg;
    }

    void SortOrder(KeyValuePair<float, float>[] values)
    {
        bool changed = true;
        while (changed)
        {
            changed = false;
            for (int i = 0; i < values.Length - 1; i++)
            {
                if (values[i].Key == values[i + 1].Key && values[i].Value > values[i + 1].Value)
                {
                    changed = true;
                    KeyValuePair<float, float> aux = values[i];
                    values[i] = values[i + 1];
                    values[i + 1] = aux;
                }
            }
        }
    }

    public void QuicksortAngles(KeyValuePair<float, float>[] elements, int left, int right)
    {
        int i = left, j = right;
        KeyValuePair<float, float> pivot = elements[(left + right) / 2];

        while (i <= j)
        {
            while (elements[i].Key < pivot.Key)
            {
                i++;
            }
            while (elements[j].Key > pivot.Key)
            {
                j--;
            }
            if (i <= j)
            {
                // Swap
                KeyValuePair<float, float> tmp = elements[i];
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

    void OnDrawGizmos()
    {
        if (vectors != null)
            foreach (Vector2 v in vectors)
                Gizmos.DrawSphere(v + (Vector2)this.gameObject.transform.position, .1f);
        Gizmos.DrawSphere(this.gameObject.transform.position, .2f);
    }
}

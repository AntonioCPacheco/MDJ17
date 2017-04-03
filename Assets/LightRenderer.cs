using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]

public class LightRenderer : MonoBehaviour {

    //Vectors that limit the Raycast
    public Vector2 openingOne;
    public Vector2 openingTwo;
    //Maximum iterations for the raycast
    public int maxFractions = 9;
    //Update delay
    public float delay = 0.5f;
    //What Raycast can hit
    public LayerMask layerMask;

    PolygonCollider2D pc2;
    List<Vector2> corners;
    float lastUpdated = 0.0f;

	// Use this for initialization
	void Start () {
        //If opening vectors not manually initialized, set them to the default value
        if (openingOne == Vector2.zero) openingOne = new Vector2(1, -1).normalized;
        if (openingTwo == Vector2.zero) openingTwo = new Vector2(1, 1).normalized;
        corners = new List<Vector2>();
        pc2 = GetComponent<PolygonCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (lastUpdated + delay < Time.realtimeSinceStartup)
        {
            //Delay next update
            lastUpdated = Time.realtimeSinceStartup;
            //Reset list
            corners = new List<Vector2>();
            corners.Add(Vector2.zero);
            //Raycast between opening vectors
            for (int fraction = 0; fraction <= maxFractions; fraction++) {
                //Interpolate between opening vectors
                Vector3 toCast = Vector3.Lerp(openingOne, openingTwo, (float)fraction / maxFractions);
                //Raycast for the chosen vector
                RaycastHit2D hit = Physics2D.Raycast(this.gameObject.transform.position, toCast, 20.0f, layerMask);
                //Add to list
                corners.Add(hit.point - (Vector2)this.gameObject.transform.position);
            }
            //Convert to array and change the collider's points
            pc2.SetPath(0, corners.ToArray());
        }
    }

    /*
    public void Quicksort(Vector2[] elements, int left, int right)
    {
        int i = left, j = right;
        Vector2 pivot = elements[(left + right) / 2];

        while (i <= j){
            while (IsClockwise(elements[i], pivot, (Vector2)this.gameObject.transform.position) < 0){
                i++;
            }
            while (IsClockwise(elements[j], pivot, (Vector2)this.gameObject.transform.position) > 0){
                j--;
            }
            if (i <= j){
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
            Quicksort(elements, left, j);

        if (i < right)
            Quicksort(elements, i, right);
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
    }*/

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (LayerMask.Equals(coll.gameObject.layer, LayerMask.NameToLayer("Switches")))
        {
            coll.gameObject.GetComponent<Switch_Behaviour>().getTriggered(true);
            Debug.Log("New Collision Enter with:" + coll.gameObject.name);
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (LayerMask.Equals(coll.gameObject.layer, LayerMask.NameToLayer("Switches")))
        {
            coll.gameObject.GetComponent<Switch_Behaviour>().getTriggered(false);
            Debug.Log("New Collision Exit with:" + coll.gameObject.name);
        }
    }
}

using UnityEngine;
using System.Collections;

public class mirrorMove : MonoBehaviour
{

    public float defaultMass;
    public float imovableMass;
    public bool beingPushed;
    float xPos;

    public Vector3 lastPos;
    public Vector3 newPos;
    public int mode;
    public int colliding;
    // Use this for initialization
    void Start()
    {
        newPos = transform.position;
        xPos = transform.position.x;
        lastPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (mode == 0)
        {

        }
        else if (mode == 1)
        {

            if (beingPushed == false)
            {
                GetComponent<Rigidbody2D>().mass = imovableMass;
            }
            else
            {
                //this.GetComponent<Mirror_Behaviour>().tChanged = true;
                GetComponent<Rigidbody2D>().mass = defaultMass;
                //	GetComponent<Rigidbody2D> ().isKinematic = false;
            }
            //this.GetComponent<Mirror_Behaviour>().tChanged = true;

        }
        else
        {
            this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    void Update()
    {
        if (lastPos != this.transform.position)
        {
            foreach(Mirror_Behaviour m in this.transform.GetComponentsInChildren<Mirror_Behaviour>())
                m.tChanged = true;
        }
        lastPos = this.transform.position;
    }
}
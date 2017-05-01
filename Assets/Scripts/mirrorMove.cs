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
            this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        }
        else if (mode == 1)
        {
            this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;

            if (beingPushed == false)
            {


                GetComponent<Rigidbody2D>().mass = imovableMass;

            }
            else
            {
                GetComponent<Rigidbody2D>().mass = defaultMass;
                //	GetComponent<Rigidbody2D> ().isKinematic = false;
            }

        }
        else
        {
            this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }


}
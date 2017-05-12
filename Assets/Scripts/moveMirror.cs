using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class moveMirror : MonoBehaviour
{
    public KeyCode grabKey = KeyCode.E;
    public float distance = 1f;
    public float maxSpeed = 10f;
    public LayerMask MirrorLayer;
    public bool rotating;
    public bool pushing;
    public float RotateSpeed = 5f;
    private float Radius;
    private Vector2 direction = Vector2.right;

    private Vector2 _centre;
    private float _angle;
    GameObject mirror;
    // Use this for initialization
    void Start()
    {
        Physics2D.queriesStartInColliders = false;
        //GameObject.Find("Text").GetComponent<UnityEngine.UI.Text>().text = "Move: WASD or Arrow Keys\nGrab / Rotate: " + grabKey;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction * this.transform.localScale.x, distance, MirrorLayer);

        float move1 = Input.GetAxis("Horizontal");
        float move2 = Input.GetAxis("Vertical");
        if (hit.collider != null &&  Input.GetKeyDown(grabKey) && !rotating && !pushing)
        {
            mirror = hit.collider.gameObject;
            mirror.GetComponent<FixedJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
            mirror.GetComponent<FixedJoint2D>().enabled = true;
            mirror.GetComponent<mirrorMove>().beingPushed = true;
            _centre = mirror.transform.position;
            Radius = Vector2.Distance(this.transform.position, mirror.transform.position);
            pushing = true;
            this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            this.GetComponent<Rigidbody2D>().angularVelocity = 0f;
            mirror.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            mirror.GetComponent<Rigidbody2D>().angularVelocity = 0f;

        }
        else if(Input.GetKeyDown(grabKey) && pushing)
        {
            pushing = false;
            rotating = true;

            this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            this.GetComponent<Rigidbody2D>().angularVelocity = 0f;
            mirror.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            mirror.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        }
        else if (Input.GetKeyDown(grabKey) && rotating)
        {
            mirror.GetComponent<FixedJoint2D>().enabled = false;
            mirror.GetComponent<mirrorMove>().beingPushed = false;
            mirror.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            mirror.GetComponent<Rigidbody2D>().angularVelocity = 0f;
            rotating = false;
        }

        else if (!rotating && !pushing)
        {
            float speed = (move1 != 0 && move2 != 0) ? Mathf.Sqrt((maxSpeed * maxSpeed)/2) : maxSpeed;
            GetComponent<Rigidbody2D>().velocity = new Vector2(move1 * speed, move2 * speed);
            if (move1 < 0 && move2 < 0)
            {
                direction = Vector2.left + Vector2.down;
            }
            else if (move1 > 0 && move2 < 0)
            {
                direction = Vector2.right + Vector2.down;
            }
            else if (move1 < 0 && move2 > 0)
            {
                direction = Vector2.left + Vector2.up;
            }
            else if (move1 > 0 && move2 > 0)
            {
                direction = Vector2.right + Vector2.up;
            }
            else if (move2 < 0)
            {
                direction = Vector2.down;
            }
            else if(move2 > 0)
            {
                direction = Vector2.up;
            }
            else if(move1 < 0)
            {
                direction = Vector2.left;
            }
            else if(move1 > 0)
            {
                direction = Vector2.right;
            }
            

        }
        else if (pushing)
        {
            float speed = (move1 != 0 && move2 != 0) ? Mathf.Sqrt((maxSpeed * maxSpeed) / 2) : maxSpeed;
            speed *= .5f;
            GetComponent<Rigidbody2D>().velocity = new Vector2(move1 * speed, move2 * speed);
            direction = mirror.transform.position - transform.position;
            mirror.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;

        }
        else if (rotating)
        {
            _angle = RotateSpeed * Time.deltaTime * move1/15;
            _centre = mirror.transform.position;
            transform.position = RotatePointAroundPivot(transform.position, _centre, Quaternion.Euler(0, 0, _angle));
            //transform.rotation = Quaternion
            direction = _centre - (Vector2)transform.position;
            mirror.GetComponent<Rigidbody2D>().rotation += _angle*500/5;
            /*Vector3 vectorToTarget = mirror.transform.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, Time.deltaTime);*/
        }
        
        if (Mathf.Abs(move1) > Mathf.Abs(move2))
        {
            move2 = 0;
        }
        else
        {
            move1 = 0;
        }
        this.GetComponent<Animator>().SetFloat("hVelocity", move1);
        this.GetComponent<Animator>().SetFloat("vVelocity", move2);
        
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
    {
        return angle * (point - pivot) + pivot;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(transform.position, (Vector2)transform.position + direction * transform.localScale.x * distance);
        Gizmos.DrawSphere((Vector2)this.transform.position + (direction * this.transform.localScale.x), .1f);


    }
   
}
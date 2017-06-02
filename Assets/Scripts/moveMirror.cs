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
    public GameObject mirrorMoveArrows;
    public GameObject mirrorRotateArrows;

    // Use this for initialization
    void Start()
    {
        Physics2D.queriesStartInColliders = false;
        //GameObject.Find("Text").GetComponent<UnityEngine.UI.Text>().text = "Move: WASD or Arrow Keys\nGrab / Rotate: " + grabKey;

        mirrorMoveArrows.SetActive(false);
        mirrorRotateArrows.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction * this.transform.localScale.x, distance, MirrorLayer);

        float move1 = Input.GetAxis("Horizontal");
        float move2 = Input.GetAxis("Vertical");
        if (hit.collider != null && Input.GetKeyDown(grabKey) && !rotating && !pushing)
        {
            mirror = hit.collider.gameObject;
            if (mirror != null && mirror.GetComponent<ReflectionManager>().isLocked()) return;
            mirror.GetComponent<FixedJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
            mirror.GetComponent<FixedJoint2D>().enabled = true;
            mirror.GetComponent<mirrorMove>().beingPushed = true;
            //mirror.GetComponent<ReflectionManager>().tChanged = true;
            _centre = mirror.transform.position;
            Radius = Vector2.Distance(this.transform.position, mirror.transform.position);
            pushing = true;
            this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            this.GetComponent<Rigidbody2D>().angularVelocity = 0f;
            mirror.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            mirror.GetComponent<Rigidbody2D>().angularVelocity = 0f;

        }
        else if (Input.GetKeyDown(grabKey) && pushing)
        {
            if (mirror != null && mirror.GetComponent<ReflectionManager>().isLocked()) return;
            pushing = false;
            rotating = true;
            //mirror.GetComponent<Mirror_Behaviour>().tChanged = true;

            this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            this.GetComponent<Rigidbody2D>().angularVelocity = 0f;
            mirror.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            mirror.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        }
        else if (Input.GetKeyDown(grabKey) && rotating)
        {
            if (mirror != null && mirror.GetComponent<ReflectionManager>().isLocked()) return;
            mirror.GetComponent<FixedJoint2D>().enabled = false;
            mirror.GetComponent<mirrorMove>().beingPushed = false;
            mirror.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            mirror.GetComponent<Rigidbody2D>().angularVelocity = 0f;
            rotating = false;
        }

        else if (!rotating && !pushing)
        {

            if (mirror != null && mirror.GetComponent<ReflectionManager>().isLocked()) return;
            mirrorRotateArrows.SetActive(false);
            float speed = (move1 != 0 && move2 != 0) ? Mathf.Sqrt((maxSpeed * maxSpeed) / 2) : maxSpeed;
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
            else if (move2 > 0)
            {
                direction = Vector2.up;
            }
            else if (move1 < 0)
            {
                direction = Vector2.left;
            }
            else if (move1 > 0)
            {
                direction = Vector2.right;
            }


        }
        else if (pushing)
        {

            if (mirror != null && mirror.GetComponent<ReflectionManager>().isLocked()) return;
            if (move1 != 0 && move2 != 0)
            {
                foreach (Mirror_Behaviour m in mirror.transform.GetComponentsInChildren<Mirror_Behaviour>())
                    m.tChanged = true;
            }

            float speed = (move1 != 0 || move2 != 0) ? Mathf.Sqrt((maxSpeed * maxSpeed) / 2) : maxSpeed;
            speed *= .5f;
            GetComponent<Rigidbody2D>().velocity = new Vector2(move1 * speed, move2 * speed);
            direction = mirror.transform.position - transform.position;
            mirror.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;
            mirrorMoveArrows.SetActive(true);
            mirrorMoveArrows.transform.position = mirror.transform.position + mirror.transform.right;
        }
        else if (rotating)
        {
            if (mirror != null && mirror.GetComponent<ReflectionManager>().isLocked()) return;
            mirrorMoveArrows.SetActive(false);

            if (move1 != 0 || move2 != 0)
            {
                foreach (Mirror_Behaviour m in mirror.transform.GetComponentsInChildren<Mirror_Behaviour>())
                    m.tChanged = true;
            }

            _angle = RotateSpeed * Time.deltaTime * move1 / 3;
            _centre = mirror.transform.position + mirror.transform.right;

            transform.position = RotatePointAroundPivot(transform.position, _centre, Quaternion.Euler(0, 0, -_angle * 10));
            //transform.rotation = Quaternion
            direction = _centre - (Vector2)transform.position;
            mirror.transform.RotateAround(_centre, Vector3.forward, -_angle * 10);
            //mirror.GetComponent<Rigidbody2D>().rotation += _angle * 10;
            mirrorRotateArrows.SetActive(true);
            mirrorRotateArrows.transform.position = mirror.transform.position + mirror.transform.right;
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
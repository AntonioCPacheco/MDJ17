using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMovement : MonoBehaviour
{

    Vector2 lastPosition;
    Quaternion lastRotation;

    // Use this for initialization
    void Start()
    {
        lastPosition = this.transform.position;
        lastRotation = this.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!lastPosition.Equals((Vector2)this.transform.position) || !lastRotation.Equals(this.transform.rotation))
        {
            this.GetComponent<Reflect>().clearDictionaries();
            ProcessLightArea[] pla = this.transform.parent.GetComponentsInChildren<ProcessLightArea>();
            foreach (ProcessLightArea p in pla)
            {
                if(p.lightSource.GetComponent<Reflect>() != null)
                    p.lightSource.GetComponent<Reflect>().clearDictionaries();
            }
        }
        lastPosition = this.transform.position;
        lastRotation = this.transform.rotation;
    }
}

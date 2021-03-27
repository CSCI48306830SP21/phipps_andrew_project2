using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabble : MonoBehaviour
{
    [Tooltip("Set true if the grabbable should be held at a certain point (i.e. the grip of a gun or sword).")]
    [SerializeField]
    private bool useHandle;

    [Tooltip("The position the grabble will be held at if useHandle is set to true.")]
    [SerializeField]
    private Transform handlePos;

    protected Transform anchor;
    protected Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = Mathf.Infinity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() {
        if(anchor != null) {
            // FOLLOW ANCHOR POSITION AND ROTATION
            Vector3 pos = (useHandle) ? handlePos.position : rb.position; 

            Vector3 between = anchor.position - pos;
            rb.velocity = between / Time.deltaTime;

            Quaternion betweenRot = anchor.rotation * Quaternion.Inverse(rb.rotation);
            float angle;
            Vector3 axis;
            betweenRot.ToAngleAxis(out angle, out axis);
            Vector3 anglularVelocity = axis * Mathf.Deg2Rad * angle;
            rb.angularVelocity = anglularVelocity / Time.deltaTime;
        }
    }

    /// <summary>
    /// Grabs the grabbable.
    /// </summary>
    /// <param name="grabber"></param>
    public virtual void Grab(Transform grabber) {
        rb.useGravity = false;
        anchor = grabber;
    }

    public virtual void Release() {
        rb.useGravity = true;
        anchor = null;
    }
}

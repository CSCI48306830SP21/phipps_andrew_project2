using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

[RequireComponent(typeof(Rigidbody))]
public class Hand : MonoBehaviour
{
    [SerializeField]
    private Transform anchor;

    public enum HandSide { Left, Right }
    [SerializeField]
    private HandSide side;
    private OVRInput.Controller myHand;

    [SerializeField]
    private GameObject handGraphics;

    [Tooltip("The percent the main trigger must be pressed down to pick up a grabbable object.")]
    [SerializeField]
    private float gripAtPercent = 0.50f;

    private Grabble grabbed;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = Mathf.Infinity;

        // Determine which hand we are
        if(side == HandSide.Left) {
            myHand = OVRInput.Controller.LTouch;
        }
        else {
            myHand = OVRInput.Controller.RTouch;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // GRAB RELEASE CONTROLS
        float trigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, myHand);
        // If we've release the trigger ad we're holding something, drop/release it.
        if(trigger < gripAtPercent && grabbed != null) {
            grabbed.Release();
            grabbed = null;
            StartCoroutine(ActivatedHandGraphics(0.5f)); // Delay reactivating the hand to prevent collision
        }
    }

    void FixedUpdate() {
        // FOLLOW ANCHOR POSITION AND ROTATION
        Vector3 between = anchor.position - rb.position;
        rb.velocity = between / Time.deltaTime;

        Quaternion betweenRot = anchor.rotation * Quaternion.Inverse(rb.rotation);
        float angle;
        Vector3 axis;
        betweenRot.ToAngleAxis(out angle, out axis);
        Vector3 anglularVelocity = axis * Mathf.Deg2Rad * angle;
        rb.angularVelocity = anglularVelocity / Time.deltaTime;
    }

    private void OnTriggerStay(Collider col) {
        // GRAB CONTROLS
        float trigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, myHand);

        // Check if the object we're trying to grab has a rigidbody / is a grabbable
        Rigidbody colRb = col.attachedRigidbody;
        if (colRb == null) return;

        Grabble grabbable = colRb.GetComponent<Grabble>();
        if (grabbable == null) return;

        // If we're pulling the trigger and we don't have something grabbed already, pick up the object
        if(trigger > gripAtPercent && grabbed == null) {
            grabbed = grabbable;
            handGraphics.SetActive(false);
            grabbable.Grab(anchor);
        }
    }

    /// <summary>
    /// Enables the hand graphics after t seconds.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private IEnumerator ActivatedHandGraphics(float t) {
        yield return new WaitForSeconds(t);
        if (grabbed == null)
            handGraphics.SetActive(true);
    }

    /// <summary>
    /// Returns the input from the primary joystick of the controller controling the hand.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMovementInput() {
        return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, myHand);
    }
}

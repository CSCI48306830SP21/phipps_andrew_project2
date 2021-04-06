using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

[RequireComponent(typeof(Rigidbody))]
public class Hand : MonoBehaviour {
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

    [Tooltip("The maximum distance we can grab an object using raycasts.")]
    [SerializeField]
    private float maxGrabDistance = 5f;

    private float trigger;

    private Grabble grabbed;

    private Rigidbody rb;

    private WorldSpaceButton targetButton;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = Mathf.Infinity;

        // Determine which hand we are
        if (side == HandSide.Left) {
            myHand = OVRInput.Controller.LTouch;
        }
        else {
            myHand = OVRInput.Controller.RTouch;
        }
    }

    // Update is called once per frame
    void Update() {
        // Keep track of trigger position for grabbing/releasing.
        trigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, myHand);

        RaycastHit hit;
        // RAYCAST UI BUTTON CONTROLS
        // Pointing at button
        if (Physics.Raycast(handGraphics.transform.position, handGraphics.transform.forward, out hit, maxGrabDistance)) {
            WorldSpaceButton button = hit.collider.GetComponent<WorldSpaceButton>();

            // If we have no button, save it and highlight it.
            if(button != null && targetButton == null) {
                targetButton = button;
                //targetButton.Highlight();
            }

            // If we're not holding anyting and we've pulled the trigger over the button, click it.
            if (button != null && grabbed == null && trigger >= gripAtPercent)
                targetButton.Click();
        }
        // No longer pointing at button.
        else {
            targetButton = null;
        }

        // RAYCAST GRABBING CONTROLS
        // If we're pointing at a grabbable object, aren't holding anything, and we've pulled the trigger attempt to grab the object.
        if(trigger >= gripAtPercent && grabbed == null && Physics.Raycast(handGraphics.transform.position, handGraphics.transform.forward, out hit, maxGrabDistance)) {
            Grabble g = hit.collider.GetComponentInParent<Grabble>();

            // Make sure the object is grabbable from a distance. Objects like levers are not distance grabbale.
            if(g != null && g.DistanceGrabbable) {
                Grab(g);
            }
        }

        // GRAB RELEASE CONTROLS
        // If we've release the trigger ad we're holding something, drop/release it.
        if (trigger < gripAtPercent && grabbed != null) {
            grabbed.Release();
            grabbed = null;
            StartCoroutine(ActivatedHandGraphics(0.5f)); // Delay reactivating the hand to prevent collision
        }

        // Also check if the hand is in active, but we don't have something grabbed. This can happen when what we were holding was destroyed (i.e. destructible or consumable).
        if(!handGraphics.activeSelf && grabbed == null) {
            StartCoroutine(ActivatedHandGraphics(0.5f));
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
        // If something is already grabbed, don't try to grab anything new.
        if (grabbed != null)
            return;

        // Check if the object we're trying to grab has a rigidbody / is a grabbable
        Rigidbody colRb = col.attachedRigidbody;
        if (colRb == null) return;

        Grabble grabbable = colRb.GetComponent<Grabble>();
        if (grabbable == null) return;

        // Grab the object
        Grab(grabbable);
    }

    /// <summary>
    /// Attempts to grab the provided grabbable.
    /// </summary>
    /// <param name="grabObject"></param>
    private void Grab(Grabble grabObject) {
        // If we're pulling the trigger and we don't have something grabbed already, pick up the object
        if (trigger >= gripAtPercent && grabbed == null) {
            grabbed = grabObject;
            handGraphics.SetActive(false);
            grabObject.Grab(anchor);
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
    public Vector2 GetJoystickInput() {
        return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, myHand);
    }
}
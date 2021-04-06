using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Grabble
{
    [SerializeField]
    private float activateDegree;

    [SerializeField]
    private AudioClip pullSound;

    private HingeJoint joint;

    private bool activated;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        distanceGrabbable = false;
        activated = false;

        joint = GetComponentInParent<HingeJoint>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Start();
    }

    protected override void FixedUpdate() {
        
        if(anchor != null) {
            Vector3 force = 100 * (anchor.position - handlePos.position);
            rb.AddForceAtPosition(force, handlePos.position);
        }

        float currentValue = joint.angle;

        if(!activated && currentValue < activateDegree) {
            activated = true;
            StartCoroutine(Activate());
        }
    }

    /// <summary>
    /// Loads the next floor when the lever is pulled.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Activate() {
        if (pullSound != null) {
            AudioSource.PlayClipAtPoint(pullSound, transform.position);

            yield return new WaitForSeconds(pullSound.length);
        }

        GameManager.Instance.LoadDungeonScene();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Weapon : Grabble
{
    [Header("Weapon Options")]
    [SerializeField]
    private int damage = 10;

    [SerializeField]
    private AudioClip hitSound;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter(Collider col) {
        // Ignore triggers and player
        if (col.tag == "Trigger" || col.tag == "Player")
            return;

        IDamageable damageable = col.GetComponent<IDamageable>();

        // Check if we hit a damageable, deal our damage.
        if(damageable != null) {
            damageable.TakeHit(damage, rb.velocity);

            if(hitSound != null)
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

    }
}

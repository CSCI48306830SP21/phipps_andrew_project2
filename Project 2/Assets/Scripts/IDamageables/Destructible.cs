using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour, IDamageable
{
    [SerializeField]
    private GameObject fracturedPrefab;

    [Tooltip("If true, the object will break after being dropped/thrown (if possible i.e. it's a grabable).")]
    [SerializeField]
    private bool fragile;

    [Tooltip("The velocity the object must meet in order to fracture if set to be fragile.")]
    [SerializeField]
    private float fractureVelocity = 1.5f;

    [Tooltip("Potential item for the grabbale to drop when broken")]
    [SerializeField]
    private Grabble drop;

    [Tooltip("The percent chance the droppable will actually have a drop.")]
    [SerializeField]
    private float dropChance = 0.10f;

    [SerializeField]
    private AudioClip breakSound;

    private bool weaponHit; // track if we've been hit with a weapon to avoid additional collisions

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        // If the object is not grabbable, then don't bother checking if it's recieving damage from being dropped/thrown.
        if (!GetComponent<Grabble>())
            fragile = false;
        else {
            // Grabbables are required to have rigidbodies and audio sources attached.
            rb = GetComponent<Rigidbody>(); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage) {
        TakeHit(damage, Vector3.zero);
    }

    public void TakeHit(int damage, Vector3 velocity) {
        // Disable physics on our self to avoid additional collision.
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;

        // Instantiate fractured object
        GameObject fractured = Instantiate(fracturedPrefab, transform.position, transform.rotation);
        fractured.transform.localScale = transform.localScale; // Make the fracture match our local scale.

        // For each fracture piece apply force to their rigidbody.
        foreach (Rigidbody rb in fractured.GetComponentsInChildren<Rigidbody>()) {
            Vector3 randomForce = new Vector3(Random.Range(50, 100), 0, Random.Range(50, 100)); // Add some random force to the fragments for a better fracture.
            rb.AddForce(randomForce + velocity);
        }

        // Play break sound
        AudioSource.PlayClipAtPoint(breakSound, transform.position);

        // See if we dropped any loot
        if(Random.value > (1 - dropChance)) {
            Instantiate(drop, transform.position, transform.rotation);
        }

        Destroy(gameObject);
        Destroy(fractured, 2f);
    }

    private void OnCollisionEnter(Collision col) {
        // Ignore collision with weapons, as they apply their damage seperately. IF we've been hit with a weapon before do not track collisions.
        if (col.collider.GetComponent<Weapon>() || weaponHit) {
            weaponHit = true;
            return;
        }

        // Ignore collisions with the player.
        if (col.collider.tag == "Player")
            return;

        // If set to fragile and we're falling and hit something, fracture the object. 
        if (fragile && rb != null && rb.velocity.magnitude >= fractureVelocity) {
            TakeHit(1, rb.velocity);
        }
    }
}

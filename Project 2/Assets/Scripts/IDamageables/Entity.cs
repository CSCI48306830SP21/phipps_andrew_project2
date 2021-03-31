using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int maxHealth;

    private int health;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = maxHealth;
    }

    /// <summary>
    /// Deals damage to the Entity. If the damage is equal to or more than the Entity's current health, it dies.
    /// </summary>
    /// <param name="damage"></param>
    public virtual void TakeDamage(int damage) {
        health -= damage;

        if (health <= 0)
            Die();
    }

    public virtual void TakeHit(int damage, Vector3 velocity) {
        TakeDamage(damage);
        
        // TODO: Do something with velocity
    }

    protected virtual void Die() {
        // TODO: Add behaviour for dying. This will typically be different for different Entities (enemies vs players) so the implementation here in the base class likely won't matter.
        // Temporary Debugging.
        Destroy(gameObject);
    }
}

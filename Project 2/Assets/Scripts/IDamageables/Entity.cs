using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int maxHealth;

    private int health;

    private bool isDead;
    public bool IsDead => isDead;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = maxHealth;
        isDead = false;
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

    /// <summary>
    /// Adds health to the Entity's current health.
    /// </summary>
    /// <param name="health"></param>
    public virtual void AddHealth(int health) {
        this.health += health;
        this.health = Mathf.Clamp(this.health, 0, maxHealth);
    }

    protected virtual void Die() {
        isDead = true;
    }
}

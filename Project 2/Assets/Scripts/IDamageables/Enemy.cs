using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Entity
{
    [SerializeField]
    private float attackRange = 2f;

    [Tooltip("The rate in seconds that the enemy attacks.")]
    [SerializeField]
    private float attackSpeed = 2f;

    [SerializeField]
    private int damage = 20;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float smoothTurn = 0.8f;

    [SerializeField]
    private CapsuleCollider bodyCollider;

    private NavMeshAgent agent;

    private Player target;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider col) {
        // Don't attempt to get a new target if we have one already
        if (target != null)
            return;

        // Check if a player is in our aggro range.
        Player t = col.GetComponent<Player>();
        
        // Get body offset
        Vector3 bodyOffset = new Vector3(transform.position.x, transform.position.y + bodyCollider.height, transform.position.z);

        int layerMask = ~((1 << 8) | (1 << 9)); // Don't include Player/Entity layer
        // Check to see if a player has entered our "aggro" range. If they have check to see if they're in line of sight.
        if (t != null && !Physics.Linecast(bodyOffset, t.transform.position, layerMask)) {
            target = t;
            StartCoroutine(Fight());
        }
    }

    private IEnumerator Fight() {
        // Make sure we still have a valid target
        if (target.IsDead) {
            target = null;
            yield break;
        }

        // Chase our target. Wait until we've gotten into attack range.
        yield return StartCoroutine(Chase());

        // Attack our target once we've made it within range.
        yield return StartCoroutine(Attack());

        // Repeat the process.
        StartCoroutine(Fight());
    }

    /// <summary>
    /// Chases the current target until we are within attack range.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Chase() {
        if (target == null || target.IsDead)
            yield break;
        
        // Start moving in the player's direction. Set stopping distance to attack range.
        agent.SetDestination(target.transform.position);
        agent.stoppingDistance = attackRange;

        yield return new WaitForSeconds(0.1f); // wait a second to give the agent time to gain speed
        while (agent.velocity != Vector3.zero) {
            animator.SetFloat("Velocity", agent.velocity.magnitude); // set animator's velocity to play walking animation

            // Continuously update path to follow player
            agent.SetDestination(target.transform.position);
            yield return null; // wait
        }

        // Reset stopping distance
        agent.isStopped = true;
        agent.ResetPath();
        agent.stoppingDistance = 0;
    }

    /// <summary>
    /// Coroutine that moves the Enemy to the specified point.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private IEnumerator MoveToPoint(Vector3 point) {
        agent.SetDestination(point);
        yield return new WaitForSeconds(0.1f); // wait a second to give the agent time to gain speed
        while (agent.velocity != Vector3.zero) {
            animator.SetFloat("Velocity", agent.velocity.magnitude); // set animator's velocity to play walking animation
            yield return null; // wait
        }
    }

    private IEnumerator Attack() {
        // Attack the target while they're still alive and in range.
        float nextAttackTime = 0;
        while(!target.IsDead && Vector3.Distance(transform.position, target.transform.position) <= attackRange) {
            // Always face the player while attacking
            Quaternion targetRot = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * smoothTurn);

            // Attack once every attackSpeed seconds
            if (Time.time >= nextAttackTime) {
                animator.Play("Attack", 0);

                target.TakeDamage(damage);

                // Use attack animation as attack speed
                nextAttackTime = Time.time + attackSpeed;
            }

            yield return null;
        }
    }

    protected override void Die() {
        base.Die();

        // TODO: Do implement death behaviour for enemies.
        Destroy(gameObject);
    }
}

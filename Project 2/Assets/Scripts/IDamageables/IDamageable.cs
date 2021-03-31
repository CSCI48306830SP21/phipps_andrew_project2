using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage);

    void TakeHit(int damage, Vector3 velocity);
}

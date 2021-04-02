using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Heal Consumable", menuName ="ScriptableObjects/Consumables/HealConsumable")]
public class ConsumableHealEffect : ConsumableEffect
{
    [SerializeField]
    private int healAmount;

    public override void Effect(Player player) {
        player.AddHealth(healAmount);
    }
}

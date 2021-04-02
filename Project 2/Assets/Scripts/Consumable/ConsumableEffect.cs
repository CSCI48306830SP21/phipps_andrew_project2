using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableEffect : ScriptableObject
{
    /// <summary>
    /// The effect the consumable has on the player.
    /// </summary>
    /// <param name="player"></param>
    public abstract void Effect(Player player);

}

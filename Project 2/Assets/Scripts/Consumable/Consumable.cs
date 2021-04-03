using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : Grabble
{
    [SerializeField]
    private ConsumableEffect effect;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
    }

    /// <summary>
    /// Gives the consumables effect to the provided Player.
    /// </summary>
    /// <param name="player"></param>
    public void Consume(Player player) {
        effect.Effect(player);
        AudioSource.PlayClipAtPoint(effect.ConsumeSound, transform.position);
        Destroy(gameObject);
    }
}

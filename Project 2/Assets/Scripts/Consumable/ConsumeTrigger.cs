using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a trigger area for consuming consumable objects.
/// </summary>
public class ConsumeTrigger : MonoBehaviour
{
    [SerializeField]
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col) {
        Consumable c = col.GetComponent<Consumable>();

        if (c != null) {
            c.Consume(player);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnPrefab;

    [SerializeField]
    private float timer = 2.0f;

    [Tooltip("If true, objects will only spawn if the current number of objects is less than the pool size.")]
    [SerializeField]
    private bool pool;

    [SerializeField]
    private int poolSize = 1;

    private List<GameObject> poolList;

    // Start is called before the first frame update
    void Start()
    {
        poolList = new List<GameObject>();

        StartCoroutine(Spawn());
    }

    /// <summary>
    /// Infinitely spawn objects using the spawn timer.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Spawn() {
        while (true) {
            // If we're using a pool, don't spawn any more objects if we've reached our pool limit.
            if (pool && poolList.Count >= poolSize) {
                // Check if any of the objects in the pool have been deleted
                poolList.RemoveAll(i => i == null);

                yield return null;
                continue;
            }

            GameObject spawn = Instantiate(spawnPrefab, transform.position, transform.rotation);

            // If using a pool add the object to the pool list.
            if (pool) poolList.Add(spawn);

            yield return new WaitForSeconds(timer);
        }
    }
}

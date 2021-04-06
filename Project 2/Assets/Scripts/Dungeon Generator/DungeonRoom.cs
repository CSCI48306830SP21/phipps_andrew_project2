using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DungeonRoom : MonoBehaviour
{
    [SerializeField]
    private Doors doors;

    public enum DoorDirection { North, East, South, West }

    [Tooltip("The chance that this room will have an enemy in it.")]
    [SerializeField]
    private float enemySpawnChance = 0.50f;

    [SerializeField]
    private Enemy[] enemies;

    [SerializeField]
    private GameObject[] clutter;

    [Tooltip("Set spawn points for clutter within the room.")]
    [SerializeField]
    private Transform[] clutterSpawnPoints;

    [Tooltip("Set spawn points for enemies within the room.")]
    [SerializeField]
    private Transform[] enemySpawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        // Fill the room with enemies/clutter.
        StartCoroutine(PopulateRoom());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleDoor(DoorDirection direction) {
        switch (direction) {
            case DoorDirection.North:
                doors.NorthDoor.SetActive(false);
                break;
            case DoorDirection.East:
                doors.EastDoor.SetActive(false);
                break;
            case DoorDirection.South:
                doors.SouthDoor.SetActive(false);
                break;
            case DoorDirection.West:
                doors.WestDoor.SetActive(false);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Fills the room with clutter and enemies at the specified points.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PopulateRoom() {
        // Spawn clutter in each of the clutter spawn points
        foreach (Transform point in clutterSpawnPoints) {
            GameObject c = clutter[Random.Range(0, clutter.Length)];
            GameObject newClutter = Instantiate(c, point.position, point.rotation);
            newClutter.transform.parent = transform;
            yield return null;
        }

        // Potentially spawn an enemy in one of the enemy spawn points
        foreach (Transform point in enemySpawnPoints) {
            if (Random.value >= (1 - enemySpawnChance)) {
                Enemy e = enemies[Random.Range(0, enemies.Length)];
                Enemy newEnemy = Instantiate(e, point.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
                newEnemy.transform.parent = transform;
                yield break;
            }
        }
    }

    [System.Serializable]
    public class Doors {

        [SerializeField]
        private GameObject northDoor;
        public GameObject NorthDoor => northDoor;

        [SerializeField]
        private GameObject eastDoor;
        public GameObject EastDoor => eastDoor;

        [SerializeField]
        private GameObject southDoor;
        public GameObject SouthDoor => southDoor;

        [SerializeField]
        private GameObject westDoor;
        public GameObject WestDoor => westDoor;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon Floor Data", menuName = "ScriptableObjects/DungeonFloorData/DungeonFloorData")]
public class DungeonFloorData : ScriptableObject
{
    [Tooltip("The possible number of rooms to be generated")]
    [SerializeField]
    private Vector2Int roomsMinMax;
    public Vector2Int RoomsMinMax => roomsMinMax;

    [Tooltip("The room size in x and y.")]
    [SerializeField]
    private Vector2 roomSize;
    public Vector2 RoomSize => roomSize;

    [SerializeField]
    private DungeonRoom roomPrefab;
    public DungeonRoom RoomPrefab => roomPrefab;

    [SerializeField]
    private DungeonRoom startRoomPrefab;
    public DungeonRoom StartRoomPrefab => startRoomPrefab;

    [SerializeField]
    private DungeonRoom endRoomPrefab;
    public DungeonRoom EndRoomPrefab => EndRoomPrefab;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private int[,] rooms;

    private List<Vector2Int> usedRooms;

    private int roomCount;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator GenerateFloor(DungeonFloorData data) {
        roomCount = Random.Range(data.RoomsMinMax.x, data.RoomsMinMax.y);
        usedRooms = new List<Vector2Int>();

        // Rooms are stored in grid
        rooms = new int[roomCount * 2, roomCount * 2];

        // Starting room is always at the center
        rooms[roomCount, roomCount] = 1;

        // Generate room map from center out
        Vector2Int currentPos = new Vector2Int(roomCount, roomCount);
        usedRooms.Add(currentPos);
        for (int i = 0; i < roomCount; i++) {
            // Choose a random room. 0 = north, 1 = east, 2 = south, 3 = west
            int nextRoom = Random.Range(0, 3);

            // Keep checking for an empty position for the room.
            while(CheckRoomPos(currentPos.x, currentPos.y, nextRoom)) {
                nextRoom = Random.Range(0, 3);
                yield return null;
            }

            // Assign the current room to the map.
            currentPos = NewRoomCoord(currentPos.x, currentPos.y, nextRoom);
            rooms[currentPos.x, currentPos.y] = 1;
            usedRooms.Add(currentPos);
        }

        print("Done mapping");
        yield return StartCoroutine(BuildRooms(data));
    }

    private IEnumerator BuildRooms(DungeonFloorData data) {
        print("Building");

        // Instantiate room prefabs using the rooms map
        for(int i = 0; i < usedRooms.Count; i++) {
            Vector2Int r = usedRooms[i];
            int row = r.x;
            int col = r.y;

            Vector3 roomPosition = new Vector3(row * data.RoomSize.x, 0, col * data.RoomSize.y);
            DungeonRoom room;

            // Spawn the spawn room in separately
            if (i == 0) {
                room = Instantiate(data.StartRoomPrefab, roomPosition, transform.rotation);
            }
            // Spawn end room in the last room generated
            else if(i == usedRooms.Count - 1) {
                room = Instantiate(data.EndRoomPrefab, roomPosition, transform.rotation);
            }
            // Instantiate room
            else {
                room = Instantiate(data.RoomPrefab, roomPosition, transform.rotation);
                room.name = "Room (" + row + ", " + col + ")";
            }

            // Toggle doors based on the room's neighbors.
            if (GetNeighbor(row, col, 0))
                room.ToggleDoor(DungeonRoom.DoorDirection.North);
            if (GetNeighbor(row, col, 1))
                room.ToggleDoor(DungeonRoom.DoorDirection.East);
            if (GetNeighbor(row, col, 2))
                room.ToggleDoor(DungeonRoom.DoorDirection.South);
            if (GetNeighbor(row, col, 3))
                room.ToggleDoor(DungeonRoom.DoorDirection.West);
            yield return null;
        }
    }

    /// <summary>
    /// Checks if the provided room pos is valid. Returns true if valid, false otherwise.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    private bool CheckRoomPos(int row, int col, int dir) {
        switch (dir) {
            // North
            case 0:
                return row - 1 < 0 || rooms[row - 1, col] == 1;
            // East
            case 1:
                return col + 1 == rooms.GetLength(1) || rooms[row, col + 1] == 1; 
            // South
            case 2:
                return row + 1 == rooms.GetLength(0) || rooms[row + 1, col] == 1; 
           // West
            case 3:
                return col - 1 < 0 || rooms[row, col - 1] == 1;
            default:
                Debug.LogError("Invalid direction provided for new room.");
                return false;
        }
    }

    /// <summary>
    /// Provides the coordinates of a new room, given the current room coordinates and a direction.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    private Vector2Int NewRoomCoord(int row, int col, int dir) {
        switch (dir) {
            // North
            case 0:
                return new Vector2Int(row - 1, col);
            // East
            case 1:
                return new Vector2Int(row, col + 1);
            // South
            case 2:
                return new Vector2Int(row + 1, col);
            // West
            case 3:
                return new Vector2Int(row, col - 1);
            default:
                Debug.LogError("Invalid direction provided for new room.");
                return Vector2Int.zero;
        }
    }

    /// <summary>
    /// Returns if the provided room has a neighbor in the provided direction.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    private bool GetNeighbor(int row, int col, int dir) {
        switch (dir) {
            // North
            case 0:
                return row - 1 >= 0 && rooms[row - 1, col] == 1;
            // East
            case 1:
                return col + 1 < rooms.GetLength(1) && rooms[row, col + 1] == 1;
            // South
            case 2:
                return row + 1 < rooms.GetLength(0) && rooms[row + 1, col] == 1;
            // West
            case 3:
                return col - 1 >= 0 && rooms[row, col - 1] == 1;
            default:
                Debug.LogError("Invalid direction provided for new room.");
                return false;
        }
    }
}

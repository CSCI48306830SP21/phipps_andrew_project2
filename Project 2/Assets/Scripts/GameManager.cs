using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Singleton
    private static GameManager instance;
    public static GameManager Instance => instance;

    [SerializeField]
    private Player player;

    [Header("Dungeon Floor Settings")]
    [SerializeField]
    private DungeonLevel[] levels;

    [Header("Music")]
    [SerializeField]
    private AudioClip mainMenuTheme;

    [SerializeField]
    private AudioClip dungeonTheme;

    [SerializeField]
    private AudioClip dungeonIntroTheme;

    [Header("Stats")]
    [SerializeField]
    private Stat maxFloorStat;

    private int currentFloor;
    public int CurrentFloor => currentFloor;

    private Text maxLevelTracker;

    void Awake() {
        DontDestroyOnLoad(this);
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnLevelDoneLoading;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelDoneLoading;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Use this instead of start, since start is not called when the level is loaded
    void OnLevelDoneLoading(Scene scene, LoadSceneMode mode) {
        // Enforce singleton pattern
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;

        GenerateDungeon();

        StartCoroutine(SeachForPlayer());

        SetMaxLevelTracker();
        StartCoroutine(SetMusic());
    }

    /// <summary>
    /// Loads the dungeon scene and updates the max level reached if necessary.
    /// </summary>
    public void LoadDungeonScene() {
        // Update current level and max level score
        if (SceneManager.GetActiveScene().name == "Main Menu")
            currentFloor = 1;
        else if (SceneManager.GetActiveScene().name == "Dungeon")
            currentFloor++;

        if (currentFloor > maxFloorStat.Value)
            maxFloorStat.SetStatValue(currentFloor);

        // Load Dungeon Scene
        SceneManager.LoadScene("Dungeon");
    }

    public void LoadMainMenuScene() {
        SceneManager.LoadScene("Main Menu");
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void Quit() {
        Application.Quit();
    }

    private void GenerateDungeon() {
        if(SceneManager.GetActiveScene().name == "Dungeon") {
            print("Start generating");

            DungeonGenerator dungeonGenerator = FindObjectOfType<DungeonGenerator>();

            DungeonFloorData data = GetCurrentLevel();
            print("Floor: " + data);

            StartCoroutine(dungeonGenerator.GenerateFloor(GetCurrentLevel()));
        }
    }

    private DungeonFloorData GetCurrentLevel() {
        // Check if we're on the first floor.
        if (currentFloor == 1)
            return levels[0].Data;

        for(int i = 0; i < levels.Length; i++) {
            // Check if we're on the last level
            if (currentFloor >= levels[levels.Length - 1].StartFloor)
                return levels[levels.Length - 1].Data;

            // Check if we're in one of the lower levels.
            if(currentFloor < levels[i].StartFloor)
                return levels[i - 1].Data;
        }

        return null;
    }

    /// <summary>
    /// Searches for the player in the scene. Used for when finding the player after the dungeon has been generated.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SeachForPlayer() {
        while(player == null) {
            player = FindObjectOfType<Player>();

            yield return null;
        }
    }
    
    private void SetMaxLevelTracker() {
        if(SceneManager.GetActiveScene().name == "Main Menu") {
            maxLevelTracker = GameObject.Find("Score Text").GetComponent<Text>();

            maxLevelTracker.text = maxFloorStat.Value.ToString();
        }
    }

    private void PlayMusic(AudioClip music) {
        if (player == null)
            return;

        player.MusicSource.clip = music;
        player.MusicSource.Play();
    }

    /// <summary>
    /// Sets the music that should be played based of the scene and current floor.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetMusic() {
        // Wait for the player to be found.
        while (player == null)
            yield return null;

        // If in the main menu, play the menu music
        if(SceneManager.GetActiveScene().name == "Main Menu") {
            PlayMusic(mainMenuTheme);
            yield break;
        }
        // If in the dungeon, play the dungeon music
        else if (SceneManager.GetActiveScene().name == "Dungeon") {
            // If we've reached a new level, play the dungeon intro theme
            if (IsStartLevel()) {
                PlayMusic(dungeonIntroTheme);

                // Play the normal dungeon theme after the intro
                yield return new WaitForSeconds(dungeonIntroTheme.length);

                PlayMusic(dungeonTheme);
            }
            // Otherwise, just play the normal theme.
            else
                PlayMusic(dungeonTheme);
        }
    }

    /// <summary>
    /// Returns true if we're on the starting floor of one of the dungeon levels.
    /// </summary>
    /// <returns></returns>
    private bool IsStartLevel() {
        foreach(DungeonLevel level in levels) {
            if (currentFloor == level.StartFloor)
                return true;
        }

        return false;
    }

    [System.Serializable]
    private class DungeonLevel {
        [SerializeField]
        private int startFloor;
        public int StartFloor => startFloor;

        [SerializeField]
        private DungeonFloorData data;
        public DungeonFloorData Data => data;
    }
}

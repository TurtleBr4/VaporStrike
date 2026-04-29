using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;

public enum gameState
{
    Menu, Game
}
public class GameManager : MonoBehaviour
{
    [Header("Static/Global Variables/Constants")]
    public int gameFrameRate = 60;
    public float platformFrameTime = 24f;
    public float fastFallMultiplier = 1.25f;
    public int parryFrameTime = 4;
    public bool debugFlag = false;
    
    public float matchTimer = 120;

    public static gameState gameState = gameState.Game;
    
    public static int playerCount = 2; //this defaults to 2

    [Header("Multiplayer Stuff")]
    [SerializeField] private PlayerInputManager pInputManager;
    [SerializeField] private int index;
    [SerializeField] private PlayerData[] playerDataArray = new PlayerData[playerCount];
    [SerializeField] private GenericCharacter[] characterArray = new GenericCharacter[playerCount];
    [SerializeField] private PlayerDatabase playerDatabase;
    [SerializeField] private GameObject[] playerPercentsArray = new GameObject[playerCount];
    public bool isMatch = false;
    public GenericCharacter player1Obj;
    public GenericCharacter player2Obj;
    public string DebugTxt = "";
    public enum ResultType
    {
        Start,
        NoContest,
        Player1,
        Player2,
    }
    public ResultType matchResult = ResultType.Start;
    public static GameManager instance { get; private set; }

    private LevelData chosenLevel;
    private GameObject[] levels;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            Debug.Log("Destroying");
        }
        else
        {
            Debug.Log("Creating Instance");
            instance = this;
            DontDestroyOnLoad(gameObject);
        } 
        
        playerDatabase = Resources.Load<PlayerDatabase>("Data/Character/PlayerDatabase");
        //playerDatabase.players = Resources.LoadAll<PlayerData>("Data/Character").ToList<PlayerData>();
        chosenLevel = Resources.Load<LevelData>("Data/Levels/ChosenLevel");
    }

    private void Start()
    {
        if (debugFlag)
        {
            pickPlayer(1, 3);
            pickPlayer(0, 3);
        }
        onGameStateChanged(gameState);
        if (gameState == gameState.Game) //cap the framerate at 60 for gameplay for simplicity
        {
            Application.targetFrameRate = gameFrameRate;
        }
    }
    public void Init()
    {
        if (debugFlag) //we're in debug here so the hud never bothers spawning things, default to the old behavior
        {
            onGameStateChanged(gameState);
            LoadLevel();
            if (gameState == gameState.Game) //cap the framerate at 60 for gameplay for simplicity
            {
                Application.targetFrameRate = gameFrameRate;
            }
        }
        
    }
    private void Update()
    {

        if (player1Obj != null && player2Obj != null)
        {
            if (isMatch)
            {
                if (matchTimer > 0)
                {
                    // Subtract the time since last frame
                    matchTimer -= Time.deltaTime;
                }
                else
                {
                    Debug.Log("Game!");
                    matchTimer = 0;
                    isMatch = false;

                    if (player1Obj.GetStocks() == player2Obj.GetStocks())
                    {
                        matchResult = ResultType.NoContest;
                    }
                    else if (player1Obj.GetStocks() <= 0)
                    {
                        matchResult = ResultType.Player2;
                    }
                    else
                    {
                        matchResult = ResultType.Player1;
                    }
                }
            }

            if (player1Obj.GetStocks() <= 0)
            {
                isMatch = false;
                matchResult = ResultType.Player2;
            }

            if (player2Obj.GetStocks() <= 0)
            {
                isMatch = false;
                matchResult = ResultType.Player1;
            }
        }
    }
    public string DisplayTime()
    {
        float minutes = Mathf.FloorToInt(matchTimer / 60);
        float seconds = Mathf.FloorToInt(matchTimer % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void Reset()
    {
        playerDataArray = new PlayerData[playerCount];
    }
    public void pickPlayer(int playerIndex, int charID) //called via the menu or something, assigns the player data to its corresponding player slot
    {
        playerDataArray[playerIndex] = playerDatabase.getPlayerById(charID);
    }
    public PlayerData GetPlayerByID(int id)
    {
        return playerDatabase.getPlayerById(id);
    }
    public PlayerData GetPlayer(int playerIndex)
    {
        return playerDataArray[playerIndex];
    }
    public void onGameStateChanged(gameState newState)
    {
        if (newState == gameState.Game)
        {
            if (pInputManager)
            {
                pInputManager.DisableJoining();
            }
            pInputManager.enabled = false; //just in case

            Application.targetFrameRate = gameFrameRate;
            GameObject guy;
            Level level = GameObject.Find("LevelData").GetComponent<Level>();
            for(int i = 0; i < level.levelSpawns.Length; i++)
            {
                guy = Instantiate(playerDataArray[i].playerPrefab);
                if (guy)
                {
                    print("Spawned Character Prefab");
                }
                else
                {
                    Debug.LogError("What the Hell!");
                }
                
            }

            characterArray = FindObjectsByType<GenericCharacter>(FindObjectsSortMode.InstanceID);
            characterArray[0].transform.position = level.levelSpawns[0].position;
            characterArray[1].transform.position = level.levelSpawns[1].position;



            for (int i = 0; i < playerCount; i++)
            {
                assignPlayerDataToPlayer(playerDataArray[i], i);
                Debug.Log("Player count: " + playerCount);
            }

            level.cam.init();
        }
        else //this is where we handle the cursor spawning fuck shit >:)
        {
            if (!pInputManager)
            {
                pInputManager = GetComponent<PlayerInputManager>();

            }
            pInputManager.DisableJoining();
            index = 0;
            pInputManager.playerPrefab = playerDataArray[index].playerPrefab;
        }
    }

    public void switchAndSpawnNext(PlayerInput pi)
    {
        index++;

        //Stash the device that was used to spawn this character
        InputDevice device = pi.GetDevice<InputDevice>();
        InputReader spawnedCharacter = pi.gameObject.GetComponent<InputReader>();
        spawnedCharacter.controllerID = device.displayName == "Keyboard" || device.displayName == "Mouse" ? "Mouse" : "Controller";

        pInputManager.playerPrefab = playerDataArray[Mathf.Min(playerCount - 1, index)].playerPrefab;

    }

    void assignPlayerDataToPlayer(PlayerData p, int playerIndex)
    {
        if (playerIndex > characterArray.Length || characterArray.Length <=0) { return; } else
        {
            characterArray[playerIndex].assignPlayerData(p, playerIndex);
            Debug.Log("Index");
        }
            
        //characterArray[playerIndex].assignPlayerData(p, playerIndex);
    }

    public void LoadLevel()
    {
        GameObject levelPrefab = levels[chosenLevel.levelID];
        Instantiate(levelPrefab);
    }
}

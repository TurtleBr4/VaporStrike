using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;

public enum gameState
{
    Menu, Game
}

public enum ResultType
{
    Start,
    NoContest,
    Player1,
    Player2,
}
public class GameManager : MonoBehaviour
{
    #region OLD GAME MANAGER
    /*
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
            pickPlayer(1, 2);
            pickPlayer(0, 3);
        }
        //onGameStateChanged(gameState);
        if (gameState == gameState.Game) //cap the framerate at 60 for gameplay for simplicity
        {
            Application.targetFrameRate = gameFrameRate;
        }
    }
    public void Init()
    {
        matchResult = ResultType.Start;
        Time.timeScale = 1.0f;
        matchTimer = 120;
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
                Debug.Log("Spawning");
                if(playerDataArray[i] != null)
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


                
            }

            characterArray = FindObjectsByType<GenericCharacter>(FindObjectsSortMode.InstanceID);
            characterArray[1].transform.position = level.levelSpawns[1].position;
            characterArray[0].transform.position = level.levelSpawns[0].position;

            assignPlayerDataToPlayer(playerDataArray[1], 0);
            assignPlayerDataToPlayer(playerDataArray[0], 1);


            for (int i = 0; i < playerCount; i++)
            {
                //assignPlayerDataToPlayer(playerDataArray[i], i);
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
    */
    #endregion
    public static GameManager instance;


    [Header("Static/Global Variables/Constants")]
    public int gameFrameRate = 60;
    public float platformFrameTime = 24f;
    public float fastFallMultiplier = 1.25f;
    public int parryFrameTime = 4;
    public bool debugFlag = false;

    [Header("Game State")]
    public float matchTimer = 120;
    public gameState currentState = gameState.Menu;
    public int playerCount = 2;
    public ResultType matchResult = ResultType.Start;
    [SerializeField] private PlayerData[] playerDataArray;
    public bool isMatch = false;
    public bool isTraining = false;

    [Header("Multiplayer Management")]
    [SerializeField] private int index;

    [Header("Dependencies")]
    [SerializeField] private PlayerDatabase playerDatabase;
    [SerializeField] private PlayerInputManager pInputManager;

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
            playerDatabase = Resources.Load<PlayerDatabase>("Data/Character/PlayerDatabase");
        }

    }

    private void Start()
    {

    }

    public void Update()
    {
        if (debugFlag)
        {
            //playerDataArray[0] = playerDatabase.getPlayerById(2);
            playerDataArray[1] = playerDatabase.getPlayerById(3);
        }
    }

    public void Init()
    {
        matchResult = ResultType.Start;
        Time.timeScale = 1.0f;
        matchTimer = 120;
        Application.targetFrameRate = gameFrameRate;
    }

    public void changeState(gameState newState)
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
            Vector3 pos1 = level.levelSpawns[0].position;
            Vector3 pos2 = level.levelSpawns[1].position;
            int playersSpawned = 0;
            for (int i = 0; i < level.levelSpawns.Length; i++)
            {

                guy = playerDataArray[i].playerPrefab;
                guy.GetComponent<Player>().setPlayerID(playersSpawned);
                playersSpawned++;
                if (guy.GetComponent<Player>() != null)
                {
                    if (guy.GetComponent<Player>().getPlayerID() == 0)
                    {
                        Instantiate(guy, pos1, Quaternion.identity);
                    }
                    if (guy.GetComponent<Player>().getPlayerID() == 1)
                    {
                        Instantiate(guy, pos2, Quaternion.identity);
                    }
                }
                if (guy)
                {
                    print("Spawned Character Prefab");
                }
                else
                {
                    Debug.LogError("What the Hell!");
                }

            }


            for (int i = 0; i < playerCount; i++)
            {
                //assignPlayerDataToPlayer(playerDataArray[i], i);
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

    public void Reset()
    {
        playerDataArray = new PlayerData[playerCount];
    }

    public PlayerData GetPlayerByID(int id)
    {
        return playerDatabase.getPlayerById(id);
    }

    public void pickPlayer(int playerIndex, int charID) //called via the menu or something, assigns the player data to its corresponding player slot
    {
        playerDataArray[playerIndex] = playerDatabase.getPlayerById(charID);
    }
    public void pickPlayer(int playerIndex, PlayerData chara) //called via the menu or something, assigns the player data to its corresponding player slot
    {
        playerDataArray[playerIndex] = chara;
    }

    public PlayerData GetPlayer(int playerIndex)
    {
        return playerDataArray[playerIndex];
    }

    public string DisplayTime()
    {
        float minutes = Mathf.FloorToInt(matchTimer / 60);
        float seconds = Mathf.FloorToInt(matchTimer % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum gameState
{
    Menu, Game
}
public class GameManager : MonoBehaviour
{
    [Header("Static/Global Variables/Constants")]
    public int gameFrameRate = 60;
    public float platformFrameTime = 24f;
    public float ffMult = 1.25f;
    public int parryFrameTime = 30;
    
    public static gameState gameState = gameState.Game;
    
    public static int playerCount = 2; //this defaults to 2

    [Header("Multiplayer Stuff")]
    [SerializeField] private PlayerInputManager pInputManager;
    [SerializeField] private int index;
    [SerializeField] private PlayerData[] playerDataArray = new PlayerData[playerCount];
    [SerializeField] private GenericCharacter[] characterArray = new GenericCharacter[playerCount];
    [SerializeField] private PlayerDatabase playerDatabase;

    private void Awake()
    {
        playerDatabase = Resources.Load<PlayerDatabase>("Data/Character/PlayerDatabase");
        
    }

    private void Start()
    {
        pickPlayer(0, 0);
        pickPlayer(1, 0);
        onGameStateChanged(gameState);
        if (gameState == gameState.Game) //cap the framerate at 60 for gameplay for simplicity
        {
            Application.targetFrameRate = gameFrameRate;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) //just to test
        {
            pickPlayer(0, 0);
            assignPlayerDataToPlayer(playerDataArray[0], 1);
        }
    }

    public void pickPlayer(int playerIndex, int charID) //called via the menu or something, assigns the player data to its corresponding player slot
    {
        playerDataArray[playerIndex] = playerDatabase.getPlayerById(charID);
    }

    void onGameStateChanged(gameState newState)
    {
        if (newState == gameState.Game)
        {
            if (pInputManager)
            {
                pInputManager.DisableJoining();
            }
            pInputManager.enabled = false; //just in case

            Application.targetFrameRate = gameFrameRate;
            
            characterArray = FindObjectsByType<GenericCharacter>(FindObjectsSortMode.None);
            for (int i = 0; i < playerCount; i++)
            {
                assignPlayerDataToPlayer(playerDataArray[i], i);
            }
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
        characterArray[playerIndex].assignPlayerData(p, playerIndex);
    }
}

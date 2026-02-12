using System;
using UnityEngine;

public enum gameState
{
    Menu, Game
}
public class GameManager : MonoBehaviour
{
    [Header("Static/Global Variables/Constants")]
    public int gameFrameRate = 60;
    public float platformFrameTime = 24f;
    
    public static gameState gameState = gameState.Game;
    
    public static int playerCount = 2; //this defaults to 2
    
    [SerializeField]private PlayerData[] playerDataArray = new PlayerData[playerCount];
    [SerializeField]private GenericCharacter[] characterArray = new GenericCharacter[playerCount];

    [SerializeField]private PlayerDatabase playerDatabase;

    private void Awake()
    {
        playerDatabase = Resources.Load<PlayerDatabase>("Data/Character/PlayerDatabase");
        
    }

    private void Start()
    {
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
        }
    }

    public void pickPlayer(int playerIndex, int charID) //called via the menu or something
    {
        playerDataArray[playerIndex] = playerDatabase.getPlayerById(charID);
    }

    void onGameStateChanged(gameState newState)
    {
        if (newState == gameState.Game)
        {
            Application.targetFrameRate = gameFrameRate;
            characterArray = FindObjectsByType<GenericCharacter>(FindObjectsSortMode.None);
            for (int i = 0; i < playerCount; i++)
            {
                assignPlayerDataToPlayer(playerDataArray[i], i);
            }
        }
    }

    void assignPlayerDataToPlayer(PlayerData p, int playerIndex)
    {
        characterArray[playerIndex].assignPlayerData(p);
    }
}

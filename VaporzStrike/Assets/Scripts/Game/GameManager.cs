using System;
using UnityEngine;

public enum gameState
{
    Menu, Game
}
public class GameManager : MonoBehaviour
{
    [Header("Static/Global Variables/Constants")]
    public static int playerCount = 2; //this defaults to 2
    public const int gameFrameRate = 60;
    public const float platformFrameTime = 24f;
    public static gameState gameState = gameState.Menu;
    
    private PlayerData[] playerDataArray = new PlayerData[playerCount];
    private GenericCharacter[] characterArray = new GenericCharacter[playerCount];

    private void Start()
    {
        if (gameState == gameState.Game) //cap the framerate at 60 for gameplay for simplicity
        {
            Application.targetFrameRate = gameFrameRate;
        }
    }

    void onGameStateChanged(gameState newState)
    {
        if (newState == gameState.Game)
        {
            Application.targetFrameRate = gameFrameRate;
            characterArray = FindObjectsByType<GenericCharacter>(FindObjectsSortMode.None);
        }
    }

    void assignPlayerDataToPlayer(PlayerData p, int playerIndex)
    {
        characterArray[playerIndex].assignPlayerData(p);
    }
}

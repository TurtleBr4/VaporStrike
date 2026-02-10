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
    public static gameState gameState = gameState.Menu;

    private void Start()
    {
        if (gameState == gameState.Game) //cap the framerate at 60 for gameplay for simplicity
        {
            Application.targetFrameRate = gameFrameRate;
        }
    }
}

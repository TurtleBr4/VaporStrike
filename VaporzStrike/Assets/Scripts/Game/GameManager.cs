using UnityEngine;

public enum gameState
{
    Menu, Game
}
public class GameManager : MonoBehaviour
{
    [Header("Static/Global Variables/Constants")]
    public static int playerCount = 2; //this defaults to 2
}

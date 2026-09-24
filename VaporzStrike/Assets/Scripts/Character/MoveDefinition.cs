using UnityEngine;

[CreateAssetMenu(
    fileName = "New Move",
    menuName = "Moves"
)]
public class MoveDefinition : ScriptableObject
{
    [Header("Move")]
    public string moveName;

    [Header("Input")]
    public GameInput button;

    public InputDirection direction =
        InputDirection.Neutral;

    [Header("Input Forgiveness")]
    [Min(0)]
    public int directionWindow = 6;

    [Min(0)]
    public int buttonWindow = 6;

    [Min(0)]
    public int bufferDuration = 8;

    [Header("Animation Data")] 
    public int animationID = 0;
    
    [Header("Data")]
    public int frameTime;
    public int endLag;
    public int hitStun;
}
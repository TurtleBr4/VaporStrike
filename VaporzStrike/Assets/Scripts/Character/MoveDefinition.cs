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
    public bool isAirMove = false;
    public int frameTime; // Includes end lag.
    public int hitStun;
    public int cancelTime; // Frame when the player can act again. 0 means no cancel.
    public bool hasDisplacement = false;

    [Header("Displacement")]
    public Vector2 displacementDirection;
    public float displacementMagnitude;
    public int displacementStartFrame;
    public int displacementEndFrame;

    [Header("Hitboxes")]
    public int[] hitboxIDs;
    public int[] hitboxStartFrames;
    public int[] hitboxEndFrames;
}
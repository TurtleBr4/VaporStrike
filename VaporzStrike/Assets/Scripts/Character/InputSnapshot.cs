using UnityEngine;

public struct InputSnapshot
{
    public int frame;

    public Vector2 leftStick;
    public Vector2 rightStick;

    public InputDirection direction;

    public bool attack;
    public bool special;
    public bool jump;
    public bool shield;
    public bool grab;
    public bool parry;

    public bool attackPressed;
    public bool specialPressed;
    public bool jumpPressed;
    public bool shieldPressed;
    public bool grabPressed;

    public bool attackReleased;
    public bool specialReleased;
    public bool jumpReleased;
    public bool shieldReleased;
    public bool grabReleased;

    public int attackHeldFrames;
    public int specialHeldFrames;
    public int jumpHeldFrames;
    public int shieldHeldFrames;
    public int grabHeldFrames;

    public int directionHeldFrames;
    internal int parryHeldFrames;
    internal bool parryReleased;
    internal bool parryPressed;
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Button
{
    Attack,
    Special,
    Jump,
    Shield,
    Grab,
    Parry
}

public enum Direction
{
    Neutral,
    Left,
    Right,
    Up,
    Down
}

public struct ButtonState
{
    public bool held;
    public bool pressed;
    public bool released;
    public int heldFrames;
}

public struct StickState
{
    public Vector2 raw;
    public Direction direction;
    public int directionHeldFrames;
}

public class NeoInputReader : MonoBehaviour
{
    private const int HISTORY_SIZE = 16;

    [Header("Stick Settings")]
    [SerializeField] private float deadzone = 0.2f;
    [SerializeField] private float directionThreshold = 0.65f;
    [SerializeField] private float directionReleaseThreshold = 0.50f;
    
    private int currentFrame = 0;

    public int CurrentFrame => currentFrame;

    private InputSnapshot[] history =
        new InputSnapshot[HISTORY_SIZE];

    private int historyIndex;

    private Vector2 leftStick;
    private Vector2 rightStick;

    private bool attack;
    private bool special;
    private bool jump;
    private bool shield;
    private bool grab;

    private InputDirection direction;

    private int attackHeldFrames;
    private int specialHeldFrames;
    private int jumpHeldFrames;
    private int shieldHeldFrames;
    private int grabHeldFrames;
    private int directionHeldFrames;

    private void Update()
    {
        UpdateFrame();
    }

    // =========================================================
    // INPUT SYSTEM
    // =========================================================

    public void OnMove(InputAction.CallbackContext context)
    {
        leftStick = context.ReadValue<Vector2>();
    }

    public void OnRightStick(InputAction.CallbackContext context)
    {
        rightStick = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        attack = context.ReadValueAsButton();
    }

    public void OnSpecial(InputAction.CallbackContext context)
    {
        special = context.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jump = context.ReadValueAsButton();
    }

    public void OnShield(InputAction.CallbackContext context)
    {
        shield = context.ReadValueAsButton();
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        grab = context.ReadValueAsButton();
    }

    // =========================================================
    // FRAME UPDATE
    // =========================================================

    private void UpdateFrame()
    {
        currentFrame++;

        InputSnapshot previous = history[historyIndex];

        InputSnapshot current = new InputSnapshot();

        current.frame = currentFrame;

        current.leftStick = leftStick;
        current.rightStick = rightStick;

        current.direction = GetDirection(
            leftStick,
            previous.direction
        );

        current.attack = attack;
        current.special = special;
        current.jump = jump;
        current.shield = shield;
        current.grab = grab;

        current.attackPressed =
            attack && !previous.attack;

        current.specialPressed =
            special && !previous.special;

        current.jumpPressed =
            jump && !previous.jump;

        current.shieldPressed =
            shield && !previous.shield;

        current.grabPressed =
            grab && !previous.grab;

        current.attackReleased =
            !attack && previous.attack;

        current.specialReleased =
            !special && previous.special;

        current.jumpReleased =
            !jump && previous.jump;

        current.shieldReleased =
            !shield && previous.shield;

        current.grabReleased =
            !grab && previous.grab;

        current.attackHeldFrames =
            attack
                ? previous.attackHeldFrames + 1
                : 0;

        current.specialHeldFrames =
            special
                ? previous.specialHeldFrames + 1
                : 0;

        current.jumpHeldFrames =
            jump
                ? previous.jumpHeldFrames + 1
                : 0;

        current.shieldHeldFrames =
            shield
                ? previous.shieldHeldFrames + 1
                : 0;

        current.grabHeldFrames =
            grab
                ? previous.grabHeldFrames + 1
                : 0;

        current.directionHeldFrames =
            current.direction == previous.direction
                ? previous.directionHeldFrames + 1
                : 1;

        historyIndex =
            (historyIndex + 1) % HISTORY_SIZE;

        history[historyIndex] = current;
    }
    
    public int FindPressedFrame(
        GameInput input,
        int window)
    {
        window = Mathf.Min(window, HISTORY_SIZE);

        for (int i = 0; i < window; i++)
        {
            InputSnapshot snapshot = GetSnapshot(i);

            if (GetButtonPressed(snapshot, input))
            {
                return snapshot.frame;
            }
        }

        return -1;
    }
    
    public int FindDirectionFrame(
        InputDirection direction,
        int window)
    {
        window = Mathf.Min(window, HISTORY_SIZE);

        for (int i = 0; i < window; i++)
        {
            InputSnapshot snapshot = GetSnapshot(i);

            if (snapshot.direction == direction)
            {
                return snapshot.frame;
            }
        }

        return -1;
    }

    // =========================================================
    // DIRECTION
    // =========================================================

    private InputDirection GetDirection(
        Vector2 stick,
        InputDirection previous)
    {
        float magnitude = stick.magnitude;

        if (magnitude < deadzone)
        {
            return InputDirection.Neutral;
        }

        float threshold =
            previous == InputDirection.Neutral
                ? directionThreshold
                : directionReleaseThreshold;

        float x = stick.x;
        float y = stick.y;

        // Continue existing direction.
        if (previous == InputDirection.Left &&
            x < -threshold)
        {
            return InputDirection.Left;
        }

        if (previous == InputDirection.Right &&
            x > threshold)
        {
            return InputDirection.Right;
        }

        if (previous == InputDirection.Up &&
            y > threshold)
        {
            return InputDirection.Up;
        }

        if (previous == InputDirection.Down &&
            y < -threshold)
        {
            return InputDirection.Down;
        }

        // Find a new direction.
        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            if (x > directionThreshold)
                return InputDirection.Right;

            if (x < -directionThreshold)
                return InputDirection.Left;
        }
        else
        {
            if (y > directionThreshold)
                return InputDirection.Up;

            if (y < -directionThreshold)
                return InputDirection.Down;
        }

        return InputDirection.Neutral;
    }

    // =========================================================
    // HISTORY
    // =========================================================

    public InputSnapshot GetSnapshot(int framesAgo)
    {
        if (framesAgo < 0 ||
            framesAgo >= HISTORY_SIZE)
        {
            return new InputSnapshot();
        }

        int index =
            historyIndex - 1 - framesAgo;

        if (index < 0)
        {
            index += HISTORY_SIZE;
        }

        return history[index];
    }

    // =========================================================
    // BUTTON QUERIES
    // =========================================================

    public bool IsHeld(GameInput input)
    {
        return GetButton(GetSnapshot(0), input);
    }

    public bool WasPressed(GameInput input)
    {
        return GetButtonPressed(GetSnapshot(0), input);
    }

    public bool WasReleased(GameInput input)
    {
        return GetButtonReleased(GetSnapshot(0), input);
    }

    public int HeldFrames(GameInput input)
    {
        return GetHeldFrames(GetSnapshot(0), input);
    }

    // =========================================================
    // GENEROUS INPUT QUERIES
    // =========================================================

    public bool WasPressed(GameInput input, int window)
    {
        window = Mathf.Min(window, HISTORY_SIZE);

        for (int i = 0; i < window; i++)
        {
            if (GetButtonPressed(GetSnapshot(i), input))
            {
                return true;
            }
        }

        return false;
    }

    public bool WasHeld(GameInput input, int window)
    {
        window = Mathf.Min(window, HISTORY_SIZE);

        for (int i = 0; i < window; i++)
        {
            if (GetButton(GetSnapshot(i), input))
            {
                return true;
            }
        }

        return false;
    }

    public bool WasDirection(
        InputDirection wantedDirection,
        int window)
    {
        window = Mathf.Min(window, HISTORY_SIZE);

        for (int i = 0; i < window; i++)
        {
            if (GetSnapshot(i).direction ==
                wantedDirection)
            {
                return true;
            }
        }

        return false;
    }

    // =========================================================
    // BUTTON HELPERS
    // =========================================================

    private bool GetButton(
        InputSnapshot snapshot,
        GameInput input)
    {
        switch (input)
        {
            case GameInput.Attack:
                return snapshot.attack;

            case GameInput.Special:
                return snapshot.special;

            case GameInput.Jump:
                return snapshot.jump;

            case GameInput.Shield:
                return snapshot.shield;

            case GameInput.Grab:
                return snapshot.grab;
        }

        return false;
    }
    
    public bool WasDirectionNearFrame(
        InputDirection direction,
        int targetFrame,
        int window)
    {
        for (int i = 0; i < HISTORY_SIZE; i++)
        {
            InputSnapshot snapshot =
                GetSnapshot(i);

            if (snapshot.frame == 0)
            {
                continue;
            }

            int difference =
                Mathf.Abs(snapshot.frame - targetFrame);

            if (difference > window)
            {
                continue;
            }

            if (snapshot.direction == direction)
            {
                return true;
            }
        }

        return false;
    }

    private bool GetButtonPressed(
        InputSnapshot snapshot,
        GameInput input)
    {
        switch (input)
        {
            case GameInput.Attack:
                return snapshot.attackPressed;

            case GameInput.Special:
                return snapshot.specialPressed;

            case GameInput.Jump:
                return snapshot.jumpPressed;

            case GameInput.Shield:
                return snapshot.shieldPressed;

            case GameInput.Grab:
                return snapshot.grabPressed;
        }

        return false;
    }

    private bool GetButtonReleased(
        InputSnapshot snapshot,
        GameInput input)
    {
        switch (input)
        {
            case GameInput.Attack:
                return snapshot.attackReleased;

            case GameInput.Special:
                return snapshot.specialReleased;

            case GameInput.Jump:
                return snapshot.jumpReleased;

            case GameInput.Shield:
                return snapshot.shieldReleased;

            case GameInput.Grab:
                return snapshot.grabReleased;
        }

        return false;
    }

    private int GetHeldFrames(
        InputSnapshot snapshot,
        GameInput input)
    {
        switch (input)
        {
            case GameInput.Attack:
                return snapshot.attackHeldFrames;

            case GameInput.Special:
                return snapshot.specialHeldFrames;

            case GameInput.Jump:
                return snapshot.jumpHeldFrames;

            case GameInput.Shield:
                return snapshot.shieldHeldFrames;

            case GameInput.Grab:
                return snapshot.grabHeldFrames;
        }

        return 0;
    }

    // =========================================================
    // STICK QUERIES
    // =========================================================

    public Vector2 GetLeftStick()
    {
        return leftStick;
    }

    public Vector2 GetRightStick()
    {
        return rightStick;
    }

    public InputDirection GetDirection()
    {
        return direction;
    }

    public int GetDirectionHeldFrames()
    {
        return GetSnapshot(0).directionHeldFrames;
    }
}
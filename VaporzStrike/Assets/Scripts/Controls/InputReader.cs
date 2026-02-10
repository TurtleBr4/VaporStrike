using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputType
{
    left, right, up, down, attack, special, parry, block
}
public class InputReader : MonoBehaviour
{
    [Header("Basic Input")] 
    public int inputBufferFrames = 4; //the amount of time (in frames) the player has to input something and have it be counted
    private InputType[] inputBuffer; //the actual storage of inputs
    public PlayerInput playerInput;
    public int framesSinceLastInput = 0;
    [Header("Required References")]
    private GenericCharacter character;
    private GameManager gameManager;

    void Start()
    {
        inputBuffer = new InputType[inputBufferFrames];
        character = GetComponent<GenericCharacter>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        framesSinceLastInput++;

        if (framesSinceLastInput > inputBufferFrames)
            ClearBuffer();
    }

    void PushInput(InputType input)
    {
        framesSinceLastInput = 0;

        for (int i = inputBuffer.Length - 1; i > 0; i--)
            inputBuffer[i] = inputBuffer[i - 1];

        inputBuffer[0] = input;
    }

    void ClearBuffer()
    {
        Array.Clear(inputBuffer, 0, inputBuffer.Length);
    }

    public bool CheckInputSequence(InputType[] sequence)
    {
        if (sequence.Length > inputBuffer.Length)
            return false;

        for (int i = 0; i < sequence.Length; i++)
        {
            if (inputBuffer[i] != sequence[i])
                return false;
        }

        return true;
    }

    public InputType[] GetBuffer() => inputBuffer;

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        Vector2 dir = ctx.ReadValue<Vector2>();

        if (dir.x > 0.5f)
            PushInput(InputType.right);
        else if (dir.x < -0.5f)
            PushInput(InputType.left);

        if (dir.y > 0.5f)
            PushInput(InputType.up);
        else if (dir.y < -0.5f)
            PushInput(InputType.down);
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            PushInput(InputType.attack);
    }

    public void OnSpecial(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            PushInput(InputType.special);
    }

    public void OnParry(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            PushInput(InputType.parry);
    }

    public void OnBlock(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            PushInput(InputType.block);
    }
}

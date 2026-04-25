using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputType
{
    neutral, left, right, up, down, attack, special, parry, block, grab
}
public class InputReader : MonoBehaviour
{
    [Header("Basic Input")] 
    public int inputBufferFrames = 8; //the amount of time (in frames) the player has to input something and have it be counted
    [SerializeField]private InputType[] inputBuffer; //the actual storage of inputs
    public PlayerInput playerInput;
    public int framesSinceLastInput = 0;
    private Vector2 moveInput;
    [Header("Required References")]
    private GenericCharacter character;
    private GameManager gameManager;
    public string controllerID;
    public string playerID;

    public bool tapJumpPressed;
    public bool jumpPressed;
    private Vector2 previousMoveInput;

    //Max's debug code
    public bool printInputs;

    void Start()
    {
        inputBuffer = new InputType[inputBufferFrames];
        character = GetComponent<GenericCharacter>();
        gameManager = FindFirstObjectByType<GameManager>();

        //Max's debug code
        printInputs = false;
    }

    void Update()
    {
        framesSinceLastInput++;

        if (framesSinceLastInput > (inputBufferFrames))
        {
            clearFirstInputFromBuffer();
        }

        if (Input.GetKeyDown(KeyCode.P)) printInputs = !printInputs;
    }


    void PushInput(InputType input)
    {
        //Max's debug code
        if (printInputs) print(input.ToString());

        framesSinceLastInput = 0;

        for (int i = inputBuffer.Length - 1; i > 0; i--)
            inputBuffer[i] = inputBuffer[i - 1];

        inputBuffer[0] = input;

        //Max's debug code
        if(printInputs) debugPrintBuffer();
    }

    public void debugPrintBuffer()
    {
        string buffer = "Input Buffer: ";
        foreach(InputType i in inputBuffer)
        {
            buffer += i.ToString();
        }
        print(buffer);
    }

    public void ClearBuffer()
    {
        Array.Clear(inputBuffer, 0, inputBuffer.Length);
    }

    public void clearFirstInputFromBuffer()
    {
        for (int i = 0; i < inputBuffer.Length; i++)
        {
            if (i + 1 < inputBuffer.Length)
            {
                inputBuffer[i] = inputBuffer[i + 1];

            }
        }
        inputBuffer[inputBuffer.Length - 1] = InputType.neutral;
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

    public Vector2 pollMovementInput()
    {
        return moveInput;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {

        Vector2 dir = ctx.ReadValue<Vector2>();

        // Tap jump edge detection
        if (dir.y > 0.7f && previousMoveInput.y <= 0.7f)
            tapJumpPressed = true;

        // Directional buffer for specials
        if (dir.x > 0.5f) PushInput(InputType.right);
        else if (dir.x < -0.5f) PushInput(InputType.left);

        if (dir.y > 0.5f) PushInput(InputType.up);
        else if (dir.y < -0.5f) PushInput(InputType.down);

        moveInput = dir;
        previousMoveInput = dir;
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
    public void OnGrab(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            PushInput(InputType.grab);
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            jumpPressed = true;
    }

}

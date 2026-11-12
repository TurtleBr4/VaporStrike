
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("State Management")]
    public PlayerStateController psc;

    [Header("Data")]
    [SerializeField] private PlayerData playerData;

    [Header("Movement")]
    [SerializeField] private NeoInputReader inputReader;
    [SerializeField] private float gravity = 30f;

    [Header("AI Data")]
    private AiAgent aiController;
    private Vector2 aiMovementInput;
    private bool aiJumpRequested;

    [Header("Grab Position")]
    [SerializeField] private Transform thePlaceWhereYouStayGrabbed;

    private Vector2 knockbackVelocity;
    private bool inKnockback;

    private int timer = 100;
    private int anotherTimer = 100;

    private bool lastFrameGrounded;
    public bool lastFrameDisplacement = false;

    public bool dropping = false;

    private int numberThatTicksUpToCalculateWhenTheJustJumpedVariableShouldBeReset = 10;
    private int hitStunTimer = -1;

    private CharacterController controller;
    private float verticalVelocity;

    private void Awake()
    {
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        psc = GetComponent<PlayerStateController>();

        psc.setActionability(true); //temp

        aiController = GetComponent<AiAgent>();
    }

    private void Update()
    {
        if (psc.getSuccessfulGrab())
        {
            psc.setActionability(false);
        }

        if (psc.getActionablility())
        {
            Move();
        }

        if (psc.getHitstun())
        {
            ApplyKnockback();

            hitStunTimer--;

            if (hitStunTimer <= 0)
            {
                psc.setHitstun(false);
                hitStunTimer = -1;
                inKnockback = false;
            }
        }

        if (timer != 100)
        {
            timer -= 1;
        }

        if (anotherTimer != 100)
        {
            anotherTimer -= 1;
        }

        Jump();
        CheckDropping();
        ApplyGravity();

        lastFrameGrounded = psc.getGrounded();
        psc.setGrounded(controller.isGrounded);

        if (!lastFrameGrounded && psc.getGrounded())
        {
            psc.justLanded = true;
            anotherTimer = playerData.landingLength;
            psc.setActionability(false);
        }

        if (anotherTimer <= 0)
        {
            psc.justLanded = false;
            psc.setActionability(true);
            anotherTimer = 100;
        }
    }

    private void CheckDropping()
    {
        Vector2 stick = GetMovementInput();

        dropping = stick.y < -0.5f;
    }

    private void Move()
    {
        Vector2 stick = GetMovementInput();

        Vector3 movement = Vector3.right * stick.x * playerData.moveSpeed;

        if (movement.x >= 0.2f || movement.x <= -0.2f)
        {
            psc.setMovin(true);

            if (movement.x > 0)
            {
                psc.setFacingRight(true);
            }
            else
            {
                psc.setFacingRight(false);
            }
        }
        else
        {
            psc.setMovin(false);
        }

        controller.Move(movement * Time.deltaTime);
    }

    public void TakeKnockback(Vector2 movement, int framesOfInaction, float currentPercent)
    {
        hitStunTimer = framesOfInaction;

        knockbackVelocity = movement * Mathf.Clamp(currentPercent - 10, 1, 10);

        inKnockback = true;

        psc.setFacingRight(movement.x > 0);
        psc.setHitstun(true);
    }

    public void DoDisplacement(Vector2 direction, float distance)
    {
        if (controller == null)
            return;

        Vector3 movement = new Vector3(
            direction.x,
            direction.y,
            0f
        );

        controller.Move(movement * distance);
    }

    private void ApplyKnockback()
    {
        if (!inKnockback)
            return;

        controller.Move(
            new Vector3(knockbackVelocity.x, knockbackVelocity.y, 0f)
            * Time.deltaTime
        );
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;

            verticalVelocity = Mathf.Max(
                verticalVelocity,
                -playerData.maxFallSpeed
            );
        }

        Vector3 gravityMovement =
            Vector3.up * verticalVelocity * Time.deltaTime;

        controller.Move(gravityMovement);
    }

    private void Jump()
    {
        if (!psc.getGrounded())
        {
            numberThatTicksUpToCalculateWhenTheJustJumpedVariableShouldBeReset--;

            if (numberThatTicksUpToCalculateWhenTheJustJumpedVariableShouldBeReset <= 0)
            {
                psc.justJumped = false;
            }

            return;
        }

        bool isAI = aiController != null && aiController.enabled;

        bool jumpButton;

        if (isAI)
        {
            jumpButton = aiJumpRequested;
            aiJumpRequested = false;
        }
        else
        {
            jumpButton = inputReader.WasPressed(GameInput.Jump, 4);
        }

        bool tapJump =
            !isAI &&
            !psc.getDisplacement() &&
            inputReader.GetDirection() == InputDirection.Up &&
            inputReader.GetDirectionHeldFrames() == 1;

        if ((jumpButton || tapJump) && psc.getActionablility())
        {
            timer = playerData.jumpSquatLength;

            psc.justJumped = true;
        }

        if (timer <= 0)
        {
            verticalVelocity = playerData.jumpPower;

            numberThatTicksUpToCalculateWhenTheJustJumpedVariableShouldBeReset = 10;

            timer = 100;
        }

        return;
    }

    public void SetAIMovementInput(Vector2 input)
    {
        aiMovementInput = Vector2.ClampMagnitude(input, 1f);
    }

    public void RequestAIJump()
    {
        aiJumpRequested = true;
    }

    private Vector2 GetMovementInput()
    {
        if (aiController != null && aiController.enabled)
        {
            return aiMovementInput;
        }

        return inputReader.GetLeftStick();
    }

    public void takeInGrabPosition(Transform t)
    {
        thePlaceWhereYouStayGrabbed = t;
    }
}

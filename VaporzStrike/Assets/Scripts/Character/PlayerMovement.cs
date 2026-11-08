using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("State Management")]
    public PlayerStateController psc;

    [Header("Data")]
    [SerializeField] private Player player;

    [Header("Movement")]
    [SerializeField] private NeoInputReader inputReader;
    [SerializeField] private float moveSpeed = 8f;

    [Header("AI Data")]
    private AiAgent aiController;
    private Vector2 aiMovementInput;
    private bool aiJumpRequested;

    [Header("Animation Movement Override")]
    [SerializeField] private Transform animatedModel;

    private bool animationMovementOverride;
    private Vector3 previousModelWorldPosition;


    [Header("OldJump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravity = 30f;
    [SerializeField] private float maxFallSpeed = 20f;
    [SerializeField] private int jumpSquatLength;
    [SerializeField] private int landingLength;

    private Vector2 knockbackVelocity;
    private bool inKnockback;

    int timer = 100;
    int anotherTimer = 100;
    bool lastFrameGrounded;
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
        if (!animationMovementOverride)
        {
            if(psc.getDisplacement() && !lastFrameDisplacement)
            {
                lastFrameDisplacement = true;
                BeginAnimationMovementOverride();
            }

            if(!psc.getDisplacement() && lastFrameDisplacement)
            {
                lastFrameDisplacement = false;
                EndAnimationMovementOverride();
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
        }

        lastFrameGrounded = psc.getGrounded();
        psc.setGrounded(controller.isGrounded);

        if (!lastFrameGrounded && psc.getGrounded())
        {
            psc.justLanded = true;
            anotherTimer = landingLength;
            psc.setActionability(false);
        }

        if (anotherTimer <= 0)
        {
            psc.justLanded = false;
            psc.setActionability(true);
            anotherTimer = 100;
        }
    }


    private void LateUpdate()
    {
        if (!animationMovementOverride || animatedModel == null)
            return;

        Vector3 currentPosition = animatedModel.position;
        Vector3 movement = currentPosition - previousModelWorldPosition;

        movement.z = 0f;

        // Instantly reposition the player instead of using CharacterController.Move().
        transform.position += movement;

        // Update the reference position after moving the parent.
        previousModelWorldPosition = animatedModel.position;
    }

    // Call when a move should control player movement through animation displacement.
    public void BeginAnimationMovementOverride()
    {
        if (animatedModel == null)
            return;

        animationMovementOverride = true;
        previousModelWorldPosition = animatedModel.position;
    }

    public void EndAnimationMovementOverride()
    {
        animationMovementOverride = false;
    }


    private void CheckDropping()
    {
        Vector2 stick = GetMovementInput();

        dropping = stick.y < -0.5f;
    }

    private void Move()
    {
        Vector2 stick = GetMovementInput();

        Vector3 movement = Vector3.right * stick.x * moveSpeed;

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

    public void TakeKnockback(Vector2 movement, int framesOfInaction)
    {
        hitStunTimer = framesOfInaction;
        knockbackVelocity = movement;
        inKnockback = true;

        psc.setFacingRight(movement.x > 0);
        psc.setHitstun(true);
    }

    private void ApplyKnockback()
    {
        if (!inKnockback)
        {
            return;
        }

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
                -maxFallSpeed
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
            inputReader.GetDirection() == InputDirection.Up &&
            inputReader.GetDirectionHeldFrames() == 1;

        if ((jumpButton || tapJump) && psc.getActionablility())
        {
            timer = jumpSquatLength;

            psc.justJumped = true;
        }

        if (timer <= 0)
        {
            verticalVelocity = jumpForce;

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
}
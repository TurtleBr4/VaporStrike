using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private NeoInputReader inputReader;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravity = 30f;
    [SerializeField] private float maxFallSpeed = 20f;

    public bool dropping = false;

    private CharacterController controller;

    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CheckDropping();

        Move();
        ApplyGravity();
        Jump();
    }

    private void CheckDropping()
    {
        Vector2 stick = inputReader.GetLeftStick();

        dropping = stick.y < -0.5f;
    }
    private void Move()
    {
        Vector2 stick = inputReader.GetLeftStick();

        Vector3 movement =
            Vector3.right * stick.x * moveSpeed;

        controller.Move(
            movement * Time.deltaTime
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
            verticalVelocity -=
                gravity * Time.deltaTime;

            verticalVelocity =
                Mathf.Max(
                    verticalVelocity,
                    -maxFallSpeed
                );
        }

        Vector3 gravityMovement =
            Vector3.up *
            verticalVelocity *
            Time.deltaTime;

        controller.Move(gravityMovement);
    }
    private void Jump()
    {
        if (!controller.isGrounded)
        {
            return;
        }

        bool jumpButton =
            inputReader.WasPressed(GameInput.Jump);

        bool tapJump =
            inputReader.GetDirection() == InputDirection.Up &&
            inputReader.GetDirectionHeldFrames() == 1;

        if (!jumpButton && !tapJump)
        {
            return;
        }

        verticalVelocity = jumpForce;
    }
}
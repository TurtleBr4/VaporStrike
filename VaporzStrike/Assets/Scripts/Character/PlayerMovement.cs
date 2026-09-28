using UnityEngine;

public class PlayerMovement : MonoBehaviour
{


    [Header("State Management")]
    public PlayerStateController psc;

    [Header("Movement")]
    [SerializeField] private NeoInputReader inputReader;
    [SerializeField] private float moveSpeed = 8f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravity = 30f;
    [SerializeField] private float maxFallSpeed = 20f;
    [SerializeField] private int jumpSquatLength;
    [SerializeField] private int landingLength;
    int timer = 100;
    int anotherTimer = 100;
    bool lastFrameGrounded;
    public bool dropping = false;
    private int numberThatTicksUpToCalculateWhenTheJustJumpedVariableShouldBeReset = 10;

    private CharacterController controller;

    private float verticalVelocity;

    private void Awake()
    {

    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        psc = GetComponent<PlayerStateController>();
        psc.setActionability(true); //temporary, will be set from the new game manager whenever i get around to doing that
    }

    private void Update()
    {

        if (psc.getActionablility())
        {
            Move();

        }

        if(timer != 100) //fuck you, vague variables upon ye
        {
            timer -= 1;
        }
        if(anotherTimer != 100)
        {
            anotherTimer -= 1;
        }


        Jump();
        CheckDropping();
        ApplyGravity();
        lastFrameGrounded = psc.getGrounded();
        psc.setGrounded(controller.isGrounded);

        if(!lastFrameGrounded && psc.getGrounded())
        {
            psc.justLanded = true;
            anotherTimer = landingLength;
            psc.setActionability(false);
        }

        if(anotherTimer <= 0)
        {
            psc.justLanded = false;
            psc.setActionability(true);
            anotherTimer = 100;
        }
       

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

        if(movement.x >= .2f || movement.x <= -.2f)
        {
            psc.setMovin(true);
            if(movement.x > 0)
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

        if (!psc.getGrounded())
        {
            numberThatTicksUpToCalculateWhenTheJustJumpedVariableShouldBeReset -= 1;
            if(numberThatTicksUpToCalculateWhenTheJustJumpedVariableShouldBeReset <= 0)
            {
                psc.justJumped = false;
            }


            return;
        }



        bool jumpButton =
            inputReader.WasPressed(GameInput.Jump, 4);

        //print(jumpButton);

        bool tapJump =
            inputReader.GetDirection() == InputDirection.Up &&
            inputReader.GetDirectionHeldFrames() == 1;

        if((jumpButton || tapJump) && psc.getActionablility())
        {

            timer = jumpSquatLength; //set a timer to account for jumpsquat

            psc.justJumped = true;
            //psc.setActionability(false);

        }

        if(timer <= 0)
        {
            verticalVelocity = jumpForce;
            numberThatTicksUpToCalculateWhenTheJustJumpedVariableShouldBeReset = 10;
            //psc.setActionability(true);
            timer = 100;
        }

        return;
    }
}
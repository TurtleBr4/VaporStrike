using UnityEngine;
using System.Collections;
using System.Linq;
public enum playerState
{
    actionable, attacking, blocking, falling, hitstun, blockstun, landlag, parry
}

public enum playerGroundState //all this does is track if a player is in the air or on the ground
{
    grounded, air
}

public class GenericCharacter : MonoBehaviour
{
    [Header("Static Character Variables")]
    [SerializeField] private string playerName;
    [SerializeField] private float weight;  //determines knockback, fall speed
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float airSpeed;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private Moveset moveset;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Runtime Character Variables")]
    [SerializeField] private float damagePercent = 0f;
    [SerializeField] private playerState currentState;
    [SerializeField] private playerGroundState currentGroundState;

    [Header("Constants")] 
    public const float minMovementStep = .5f;

    [Header("TEMPORARY MOVEMENT VARIABLES, DONT GET TOO ATTACHED!")]
    public bool isJumping;
    public bool isGrounded;
    public bool isActionable;
    public CharacterController c;
    public float groundCheckDistance = 0.15f;
    public float dropDuration = 0.25f;
    int oneWayLayer;
    public bool dropping;
    private Vector3 storedMomentum = Vector3.zero;
    private Vector3 movementInput;
    private Vector3 velocity;

    private void Awake()
    {
        oneWayLayer = LayerMask.NameToLayer("OneWayPlatform");
    }

    private void Start()
    {
        inputReader = GetComponent<InputReader>();
    }

    private void Update()
    {


        
        takeInput(readInputBuffer());
        doMovement();
        if (doGroundCheck())
        {
            currentGroundState = playerGroundState.grounded;
        }
        else
        {
            currentGroundState = playerGroundState.air;
        }

        doPlatformHandle();
    }

    Move readInputBuffer()
    {
       return moveset.GetMatchingMove(inputReader.GetBuffer());
    }

    public virtual void takeInput(Move move)
    {

        //Switches state based on move
        if(move.inputSequence.Contains<InputType>(InputType.attack) || move.inputSequence.Contains<InputType>(InputType.special))
            currentState = playerState.attacking;
        else if(move.inputSequence.Contains<InputType>(InputType.parry))
            currentState = playerState.parry;
        else if(move.inputSequence.Contains<InputType>(InputType.block))
            currentState = playerState.blocking;
        else 
            currentState = playerState.actionable;

        //switch (input)
        //{
        //    case InputType.attack:
        //        currentState = playerState.attacking;
        //        break;
        //    case InputType.special:
        //        currentState = playerState.attacking;
        //        doAttack();
        //        break;
        //    case InputType.parry:
        //        currentState = playerState.parry;
        //        break;
        //    case InputType.block:
        //        currentState = playerState.blocking;
        //        break;
        //    case InputType.neutral:
        //        break;
        //    default: //for movement
        //        currentState = playerState.actionable;
        //        break;
        //}

    }

    //I renamed this so it's less confusing when talking about moves
    public virtual void doMovement()
    {

        float x = inputReader.pollMovementInput().x;

        //deadzone
        if (Mathf.Abs(x) < 0.1f) x = 0f;

        Vector3 horizontal = new Vector3(x, 0f, 0f) * moveSpeed;


        //jump triggers only once per tap/button
        if (currentGroundState == playerGroundState.grounded && (inputReader.tapJumpPressed || inputReader.jumpPressed))
        {
            velocity.y = Mathf.Sqrt(jumpPower * 2f * weight);
            inputReader.tapJumpPressed = false;
            inputReader.jumpPressed = false;
        }

        //grav
        velocity.y += -weight * Time.deltaTime;
        if (velocity.y < -maxFallSpeed) velocity.y = -maxFallSpeed;

       
        Vector3 move = horizontal * Time.deltaTime;
        move.y = velocity.y * Time.deltaTime;

        c.Move(move);
    }

    public virtual bool doGroundCheck()
    {
        return Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
    }

    public virtual void doPlatformHandle()
    {
        
    }

    public virtual void doAttack()
    {
        Move move = moveset.GetMatchingMove(inputReader.GetBuffer());

        if (move)
        {
            //print("wahoo!");
            //PerformMove(move);
            inputReader.ClearBuffer(); // consume the inputs
        }
    }

    public virtual void doGetHit()
    {

    }

    public virtual void doParry()
    {

    }

    public virtual void doBlock()
    {

    }

    public virtual void doBlastZoneDeath()
    {

    }

    public virtual void doTakeDamage()
    {

    }


    IEnumerator DropThrough()
    {
        dropping = true;
        yield return new WaitForSeconds(dropDuration);
        dropping = false;
    }

    public bool ShouldIgnorePlatform(Collider platform)
    {
        if (!platform.gameObject.layer.Equals(oneWayLayer))
            return false;

        if (dropping)
            return true;

        // Player feet position
        Vector3 feet = transform.position + Vector3.up * c.skinWidth;

        // Is the platform above the player?
        return platform.bounds.min.y > feet.y;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (ShouldIgnorePlatform(hit.collider))
        {
            // Cancel upward collision
            
            if (hit.moveDirection.y > 0f)
            {
                Physics.IgnoreCollision(hit.collider, gameObject.GetComponent<Collider>(), true);
                velocity.y = Mathf.Min(velocity.y, 0f);
            }
            else
            {
                // Land normally
                Physics.IgnoreCollision(hit.collider, gameObject.GetComponent<Collider>(), false);
                if (hit.moveDirection.y < 0f)
                    velocity.y = 0f;
            }
        }
    }




}

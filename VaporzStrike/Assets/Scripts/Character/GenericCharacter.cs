using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;
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
    private static readonly int Property = Animator.StringToHash("Animation ID"); //thanks jetbrains rider for the effecient lookup or whatever

    [Header("Static Character Variables")]
    [SerializeField] private PlayerData playerData;
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
    [SerializeField] private Vector3 storedMomentum = Vector3.zero;
    [SerializeField] private Vector3 movementInput;
    [SerializeField] private Vector3 velocity;
    public TextMeshProUGUI perecentTxt; //auto asign in the future
    public bool dropping = false;
    public bool fastFalling = false;
    public bool isInAnimation = false;

    [Header("Constants and Dependencies")] 
    public CharacterController c;
    public Animator anim;
    public GameManager gameManager;
    public const float minMovementStep = .5f;
    public float ffMult;

    [Header("TEMPORARY MOVEMENT VARIABLES, DONT GET TOO ATTACHED!")]
    public bool isJumping;
    public bool isGrounded;
    public bool isActionable;

    public float groundCheckDistance = 0.15f;
    public float dropDuration = 0.25f;
    int oneWayLayer;
    

    private void Awake()
    {
        oneWayLayer = LayerMask.NameToLayer("OneWayPlatform");
    }

    private void Start()
    {
        inputReader = GetComponent<InputReader>();
        gameManager = FindFirstObjectByType<GameManager>();

        ffMult = gameManager.ffMult;
    }

    public void assignPlayerData(PlayerData p)
    {
        playerData = p;
        populateFromPlayerData();
    }

    private void populateFromPlayerData()
    {
        if (playerData)
        {
            moveset = playerData.moveset;
            moveSpeed  = playerData.moveSpeed;
            weight = playerData.weight;
            airSpeed = playerData.airSpeed;
            playerName = playerData.playerName;
            maxFallSpeed =  playerData.maxFallSpeed;
            jumpPower = playerData.jumpPower;
        }
        else
        {
            Debug.LogError("No PlayerData assigned!"); //rider shut up about performance challenge failed
        }
    }

    public void recieveCharacterPrefab(GameObject prefab)
    {

    }

    private void Update()
    {

        perecentTxt.text  = damagePercent.ToString() +"%";
        
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

        //doDownHandle();
    }

    Move readInputBuffer()
    {
       return moveset.GetMatchingMove(inputReader.GetBuffer());
    }

    public virtual void takeInput(Move move)
    {

        //Switches state based on move
        if (move)
        {
            if (move.inputSequence.Contains<InputType>(InputType.attack) ||
                move.inputSequence.Contains<InputType>(InputType.special))
            {
               doAttack(move);
            }
            else if(move.inputSequence.Contains<InputType>(InputType.parry))
                currentState = playerState.parry;
            else if (move.inputSequence.Contains<InputType>(InputType.block))
                currentState = playerState.blocking;
        }
        else
        {
            currentState = playerState.actionable;

        }
 
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
        float y = inputReader.pollMovementInput().y;

        //deadzone
        if (Mathf.Abs(x) < 0.1f) x = 0f;
        if (Mathf.Abs(y) < 0.1f)  y = 0f;

        Vector3 horizontal;

        if (currentGroundState == playerGroundState.air) //use the airspeed
        {
            horizontal = new Vector3(x, 0f, 0f) * airSpeed;
        }
        else
        {
            horizontal = new Vector3(x, 0f, 0f) * moveSpeed;
            fastFalling = false; //turned off the fast fall here because why not
        }



        //jump triggers only once per tap/button
        if (currentGroundState == playerGroundState.grounded && (inputReader.tapJumpPressed || inputReader.jumpPressed))
        {
            velocity.y = Mathf.Sqrt(jumpPower * 2f * weight);
            inputReader.tapJumpPressed = false;
            inputReader.jumpPressed = false;
        }
        
        //down check, using the raw input data instead of the buffer for rn
        if (y < -0.5f)
        {
            doDownHandle(true);
        }
        else
        {
            doDownHandle(false);
        }

        //grav
        if (currentGroundState != playerGroundState.grounded)
        {
            if (dropping)
            {
                fastFalling = true;
            }
            if (fastFalling)
            {
                velocity.y += -(weight * ffMult) * Time.deltaTime;
                if (velocity.y < -(maxFallSpeed * ffMult)) velocity.y = -(maxFallSpeed + 10); //i love arbitrary numbers!!!!
            }
            else
            {
                velocity.y += -weight * Time.deltaTime;
                if (velocity.y < -maxFallSpeed) velocity.y = -maxFallSpeed;
            }

        }
        
        Vector3 move = horizontal * Time.deltaTime;
        move.y = velocity.y * Time.deltaTime;

        c.Move(move);
        
        
    }

    public virtual bool doGroundCheck()
    {
        return Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
    }

    public virtual void doDownHandle(bool isDown) //handles fastfalling, platform dropthru
    {
        if (isDown)
        {
            dropping = true;
        }
        else
        {
            dropping = false;
        }
    }

    public virtual void doAttack(Move move)
    {

        if (move)
        {
            print(move.name);
            performMove(move.animationId);
            inputReader.ClearBuffer(); // consume the inputs
        }
    }

    public virtual void performMove(int animID)
    {
        if (anim)
        {
            isInAnimation = true;
            anim.SetInteger(Property, animID);
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
    
    #region Animator Flags

    public void StartAnimation()
    {
        isInAnimation = true;
    }

    public void StopAnimation()
    {
        isInAnimation = false;
    }

    public void CreateHitbox(Transform hitbox)
    {
        
    }

    public void DestroyHitbox()
    {
        
    }

    public void ModifyHurtbox()
    {
        
    }
    
    #endregion




}

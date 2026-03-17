using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;
public enum playerState
{
    actionable, attacking, blocking, falling, hitstun, blockstun, landlag, parry
}

public enum faceDirection
{
    left, right
}

public enum playerGroundState //all this does is track if a player is in the air or on the ground
{
    grounded, air
}

public class GenericCharacter : MonoBehaviour
{
    private static readonly int AnimationID = Animator.StringToHash("Animation ID"); //thanks jetbrains rider for the effecient lookup or whatever

    [Header("Static Character Variables")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private int playerID;
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
    [SerializeField] private int stockCount = 0;
    [SerializeField] private playerState currentState;
    [SerializeField] private playerGroundState currentGroundState;
    [SerializeField] private Vector3 storedMomentum = Vector3.zero;
    [SerializeField] private Vector3 movementInput;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private int frameTimer;
    [SerializeField] private faceDirection faceDirection;
    public TextMeshProUGUI perecentTxt; //auto assign in the future
    public bool dropping = false;
    public bool fastFalling = false;
    public bool isInAnimation = false;

    [Header("Constants and Dependencies")] 
    public CharacterController c;
    public Animator anim;
    public GameManager gameManager;
    public const float minMovementStep = .5f;
    public float ffMult; //fast fall multiplier
    public int pFrameTime;
    
    private void Start()
    {
        inputReader = GetComponent<InputReader>();
        gameManager = FindFirstObjectByType<GameManager>();
        faceDirection = faceDirection.right;
        getConstantsFromGameManager();
    }

    public void getConstantsFromGameManager()
    {
        pFrameTime = gameManager.parryFrameTime;
        ffMult = gameManager.ffMult;
        
    }

    public void assignPlayerData(PlayerData p, int pid)
    {
        playerData = p;
        playerID = pid;
        if (pid % 2 == 0)
        {
            faceDirection = faceDirection.left;
        }
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
            recieveCharacterPrefab(playerData.playerPrefab);
        }
        else
        {
            Debug.LogError("No PlayerData assigned!"); //rider shut up about performance challenge failed
        }
    }

    public void recieveCharacterPrefab(GameObject prefab)
    {
        GameObject guy = Instantiate(prefab, transform);
        guy.transform.position = transform.position;
        if(faceDirection == faceDirection.left) guy.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        anim = guy.GetComponent<Animator>();
    }

    private void debugInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) //simulate taking a hit
        {
            currentState = playerState.hitstun;
            doTakeKnockback(new Vector2(.5f,2), 20f);
            frameTimer = 36;
        }
    }

    public virtual void stateManager()
    {

        if (currentState == playerState.hitstun && frameTimer != 0)
        {
            frameTimer--;
        }
        else
        {
            currentState = playerState.actionable;
        }
    }
    public PlayerData GetPlayerData()
    {
        return playerData;
    }
    private void Update()
    {

        perecentTxt.text  = damagePercent.ToString() +"%";
        
        takeInput(readInputBuffer());
        debugInputs();
        stateManager();
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
        if (move && currentState != playerState.hitstun)
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
        else if (currentState == playerState.hitstun)
        {
            //here just in case
        }
        else
        {
            currentState = playerState.actionable;

        }
    }

    //I renamed this so it's less confusing when talking about moves
    public virtual void doMovement()
    {
        if (currentState != playerState.hitstun) //regular movement
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
            switchFaceDirection(move);
        }
        else //just got hit here
        {
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
        
            Vector3 move = velocity * Time.deltaTime;
            move.y += velocity.y * Time.deltaTime;

            c.Move(move);
        }
        
    }
    
    public virtual void switchFaceDirection(Vector3 moveDirection) //fix later
    {
        float rot = transform.eulerAngles.y;
        if (playerID % 2 == 0) //even players
        {
            if (moveDirection.x > 0)
            {
                rot += 180f;
                if(rot > 180f)  rot = 180f;
                faceDirection = faceDirection.right;
                transform.localEulerAngles = new Vector3(0f, rot, 0f);
            }
            else if (moveDirection.x < 0)
            {
                rot -= 180f;
                if(rot < 0f)  rot = 0f;
                faceDirection = faceDirection.left;
                transform.localEulerAngles = new Vector3(0f, rot, 0f);
            }
        }
        else
        {
            if (moveDirection.x > 0)
            {
                rot -= 180f;
                if(rot < 0f)  rot = 0f;
                faceDirection = faceDirection.right;
                transform.localEulerAngles = new Vector3(0f, rot, 0f);
            }
            else if (moveDirection.x < 0)
            {
                rot += 180f;
                if(rot > 180f)  rot = 180f;
                faceDirection = faceDirection.left;
                transform.localEulerAngles = new Vector3(0f, rot, 0f);
            }
        }
        
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
            anim.SetInteger(AnimationID, animID);
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

    public virtual void doTakeKnockback(Vector2 direction, float knockbackPower) //possibly redundant
    {
        velocity = (direction * knockbackPower);
    }

    public virtual void doTakeDamage(HitboxData hitbox)
    {
        currentState = playerState.hitstun;
        frameTimer = hitbox.hitStun;
        damagePercent += hitbox.damage;
        doTakeKnockback(hitbox.launchDirection, hitbox.knockback); 
    }
    
    #region Animator Flags

    public void StartAnimation(int animID)
    {
        isInAnimation = true;
        anim.SetInteger(AnimationID, animID);
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

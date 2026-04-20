using System;
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
    [SerializeField] private float baseShieldHP;
    [SerializeField] private Moveset moveset;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject[] hitboxAnchors;

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
    [SerializeField] private Move lastUsedMove;
    [SerializeField] bool isAttackHitboxActive = false;
    public TMP_Text perecentTxt; //auto assign in the future
    public bool dropping = false;
    public bool fastFalling = false;
    public bool isInAnimation = false;
    public bool hasHitAttack = false;
    public float shieldHP;
    public bool isShieldActive = false;
    public GameObject shield;

    [Header("Constants and Dependencies")] 
    public CharacterController c;
    public Animator anim;
    public GameManager gameManager;
    public HUD hud;
    public const float minMovementStep = .5f;
    public float ffMult; //fast fall multiplier
    public int pFrameTime;

    int playerStocks = 3;
    float drag = 1f;
    private void Start()
    {
        inputReader = GetComponent<InputReader>();
        gameManager = FindFirstObjectByType<GameManager>();
        faceDirection = faceDirection.right;
        hud = FindFirstObjectByType<HUD>();
        getConstantsFromGameManager();
        shield = GetComponentInChildren<Shield>().gameObject;
    }

    public void getConstantsFromGameManager()
    {
        pFrameTime = gameManager.parryFrameTime;
        ffMult = gameManager.fastFallMultiplier;
        
    }

    public int getPlayerID()
    {
        return playerID;
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

            hitboxAnchors = GetComponentsInChildren<Transform>()
                .Where(t => t.CompareTag("HitboxAnchor"))
                .Select(t => t.gameObject)
                .ToArray();
        }
        else
        {
            Debug.LogError("No PlayerData assigned!"); //rider shut up about performance challenge failed
        }
    }

    public void recieveCharacterPrefab(GameObject prefab)
    {
        print("Spawned Character Prefab");
        GameObject guy = Instantiate(prefab, transform);
        guy.transform.position = transform.position;
        if(faceDirection == faceDirection.left) guy.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        anim = guy.GetComponent<Animator>();
    }

    private void debugInputs()
    {
        if (gameManager.debugFlag)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) //simulate taking a hit
            {
                currentState = playerState.hitstun;
                doTakeKnockback(new Vector2(.5f,2), 20f);
                frameTimer = 36;
            }


        }
        
    }

    public virtual void stateManager()
    {

        if (currentState == playerState.hitstun && frameTimer != 0)
        {
            frameTimer--;
        }
        else if (currentState == playerState.attacking && frameTimer > 0)
        {
            int curFrame = lastUsedMove.moveTime - frameTimer;
            print(curFrame);
            if (curFrame == lastUsedMove.hitboxStartTime)
            {
                isAttackHitboxActive = true;
                print("starting hitbox");
            }
            if (curFrame == lastUsedMove.hitboxEndTime)
            {
                isAttackHitboxActive = false;
                print("ending hitbox");
            }

            if (isAttackHitboxActive)
            {
                handleAttack(lastUsedMove);
            }
            frameTimer--;
        }
        else if (currentState ==  playerState.blocking)
        {
            frameTimer = 6; //replace with a constant
            if (frameTimer != 0)
            {
                frameTimer--;
            }
            else
            {
                doBlock();
            }

        }
        else if (currentState == playerState.attacking && frameTimer ==0)
        {
            currentState = playerState.actionable;
            hasHitAttack = false;
            frameTimer = 0;
        }

        if (frameTimer < 0)
        {
            frameTimer = 0;
        }
    }
    public PlayerData GetPlayerData()
    {
        return playerData;
    }
    private void Update()
    {

        //perecentTxt.text  = damagePercent.ToString() +"%";
        
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
        updatePercentText();
        velocity *= drag;
    }

    public void updatePercentText()
    {
        if (playerID == 0)
        {
            hud.p1Percent.text = damagePercent.ToString();
            hud.p1Stocks.text = "x"+playerStocks.ToString();
            GameManager.instance.player1Obj = this;
        }
        else
        {
            hud.p2Percent.text = damagePercent.ToString();
            hud.p2Stocks.text = "x" + playerStocks.ToString();
            GameManager.instance.player2Obj = this;
        }
    }
    public int GetStocks()
    {
        return playerStocks;
    }
    Move readInputBuffer()
    {
       return moveset.GetMatchingMove(inputReader.GetBuffer());
    }

    public virtual void takeInput(Move move)
    {
        if(move == null)
        {
            //while no input detected
            
        }
        //Switches state based on move
        if (move && currentState != playerState.hitstun)
        { 

            if (move.inputSequence.Contains<InputType>(InputType.attack) ||
                move.inputSequence.Contains<InputType>(InputType.special))
            {
                Debug.Log("attacking!");
                startAttack(move);
            }
            else if (move.inputSequence.Contains<InputType>(InputType.parry))
            {
                currentState = playerState.parry;
                Debug.Log("parry");
            }
            else if (move.inputSequence.Contains<InputType>(InputType.block))
            {
                currentState = playerState.blocking;
                Debug.Log("blocking");
            }

        }
        else if (currentState == playerState.hitstun)
        {
            //here just in case
        }
        else if (currentState == playerState.attacking)
        {
            //do nothing 
        }
        else if (currentState == playerState.blocking)
        {
            //do nothing 
        }
        else
        {
            currentState = playerState.actionable;
            drag = 1f;

        }
    }

    //I renamed this so it's less confusing when talking about moves
    public virtual void doMovement()
    {
        if (currentGroundState == playerGroundState.grounded)
        {
            doBlock(); 

            if (inputReader.blockPressed)
                currentState = playerState.blocking;
        }
        //universal blocking for all chars

        if (currentState == playerState.blocking && !inputReader.blockPressed)
        {
            currentState = playerState.actionable;
            isShieldActive = false;
        }
        if (currentState == playerState.actionable) //regular movement
        {
            float x = inputReader.pollMovementInput().x;
            float y = inputReader.pollMovementInput().y;

            velocity.x = 0; //make sure we done have leftover velocity
            
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

        if (!Mathf.Approximately(transform.position.z, 4.5f))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 4.5f);
        }
        
    }
    
    public virtual void switchFaceDirection(Vector3 moveDirection) //fixed?
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

    public virtual void startAttack(Move move)
    {
        if (move)
        {
            lastUsedMove = move;
            currentState = playerState.attacking;
            frameTimer = move.moveTime;
            setMoveAnim(move.animationId);
            inputReader.ClearBuffer(); // consume the inputs
        }
    }

    public void handleAttack(Move m)
    {
        if (!m)
        {
            return;
        }

        if (m.hasProjectile)
        {
            GameObject proj = Instantiate(m.projectile);
            proj.GetComponent<Projectile>().initProjectile(m.hds, playerID, m.projectileSpeed, m.projectileLifetime);
            print("Spawned Projectile");
        }
        else
        {
            checkAndSendAttackInfo((createAttackHitbox(m)));
        }
    }

    public virtual void checkAndSendAttackInfo(Collider[] colliders)
    {
        if (colliders != null)
        {
            foreach (Collider col in colliders)
            {
                //print(col.name);
                if (col.CompareTag("Hurtbox"))
                {
                    GenericCharacter guy = col.GetComponentInParent<GenericCharacter>();
                    if (!guy.gameObject.Equals(gameObject) && !hasHitAttack) //implement multihits later
                    {
                        print("Hit an Enemy");
                        hasHitAttack = true;
                        guy.doTakeDamage(lastUsedMove.hds);
                    }
                }
            
            }
        }
    }

    public virtual Collider[] createAttackHitbox(Move move)
    {
        if (!hasHitAttack)
        {
            Transform location = hitboxAnchors[(int)move.hitboxLocation].transform;

            if (gameManager.debugFlag)
            {
                print("Spawned Hitbox at: " + location.name + ": " + location.position);
            }

            return Physics.OverlapSphere(location.position, move.radius, Physics.AllLayers,
                QueryTriggerInteraction.Collide);
        }

        return null;
    }


    public void OnDrawGizmosSelected() //visualizes the hitboxes while in the editor
    {
        Gizmos.color = new Color(1, 0, 0, 0.75F);
        if (lastUsedMove != null && currentState == playerState.attacking)
        {
            Gizmos.DrawSphere(hitboxAnchors[(int)lastUsedMove.hitboxLocation].transform.position, lastUsedMove.radius);
        }
    }

    public virtual void setMoveAnim(int animID)
    {
        if (anim)
        {
            isInAnimation = true;
            anim.SetInteger(AnimationID, animID);
        }
    }

    public virtual void doParry()
    {
        
    }

    public virtual void doBlock()
    {
        
       isShieldActive = (inputReader.blockPressed);
        
       shield.SetActive(isShieldActive);

       shieldHP += (isShieldActive) ? -1: 1;

        if(shieldHP < 0) { shieldHP = 0; }
        if (shieldHP > baseShieldHP) { shieldHP = baseShieldHP; }
        var shieldScale = shieldHP / 100f;
        shield.transform.localScale = new Vector3(3*shieldScale, 3 * shieldScale, 3 * shieldScale);    
    }

    public virtual void doBlastZoneDeath()
    {
        damagePercent = 0;
        playerStocks--;
        velocity = Vector3.zero;
    }

    public virtual void doTakeKnockback(Vector2 direction, float knockbackPower) //possibly redundant
    {
        if (!isShieldActive)
        {
            velocity = Vector3.zero;
            velocity = (direction * (knockbackPower + (damagePercent / 10) * 1.4f));
            drag = 0.95f;
        }
    }

    public virtual void doTakeDamage(HitboxDataStruct hitbox)
    {
        if (!isShieldActive)
        {
            currentState = playerState.hitstun;
            frameTimer = hitbox.hitStun;
            damagePercent += hitbox.damage;
            doTakeKnockback(hitbox.launchDirection, hitbox.knockback);
        }
        else
        {
            shieldHP -= hitbox.hitStun;
        }
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

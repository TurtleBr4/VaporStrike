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
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int LastAnimIdle = Animator.StringToHash("LastAnimIdle");
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int LastAnimID = Animator.StringToHash("Last Animation ID");
    private static readonly int StartRun = Animator.StringToHash("Start Run");


    public int lastAnimId = 0;
    private int currentAnim = 0;
    public bool runStartTracker = false;
    private bool isMoving = false;
    private bool wasMoving;

    [Header("Static Character Variables")]
    [SerializeField] private GameObject[] moveHitboxHolders;
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
    //[SerializeField] private GameObject[] hitboxAnchors;

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
    public bool isGrabbing = false;
    public bool isGrabbed = false;
    public bool isGrounded = true;
    private playerGroundState lastGroundState;
    
    public GameObject grabbox;
    [Header("Constants and Dependencies")] 
    public CharacterController c;
    public Animator anim;
    public GameManager gameManager;
    public HUD hud;
    public GameObject shield;
    public const float minMovementStep = .5f;
    public float ffMult; //fast fall multiplier
    public int pFrameTime;

    int playerStocks = 3;
    float drag = 1f;


    private void Start()
    {
        inputReader = GetComponent<InputReader>();
        gameManager = FindFirstObjectByType<GameManager>();
        //faceDirection = faceDirection.right;
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

    public faceDirection getFaceDir()
    {
        return faceDirection;
    }

    public void assignPlayerData(PlayerData p, int pid)
    {
        playerData = p;
        playerID = pid;
        if (pid % 2 != 0)
        {
            faceDirection = faceDirection.left;
        }
        else
        {
            faceDirection = faceDirection.right;
        }
        populateFromPlayerData();
    }
    
    private void populateFromPlayerData()
    {
        if (playerData)
        {
            Debug.Log("PlayerData found");
            moveset = playerData.moveset;
            moveSpeed  = playerData.moveSpeed;
            weight = playerData.weight;
            airSpeed = playerData.airSpeed;
            playerName = playerData.playerName;
            maxFallSpeed =  playerData.maxFallSpeed;
            jumpPower = playerData.jumpPower;
            if (faceDirection == faceDirection.left) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            //recieveCharacterPrefab(playerData.playerPrefab);

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
        /*
        if (gameManager.debugFlag)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) //simulate taking a hit
            {
                currentState = playerState.hitstun;
                doTakeKnockback(new Vector2(.5f,2), 20f);
                frameTimer = 36;
            }


        }
        */
    }

    public virtual void stateManager()
    {
        if (isGrabbed) { currentState = playerState.hitstun; }
        if (isGrabbing) { 
            currentState = playerState.attacking;
        }
        
        //clamp
        if (frameTimer < 0)
            frameTimer = 0;


        //hitstun
        if (currentState == playerState.hitstun)
        {
            if (frameTimer > 0)
                frameTimer--;

            if (frameTimer == 0)
                currentState = playerState.actionable;
        
        if (currentState == playerState.hitstun && frameTimer != 0)
        {
            frameTimer--;
        }
        //attack
        else if (currentState == playerState.attacking && !isGrabbing)
        {
            if (frameTimer > 0)
            {
                int curFrame = lastUsedMove.moveTime - frameTimer;

                if (curFrame == lastUsedMove.hitboxStartTime)
                {
                    isAttackHitboxActive = true;
                    print("starting hitbox");
                }

                if (curFrame >= lastUsedMove.hitboxEndTime)
                {
                    isAttackHitboxActive = false;
                    moveHitboxHolders[lastUsedMove.moveID].SetActive(false);
                    print("ending hitbox");
                }

                if (isAttackHitboxActive)
                {
                    handleAttack(lastUsedMove);

                    if (lastUsedMove.hasDisplacement)
                    {
                        doDisplacmentOverride(
                            lastUsedMove.displacementDirection,
                            lastUsedMove.displacementMagnitude
                        );
                    }
                }

                frameTimer--;
            }

            if (frameTimer == 0)
            {
                currentState = playerState.actionable;
                hasHitAttack = false;
                isAttackHitboxActive = false;
            }
        }
        //block
        else if (currentState == playerState.blocking)
        {
            if (frameTimer == 0)
                frameTimer = 6;

            frameTimer--;

            if (frameTimer <= 0)
            {
                doBlock();
                frameTimer = 0;
                currentState = playerState.actionable;
            }
        }
        //parry
        else if (currentState == playerState.parry)
        {
            if (frameTimer == 0)
                frameTimer = gameManager.parryFrameTime;

            frameTimer--;

            if (frameTimer <= 0)
            {
                frameTimer = 0;
                currentState = playerState.actionable;
            }
        }

        //action
        if (currentState == playerState.actionable)
        {
            setMoveAnim(0);
            foreach(GameObject g in moveHitboxHolders)
            {
                if(g != null)
                {
                    g.SetActive(false);
                }

            }
        }


        //another clamp bc lol
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
            anim.SetBool(Grounded, true);
            isGrounded = true;
        }
        else
        {
            currentGroundState = playerGroundState.air;
            anim.SetBool(Grounded, false);
            isGrounded = false;
        }

        if (currentGroundState != lastGroundState)
        {
            if (lastGroundState == playerGroundState.air) //Landing animation
            {
                StartAnimation(-2); //w magic numbers
                lastGroundState = currentGroundState;
            }
            else //Jumping animation
            {
                StartAnimation(-1); //w magic numbers
                lastGroundState = currentGroundState;
            }
        }

        //doDownHandle();
        updatePercentText();
        //velocity *= drag;
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
                if(currentState == playerState.actionable)
                {
                    Debug.Log("attacking!");
                    startAttack(move);
                }
                
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
        if (!isGrabbing && !inputReader.grabPressed && grabbox.activeInHierarchy)
        {
            currentState = playerState.actionable;
            isGrabbing = false;
            grabbox.SetActive(false);
        }
        if (currentState == playerState.actionable) //regular movement
        {
            float x = inputReader.pollMovementInput().x;
            float y = inputReader.pollMovementInput().y;

            velocity.x = 0; //make sure we done have leftover velocity

            //deadzone
            if (Mathf.Abs(x) < 0.25f) x = 0f;
            if (Mathf.Abs(y) < 0.25f) y = 0f;

            if (currentGroundState == playerGroundState.grounded)
            {
                bool isMoving = Mathf.Abs(x) > 0.1f;

                if (isMoving && !wasMoving)
                {
                    setMoveAnim(-3);
                    anim.SetTrigger(StartRun);
                }
                    
                if (isMoving && wasMoving)
                    setMoveAnim(-3);

                if (!isMoving && wasMoving)
                    setMoveAnim(0);

                wasMoving = isMoving;
            }
            
           

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



            //jump triggers only once per tap/button TAP JUMP IS OFF FOR THIS BUID, FIX LATER inputReader.tapJumpPressed
            if (currentGroundState == playerGroundState.grounded &&  inputReader.jumpPressed &&currentState == playerState.actionable)
            {
                setMoveAnim(-1); //w magic number 
                velocity.y = Mathf.Sqrt(jumpPower * 2f * weight);
                inputReader.tapJumpPressed = false;
                inputReader.jumpPressed = false;
            }

            //jump triggers only once per tap/button
            if (currentGroundState == playerGroundState.grounded && (inputReader.grabPressed))
            {
                inputReader.grabPressed = false;
                grabbox.SetActive(true);
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
            if (moveDirection.x < 0)
            {
                rot += 180f;
                if(rot > 180f)  rot = 180f;
                faceDirection = faceDirection.left;
                transform.localEulerAngles = new Vector3(0f, rot, 0f);
            }
            else if (moveDirection.x > 0)
            {
                rot -= 180f;
                if(rot < 0f)  rot = 0f;
                faceDirection = faceDirection.right;
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
            setMoveAnim(move.moveID);
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
            if (moveHitboxHolders[m.moveID] != null)
            {
                moveHitboxHolders[m.moveID].SetActive(true);

            }
            //checkAndSendAttackInfo((createAttackHitbox(m)));
        }
    }

    public void doDisplacmentOverride(Vector2 direction, float magnitude)
    {
        velocity = Vector3.zero;
        velocity = direction * magnitude;
        drag = 0.95f;
    }

    [Obsolete]
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
    public void GetGrabbed()
    {
        isGrabbed = true;
    }
    public void GrabRelease()
    {
        isGrabbed = false;
        currentState = playerState.actionable;
    }
    [Obsolete]/*
    public virtual Collider[] createAttackHitbox(Move move)
    {
        if (!hasHitAttack)
        {
            Transform location = hitboxAnchors[(int)move.hitboxLocation].transform;

            if (gameManager.debugFlag)
            {
                print("Spawned Hitbox at: " + location.name + ": " + location.position);
            }

            //return Physics.OverlapSphere(location.position, move.radius, Physics.AllLayers, QueryTriggerInteraction.Collide);
        }

        return null;
    }
    */


    public void OnDrawGizmosSelected() //visualizes the hitboxes while in the editor
    {
        Gizmos.color = new Color(1, 0, 0, 0.75F);
        if (lastUsedMove != null && currentState == playerState.attacking)
        {
            //Gizmos.DrawSphere(hitboxAnchors[(int)lastUsedMove.hitboxLocation].transform.position, lastUsedMove.radius);
        }
    }

    public virtual void setMoveAnim(int animID)
    {
        lastAnimId = currentAnim;

        if (anim && animID != currentAnim)
        {
            currentAnim = animID;
            isInAnimation = true;
            anim.SetInteger(LastAnimID, lastAnimId);
            anim.SetInteger(AnimationID, animID);
            
            if(lastAnimId == 0)
            {
                anim.SetBool(LastAnimIdle, true);
            }
            else
            {
                anim.SetBool(LastAnimIdle, false);
            }

            if(animID == -3 && runStartTracker == false)
            {
                if(lastAnimId != currentAnim)
                {
                    anim.SetBool(Running, true);
                    print("fuck you die");
                   
                    runStartTracker = true;
                }
                
            }
            else
            {
                print("fuck you, live");
                anim.SetBool(Running, false) ;
            }

            if(animID != -3)
            {
                runStartTracker = false;
            }
        }
        else {
            Debug.Log("Repeat Animaton");
            return; 
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
        if (!isShieldActive || currentState != playerState.parry)
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

    public bool getGroundState()
    {
        if (doGroundCheck())
        {
            return isGrounded;
        }
        else
        {
            return isGrounded;
        }
    }


    public virtual void doTakePercent(int amount)
    {
        if (!isShieldActive)
        {
            currentState = playerState.hitstun;
            damagePercent += amount;
        }
    }
    #region Animator Flags

    public void StartAnimation(int animID)
    {

        lastAnimId = anim.GetInteger(AnimationID);

        if (anim && animID != currentAnim)
        {
            currentAnim = animID;
            isInAnimation = true;
            anim.SetInteger(LastAnimID, lastAnimId);
            anim.SetInteger(AnimationID, animID);

            if (lastAnimId == 0)
            {
                anim.SetBool(LastAnimIdle, true);
            }
            else
            {
                anim.SetBool(LastAnimIdle, false);
            }

            if (animID == -3 && runStartTracker == false)
            {
                anim.SetBool(Running, true);
                print("fuck you die");
                anim.SetTrigger(StartRun);
                runStartTracker = true;
            }
            else
            {
                anim.SetBool(Running, false);
            }

            if (animID != -3)
            {
                runStartTracker = false;
            }
        }
        else
        {
            Debug.Log("Repeat Animaton");
            return;
        }
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

using UnityEngine;

public enum playerState
{
    actionable, attacking, blocking, falling, hitstun, blockstun, landlag
}

public class GenericCharacter : MonoBehaviour
{
    [Header("Static Character Variables")]
    [SerializeField] private string playerName;
    [SerializeField] private float weight;  //determines knockback, fall speed
    [SerializeField] private float moveSpeed;
    [SerializeField] private float airSpeed;
    [SerializeField] private Moveset moveset;

    [Header("Runtime Character Variables")]
    [SerializeField] private float damagePercent = 0f;
    [SerializeField] private playerState currentState;

    [Header("TEMPORARY MOVEMENT VARIABLES, DONT GET TOO ATTACHED!")]
    public bool isJumping;
    public bool isGrounded;
    public bool isActionable;
    public CharacterController c;
    private Vector3 storedMomentum = Vector3.zero;
    private Vector3 movementInput;
    private Vector3 velocity;


    private void Update()
    {
        doMove();
    }

    public virtual void doMove()
    {
        //this is just test code, max please change later k thx
        isJumping = Input.GetButtonDown("Jump");
        float pMoveX = Input.GetAxisRaw("Horizontal");
        float pMoveZ = Input.GetAxisRaw("Vertical");
        float fallSpeed = -weight;
        float jumpPower = airSpeed;

        Vector3 moveDir = new Vector3(pMoveX, pMoveZ).normalized;

        if (c.isGrounded)
        {
            isGrounded = true;
            isActionable = true;

            
        }
        else
        {
            isGrounded = false;
            isActionable = false;

            //keep momentum from takeoff in air
            movementInput = storedMomentum;
        }

        //update movement while grounded
        if (moveDir == Vector3.zero)
        {
            movementInput = Vector3.zero;
            storedMomentum = Vector3.zero;
        }
        else
        {
            movementInput = moveDir * moveSpeed;
            storedMomentum = movementInput;   //save last ground movement
        }

        if (isGrounded && isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpPower * -2f * fallSpeed);
        }

        velocity.y += fallSpeed * Time.deltaTime;

        Vector3 move = movementInput * Time.deltaTime;
        move.y = velocity.y * Time.deltaTime;

        c.Move(move);


    }

    public virtual void doAttack()
    {

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




}

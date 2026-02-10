using UnityEngine;
using System.Collections;
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
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private Moveset moveset;

    [Header("Runtime Character Variables")]
    [SerializeField] private float damagePercent = 0f;
    [SerializeField] private playerState currentState;

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

    private void Update()
    {
        doMove();
        doPlatformHandle();
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

        if (velocity.y < -maxFallSpeed) velocity.y = -maxFallSpeed;

        Vector3 move = movementInput * Time.deltaTime;
        move.y = velocity.y * Time.deltaTime;

        c.Move(move);


    }

    public virtual void doPlatformHandle()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Space)) //againt, debug code plz fix thx
        {
            StartCoroutine(DropThrough());
        }
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
        print("fuck");
        if (ShouldIgnorePlatform(hit.collider))
        {
            print("double fuck");
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

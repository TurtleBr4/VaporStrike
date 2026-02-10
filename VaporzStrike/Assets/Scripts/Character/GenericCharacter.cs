using UnityEngine;
using System.Collections;
public enum playerState
{
    actionable, attacking, blocking, falling, hitstun, blockstun, landlag, parry
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
    [SerializeField] private InputReader inputReader;

    [Header("Runtime Character Variables")]
    [SerializeField] private float damagePercent = 0f;
    [SerializeField] private playerState currentState;

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

    private void Update()
    {
        doMove(readInputBuffer());
        doPlatformHandle();
    }

    InputType readInputBuffer()
    {
       return inputReader.GetBuffer()[0];
    }

    public virtual void doMove(InputType input)
    {
        switch (input)
        {
            case InputType.attack:
                currentState = playerState.attacking;
                break;
            case InputType.special:
                currentState = playerState.attacking;
                break;
            case InputType.parry:
                currentState = playerState.parry;
                break;
            case InputType.block:
                currentState = playerState.blocking;
                break;
            default: //for movement
                break;
        }

    }

    public virtual void doPlatformHandle()
    {
        
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

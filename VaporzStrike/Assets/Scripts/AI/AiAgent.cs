
using UnityEngine;

public class AiAgent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player target;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerStateController psc;
    [SerializeField] private FighterMoves fighterMoves;
    [SerializeField] private InputBuffer inputBuffer;

    [Header("Behavior")]
    [SerializeField] private float approachDistance = 3f;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float attackCooldownMin = 0.8f;
    [SerializeField] private float attackCooldownMax = 1.5f;
    [SerializeField] private float jumpCooldownMin = 2f;
    [SerializeField] private float jumpCooldownMax = 5f;

    private float nextAttackTime;
    private float nextJumpTime;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (psc == null)
            psc = GetComponent<PlayerStateController>();

        if (fighterMoves == null)
            fighterMoves = GetComponent<FighterMoves>();

        if (inputBuffer == null)
            inputBuffer = GetComponent<InputBuffer>();
    }

    private void Start()
    {
        nextAttackTime = Time.time + 1f;
        nextJumpTime = Time.time + Random.Range(jumpCooldownMin, jumpCooldownMax);
    }

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
        }

        if (target == null)
        {
            playerMovement.SetAIMovementInput(Vector2.zero);
            return;
        }

        float difference = target.transform.position.x - transform.position.x;

        float distance = Mathf.Abs(difference);

        // Always face the opponent.
        psc.setFacingRight(difference >= 0f);

        // Do not make decisions while unable to act.
        if (!psc.getActionablility())
        {
            playerMovement.SetAIMovementInput(Vector2.zero);
            return;
        }

        //approach if too far away.
        if (distance > approachDistance)
        {
            playerMovement.SetAIMovementInput(new Vector2(Mathf.Sign(difference), 0f));
        }
        else
        {
            playerMovement.SetAIMovementInput(Vector2.zero);
        }

        //occasionally jump while grounded.
        if (psc.getGrounded() && Time.time >= nextJumpTime)
        {
            playerMovement.RequestAIJump();

            nextJumpTime = Time.time + Random.Range(jumpCooldownMin, jumpCooldownMax);
        }

        //attack when close enough.
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            TryAttack();

            nextAttackTime = Time.time + Random.Range(attackCooldownMin, attackCooldownMax);
        }
    }

    private void TryAttack()
    {
        MoveDefinition[] moves = fighterMoves.GetMoves();

        if (moves == null || moves.Length == 0)
            return;

        bool grounded = psc.getGrounded();

        InputDirection direction = InputDirection.Neutral;

        if (target.transform.position.x > transform.position.x)
        {
            direction = InputDirection.Right;
        }
        else if (target.transform.position.x < transform.position.x)
        {
            direction = InputDirection.Left;
        }

        //count moves the AI can currently use.
        int availableMoves = 0;

        for (int i = 0; i < moves.Length; i++)
        {
            MoveDefinition move = moves[i];

            if (move == null)
                continue;

            if (grounded && move.isAirMove)
                continue;

            if (!grounded && !move.isAirMove)
                continue;

            if (move.direction != InputDirection.Neutral && move.direction != direction)
                continue;

            availableMoves++;
        }

        if (availableMoves == 0)
            return;

        //choose one compatible move at random
        int choice = Random.Range(0, availableMoves);

        for (int i = 0; i < moves.Length; i++)
        {
            MoveDefinition move = moves[i];

            if (move == null)
                continue;

            if (grounded && move.isAirMove)
                continue;

            if (!grounded && !move.isAirMove)
                continue;

            if (move.direction != InputDirection.Neutral && move.direction != direction)
                continue;

            if (choice > 0)
            {
                choice--;
                continue;
            }

            //inject the move directly into the buffer
            inputBuffer.AddInput(
                move.button,
                move.direction
            );

            return;
        }
    }

    private void FindTarget()
    {
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);

        float closestDistance = float.MaxValue;

        foreach (Player candidate in players)
        {
            if (candidate == null || candidate.gameObject == gameObject)
                continue;

            float distance = Vector3.Distance(transform.position, candidate.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = candidate;
            }
        }
    }
}

using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Information")]
    [SerializeField] private string Name;
    [SerializeField] private float Percent;
    [SerializeField] private int playerID;
    [Header("References")]
    [SerializeField] PlayerStateController playerState;
    [SerializeField] PlayerMovement playerMove;
    [SerializeField] FighterMoves moves;

    public void takeDamage(float dmg, Vector2 direction, int hitstun)
    {
        Percent += dmg;
        playerMove.TakeKnockback(direction, hitstun);
    }

    public MoveDefinition getCurrentMove()
    {
        return moves.getCurrentMove();
    }

    public void takeInPlayerData(PlayerData data)
    {

    }

    public void doBlastZoneDeath()
    {

    }

    public int getPlayerID()
    {
        return playerID;
    }
}

using System;
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
    [SerializeField] private PlayerData playerData;

    private void Start()
    {
        if (playerData)
        {
            moves.grabboxStart = playerData.grabboxStartFrame;
            moves.grabboxEnd = playerData.grabboxEndFrame;
            moves.grabDuration = playerData.grabboxDuration;
        }

    }

    private void Update()
    {
        if(playerID == 1)
        {
            GetComponent<AiAgent>().enabled = GameManager.instance.isTraining;
        }
    }

    public void takeDamage(float dmg, Vector2 direction, int hitstun)
    {
        Percent += dmg;
        playerMove.TakeKnockback(direction, hitstun);
    }

    public void getGrabbed(Transform position) //TO DO: link the grabbox's position to the player while being grabbed
    {
        playerState.setActionability(false);
        playerState.setGettingGrabbed(true);
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
    public void setPlayerID(int num)
    {
        playerID = num;
    }

    public PlayerData getPlayerData()
    {
        return playerData;
    }

    public void toggleDisplacement(bool s)
    {
        if (s)
        {
            playerMove.BeginAnimationMovementOverride();
        }
        else
        {
            playerMove.EndAnimationMovementOverride();
        }
    }
}

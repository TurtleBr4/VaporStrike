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
        if (HUD.instance != null)
        {
            if (playerID == 0)
            {
                HUD.instance.p1Percent.value = Percent;
            }
            if (playerID == 1)
            {
                HUD.instance.p2Percent.value = Percent;
                GetComponent<AiAgent>().enabled = GameManager.instance.isTraining && HUD.instance.control == HUD.PracticeControl.AI;
            }
        }
        else
        {
            Debug.Log("HUD is null");
        }
    }

    public void takeDamage(float dmg, Vector2 direction, int hitstun)
    {
        Percent += dmg;
        playerMove.TakeKnockback(direction, hitstun, Percent);
    }

    public void getGrabbed(Transform position) //TO DO: link the grabbox's position to the player while being grabbed
    {
        playerMove.takeInGrabPosition(position);
        playerState.setActionability(false);
        playerState.setGettingGrabbed(true);
    }

    public void doAGrab() //set the player to a state where they can choose to throw
    {
        playerState.setActionability(false);
        playerState.setSuccessfulGrab(true);
        print("grabbed a guy!");
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
}

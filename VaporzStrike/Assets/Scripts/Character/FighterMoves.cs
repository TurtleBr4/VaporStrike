using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FighterMoves : MonoBehaviour
{
    [Header("State Management")]
    public PlayerStateController psc;

    [Header("Input")]
    [SerializeField] private NeoInputReader inputReader;
    [SerializeField] private InputBuffer inputBuffer;
    [SerializeField] private InputInterpreter inputInterpreter;
    [SerializeField] private MoveDefinition[] moves;

    [Header("Animation Controls")]
    [SerializeField] private AnimationClip[] animations;
    [SerializeField] private AnimationClip[] nonAttackingAnimations; //fixed indicies: 0 idle, 1 walk, 2 jump, 3 landing, 4 air idle, 5 for grab, 6 for throw, 7 for parry
    [SerializeField] private bool isAnimatingAndAttacking = false;
    [SerializeField] private bool isInSecondaryAction = false;
    [SerializeField] private int frameCount = 0;
    [SerializeField] private Animation animControl;
    [SerializeField] private Dictionary<int, AnimationClip> idToAnimation; 
    [SerializeField] private AnimationClip currentAnimation;
    [SerializeField] private MoveDefinition currentMove;

    [Header("Hitbox Data")]
    [SerializeField] private GameObject[] hitboxes; //each index contains an object that holds any relevant hitboxes for the move at the corresponding index
    
    [Header("Grab Data")]
    [SerializeField] private GameObject grab;
    public int grabboxStart;
    public int grabboxEnd;
    [FormerlySerializedAs("grabboxDuration")] public int grabDuration;

    
    private void Start()
    {
        //TEMP FRAME LIMITER
        Application.targetFrameRate = 60;
        idToAnimation = new Dictionary<int, AnimationClip>();
        psc = GetComponent<PlayerStateController>();
        foreach (MoveDefinition m in moves)
        {
            idToAnimation[m.animationID] = animations[m.animationID];
        }
    }

    private void Update()
    {
        if (!psc.getActionablility() && isAnimatingAndAttacking) //if we arent actionable and we're attacking run these checks
        {
            AttackingChecks();
        }
        else if (!psc.getActionablility() && isInSecondaryAction)
        {
            DoSecondaryAction();
        }
        else
        {
            //print("checking for moves");
            CheckMoves();
            CheckSecondaryAction();
            CheckOtherAnimations();
        }

        if (!psc.getFacingRight())
        {
            Vector3 currentAngles = animControl.transform.eulerAngles;
            currentAngles.y = -90f; // Change only the Y axis
            animControl.transform.eulerAngles = currentAngles;
        }
        else
        {
            Vector3 currentAngles = animControl.transform.eulerAngles;
            currentAngles.y = 90f; // Change only the Y axis
            animControl.transform.eulerAngles = currentAngles;
        }
       // Debug.Log(frameCount);

    }

    private void CheckMoves()
    {
        for (int i = 0; i < moves.Length; i++)
        {
            MoveDefinition move = moves[i];

            if (!inputInterpreter.CheckMove(move))
            {
                frameCount = 0;
                continue;
            }

            if(psc.getGrounded() && move.isAirMove)
            {
                print("AirCheck running");
                frameCount = 0;
                continue;
            }

            BeginMove(move);
            return;
        }
    }

    private void CheckSecondaryAction() //parry and grab
    {
        if (inputInterpreter.CheckGrab(6))
        {
            psc.setActionability(false);
            psc.setGrabbing(true);
            
            isInSecondaryAction = true;
            currentAnimation = nonAttackingAnimations[5];
            animControl.AddClip(currentAnimation, "Grab");
            animControl.Play("Grab");
            
            frameCount = grabDuration; 
        }

        if (inputInterpreter.CheckParry(1))
        {
            psc.setActionability(false);
            psc.setParrying(true);
            isInSecondaryAction = true;
            
            currentAnimation = nonAttackingAnimations[7];
            animControl.AddClip(currentAnimation, "Parry");
            animControl.Play("Parry");
            
            frameCount = 1;
        }
    }

    private void DoSecondaryAction()
    {
        if (frameCount == 0)
        {
            psc.setActionability(true);
            isInSecondaryAction = false;
            
            psc.setGrabbing(false);
            psc.setParrying(false);
            
            animControl.clip = null;
            return;
        }
        
        if (animControl.isPlaying)
        {
            animControl.Stop();
        }
        
        if (psc.getGrabbing())
        {
            if (frameCount == grabboxStart)
            {
                grab.SetActive(true);
            }

            if (frameCount == grabboxEnd)
            {
                grab.SetActive(false);
            }
        }

        frameCount -= 1;
    }

    private void CheckOtherAnimations()
    {
        if(frameCount == 0)
        {
            if (psc.getMoving() && psc.getGrounded() && !animControl.IsPlaying("Walk")) //walking and running
            {
                currentAnimation = nonAttackingAnimations[1];
                animControl.AddClip(currentAnimation, "Walk");
                animControl.Play("Walk");
            }

            if (!psc.getMoving() && psc.getGrounded() && !(animControl.IsPlaying("Idle") || animControl.IsPlaying("Jump") || animControl.IsPlaying("Landing")))
            {
                currentAnimation = nonAttackingAnimations[0];
                animControl.AddClip(currentAnimation, "Idle");
                animControl.Play("Idle");
            }
            if (psc.justJumped && !animControl.IsPlaying("Jump"))
            {
                animControl.Stop();
                currentAnimation = nonAttackingAnimations[2];
                animControl.AddClip(currentAnimation, "Jump");
                animControl.Play("Jump");
            }
            if (!animControl.IsPlaying("Landing") && psc.justLanded)
            {
                currentAnimation = nonAttackingAnimations[3];
                animControl.AddClip(currentAnimation, "Landing");
                animControl.Play("Landing");
            }

            if (!psc.getMoving() && !psc.getGrounded() && !(animControl.IsPlaying("AirIdle") || animControl.IsPlaying("Jump") || animControl.IsPlaying("Landing") || animControl.IsPlaying("Idle")))
            {
                currentAnimation = nonAttackingAnimations[4];
                animControl.AddClip(currentAnimation, "AirIdle");
                animControl.Play("AirIdle");
            }
        }

    }

    private void BeginMove(MoveDefinition move)
    {
        //Debug.Log("Executing move: " + move.moveName);

        if (animControl.isPlaying)
        {
            animControl.Stop();
        }

        idToAnimation.TryGetValue(move.animationID, out currentAnimation);
        animControl.AddClip(currentAnimation, move.name);
        animControl.Play(move.name);


        isAnimatingAndAttacking = true;
        frameCount = move.frameTime;

        currentMove = move;

        psc.setActionability(false); //inactionable
    }

    private void AttackingChecks()
    {
        if (frameCount == 0)
        {
           
            psc.setActionability(true);
            isAnimatingAndAttacking = false;
            animControl.clip = null;
            return;
            
        }

        if(frameCount <= currentMove.cancelTime)
        {
            psc.setActionability(true); //when the threshold for actionability is reached for a cancelable move, set actionability
        }

        if(frameCount == currentMove.hitboxStartFrame)
        {
            hitboxes[currentMove.animationID].SetActive(true);
            Debug.Log("Ahh hit");
        }

        if (frameCount <= currentMove.hitboxEndFrame)
        {
            hitboxes[currentMove.animationID].SetActive(false);
        }

        frameCount -= 1;
    }

    public MoveDefinition getCurrentMove()
    {
        return currentMove;
    }
   
}
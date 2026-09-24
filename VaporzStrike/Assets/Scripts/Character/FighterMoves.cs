using System;
using System.Collections.Generic;
using UnityEngine;

public class FighterMoves : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private NeoInputReader inputReader;
    [SerializeField] private InputBuffer inputBuffer;
    [SerializeField] private InputInterpreter inputInterpreter;
    [SerializeField] private MoveDefinition[] moves;
    [Header("Animation Controls")]
    [SerializeField] private Animation[] animations;
    private Dictionary<int, Animation> idToAnimation = new Dictionary<int, Animation>();
    private Animation currentAnimation;
    private MoveDefinition currentMove;

    private void Start()
    {
        
    }

    private void Update()
    {
        CheckMoves();
    }

    private void CheckMoves()
    {
        for (int i = 0; i < moves.Length; i++)
        {
            MoveDefinition move = moves[i];

            if (!inputInterpreter.CheckMove(move))
            {
                continue;
            }

            ExecuteMove(move);
            return;
        }
    }

    private void ExecuteMove(MoveDefinition move)
    {
        Debug.Log("Executing move: " + move.moveName);
        idToAnimation.TryGetValue(move.animationID, out currentAnimation);
        currentAnimation.Play();

        while (currentAnimation.isPlaying)
        {
            
        }
    }

   
}
using UnityEngine;

public class FighterMoves : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private InputBuffer inputBuffer;
    [SerializeField] private InputInterpreter inputInterpreter;

    [SerializeField] private MoveDefinition[] moves;

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
    }
}
using UnityEngine;
using UnityEngine.Windows;

[CreateAssetMenu(fileName = "Moveset", menuName = "Scriptable Objects/Moveset")]
public class Moveset : ScriptableObject
{
    public Move[] moves;
    public GameObject playerState;

    /// <summary>
    /// Checks the input buffer for the **longest matching move** in the moveset.
    /// Returns the move if found, null otherwise.
    /// </summary>
    public Move GetMatchingMove(InputType[] inputBuffer)
    {
        Move bestMatch = null;
        int bestLength = 0;

        foreach (Move move in moves)
        {
            if (move.inputSequence.Length > inputBuffer.Length)
                continue;

            bool match = true;
            // Compare move sequence from the end of the buffer (most recent inputs)
            for (int i = 0; i < move.inputSequence.Length; i++)
            {
                //hopefully allows for airieals I just wanted the game to compile for some other testing
/*                if (i < 5 && !playerState.GetComponent<GenericCharacter>().isGrounded)
                {
                    i = 5;
                }*/
                if (inputBuffer[i] != move.inputSequence[i])
                {
                    match = false;
                    break;
                }
            }

            // Pick the longest move (to prioritize complex moves)
            if (match && move.inputSequence.Length > bestLength)
            {
                bestMatch = move;
                bestLength = move.inputSequence.Length;
            }
        }

        return bestMatch;
    }
}

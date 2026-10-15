using UnityEngine;

public class InputInterpreter : MonoBehaviour
{
    [SerializeField] private NeoInputReader inputReader;
    [SerializeField] private InputBuffer inputBuffer;

    public bool CheckMove(MoveDefinition move)
    {
        int buttonFrame =
            inputReader.FindPressedFrame(
                move.button,
                move.buttonWindow
            );

        if (buttonFrame == -1)
        {
            return false;
        }

        if (move.direction == InputDirection.Neutral)
        {
            return true;
        }

        return inputReader.WasDirectionNearFrame(
            move.direction,
            buttonFrame,
            move.directionWindow
        );
    }

    public bool CheckParry(int buttonWindow)
    {
        return inputReader.WasPressed(
            GameInput.Parry,
            buttonWindow
        );
    }

    public bool CheckGrab(int buttonWindow)
    {
        return inputReader.WasPressed(
            GameInput.Grab,
            buttonWindow
        );
    }

    public bool CheckBufferedMove(
        BufferedInput bufferedInput,
        MoveDefinition move)
    {
        if (bufferedInput.button != move.button)
        {
            return false;
        }

        int currentFrame = inputReader.CurrentFrame;

        int framesSinceInput =
            currentFrame - bufferedInput.frame;

        if (framesSinceInput > move.bufferDuration)
        {
            return false;
        }

        if (move.direction == InputDirection.Neutral)
        {
            return true;
        }

        return inputReader.WasDirectionNearFrame(
            move.direction,
            bufferedInput.frame,
            move.directionWindow
        );
    }

    public bool TryGetMove(
        MoveDefinition[] moves,
        out MoveDefinition result)
    {
        result = null;

        if (!inputBuffer.TryGetOldestInput(
                out BufferedInput buffered))
        {
            return false;
        }

        for (int i = 0; i < moves.Length; i++)
        {
            MoveDefinition move = moves[i];

            if (!CheckBufferedMove(buffered, move))
            {
                continue;
            }

            result = move;

            inputBuffer.ConsumeInput(buffered);

            return true;
        }

        return false;
    }
}

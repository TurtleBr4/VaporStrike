
using System.Collections.Generic;
using UnityEngine;

public class InputBuffer : MonoBehaviour
{
    [SerializeField] private NeoInputReader inputReader;

    private List<BufferedInput> bufferedInputs =
        new List<BufferedInput>();

    private void Update()
    {
        if (inputReader != null)
        {
            RecordInputs();
            RemoveExpiredInputs();
        }
    }

    private void RecordInputs()
    {
        RecordButton(GameInput.Attack);
        RecordButton(GameInput.Special);
        RecordButton(GameInput.Jump);
        RecordButton(GameInput.Shield);
        RecordButton(GameInput.Grab);
        RecordButton(GameInput.Parry);
    }

    private void RecordButton(GameInput button)
    {
        if (!inputReader.WasPressed(button))
        {
            return;
        }

        AddInput(button, inputReader.GetDirection());
    }

    // Allows the AI to inject an input directly.
    public void AddInput(GameInput button, InputDirection direction)
    {
        int frame = 0;

        if (inputReader != null)
        {
            frame = inputReader.CurrentFrame;
        }

        BufferedInput input = new BufferedInput(
            button,
            direction,
            frame
        );

        bufferedInputs.Add(input);
    }

    private void RemoveExpiredInputs()
    {
        int currentFrame = inputReader.CurrentFrame;

        for (int i = bufferedInputs.Count - 1;
             i >= 0;
             i--)
        {
            if (currentFrame - bufferedInputs[i].frame > 10)
            {
                bufferedInputs.RemoveAt(i);
            }
        }
    }

    public bool TryGetOldestInput(
        out BufferedInput input)
    {
        if (bufferedInputs.Count == 0)
        {
            input = default;
            return false;
        }

        input = bufferedInputs[0];
        return true;
    }

    public void ConsumeInput(BufferedInput input)
    {
        bufferedInputs.Remove(input);
    }

    public void Clear()
    {
        bufferedInputs.Clear();
    }

    public int Count()
    {
        return bufferedInputs.Count;
    }
}

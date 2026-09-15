using System.Collections.Generic;
using UnityEngine;

public class InputBuffer : MonoBehaviour
{
    [SerializeField] private NeoInputReader inputReader;

    private List<BufferedInput> bufferedInputs =
        new List<BufferedInput>();

    private void Update()
    {
        RecordInputs();
    }

    private void RecordInputs()
    {
        RecordButton(GameInput.Attack);
        RecordButton(GameInput.Special);
        RecordButton(GameInput.Jump);
        RecordButton(GameInput.Shield);
        RecordButton(GameInput.Grab);

        RemoveExpiredInputs();
    }

    private void RecordButton(GameInput button)
    {
        if (!inputReader.WasPressed(button))
        {
            return;
        }

        BufferedInput input =
            new BufferedInput(
                button,
                inputReader.GetDirection(),
                inputReader.CurrentFrame
            );

        bufferedInputs.Add(input);
    }

    private void RemoveExpiredInputs()
    {
        int currentFrame =
            inputReader.CurrentFrame;

        for (int i = bufferedInputs.Count - 1;
             i >= 0;
             i--)
        {
            // Give the input a generous default lifetime.
            if (currentFrame -
                bufferedInputs[i].frame > 10)
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

    public void ConsumeInput(
        BufferedInput input)
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
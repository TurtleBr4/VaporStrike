public enum GameInput
{
    Attack,
    Special,
    Jump,
    Shield,
    Grab,
    Parry
}

public enum InputDirection
{
    Neutral,
    Left,
    Right,
    Up,
    Down
}

public struct MoveInput
{
    public InputDirection direction;
    public GameInput button;

    public int directionWindow;
    public int buttonWindow;

    public MoveInput(
        InputDirection direction,
        GameInput button,
        int directionWindow = 6,
        int buttonWindow = 6)
    {
        this.direction = direction;
        this.button = button;

        this.directionWindow = directionWindow;
        this.buttonWindow = buttonWindow;
    }
}

public struct BufferedInput
{
    public GameInput button;
    public InputDirection direction;

    public int frame;

    public BufferedInput(
        GameInput button,
        InputDirection direction,
        int frame)
    {
        this.button = button;
        this.direction = direction;
        this.frame = frame;
    }
}
namespace WE.Runtime;

public readonly record struct KeyboardState(bool Left, bool Right)
{
    public bool Up { get; init; }
    public bool Down { get; init; }
    public bool W { get; init; }
    public bool A { get; init; }
    public bool S { get; init; }
    public bool D { get; init; }
}

using System.Numerics;

namespace WE.Runtime;

public readonly record struct MouseClick(Vector2 Ndc, float AspectRatio);

public readonly record struct MouseState(float MiddleDragX, float ScrollY)
{
    public MouseClick? RightClick { get; init; }
}

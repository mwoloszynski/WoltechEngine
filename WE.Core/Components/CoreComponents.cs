namespace WE.Core.Components;

/// <summary>
/// Data on an object's position and velocity within the game world (a structure designed for performance)
/// </summary>
public struct TransformComponent
{
    public float X;
    public float Y;
    public float VelocityX;
    public float VelocityY;

    public TransformComponent(float x, float y, float vx = 0, float vy = 0)
    {
        X = x;
        Y = y;
        VelocityX = vx;
        VelocityY = vy;
    }
}

/// <summary>
/// Basic data on the object's appearance for the renderer
/// </summary>
public struct SpriteComponent
{
    public float R;
    public float G;
    public float B;
    public float Size;

    public SpriteComponent(float r, float g, float b, float size = 1.0f)
    {
        R = r;
        G = g;
        B = b;
        Size = size;
    }
}

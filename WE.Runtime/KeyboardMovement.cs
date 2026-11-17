using System.Numerics;

namespace WE.Runtime;

public static class KeyboardMovement
{
    public static Vector3 Step(
        Vector3 position, KeyboardState keyboard, Vector3 cameraOffset, float distance)
    {
        int horizontal = (keyboard.D ? 1 : 0) - (keyboard.A ? 1 : 0);
        int vertical = (keyboard.W ? 1 : 0) - (keyboard.S ? 1 : 0);

        return MoveInView(position, horizontal, vertical, cameraOffset, distance);
    }

    internal static Vector3 MoveInView(
        Vector3 position, int horizontal, int vertical, Vector3 cameraOffset, float distance)
    {
        if ((horizontal == 0 && vertical == 0) || distance <= 0f)
            return position;

        var forward = new Vector3(-cameraOffset.X, 0f, -cameraOffset.Z);
        forward = forward.LengthSquared() > 0f ? Vector3.Normalize(forward) : -Vector3.UnitZ;
        var right = Vector3.Cross(forward, Vector3.UnitY);
        var direction = Vector3.Normalize(right * horizontal + forward * vertical);

        return position + direction * distance;
    }
}

using System.Numerics;
using WE.Graphics;

namespace WE.Runtime;

public sealed class ClickMovement
{
    private Vector2? _target;

    public void SetTarget(Vector3 target)
    {
        _target = new Vector2(target.X, target.Z);
    }

    public Vector3 Step(Vector3 position, float distance)
    {
        if (_target is not { } target || distance <= 0f)
            return position;

        var destination = new Vector3(target.X, position.Y, target.Y);
        var offset = destination - position;
        float remaining = offset.Length();

        if (remaining <= distance)
        {
            _target = null;
            return destination;
        }

        return position + offset * (distance / remaining);
    }

    public static bool TryGetTarget(MouseClick click, Camera3D camera, float planeY, out Vector3 target)
    {
        target = default;

        if (click.AspectRatio <= 0f || !float.IsFinite(click.AspectRatio)
            || !(MathF.Abs(click.Ndc.X) <= 1f && MathF.Abs(click.Ndc.Y) <= 1f))
            return false;

        var viewProjection = camera.CreateViewMatrix()
            * camera.CreateProjectionMatrix(click.AspectRatio);

        if (!Matrix4x4.Invert(viewProjection, out var inverse))
            return false;

        // Two finite depths also work with an infinite far plane.
        var near = Unproject(click.Ndc, 0f, inverse);
        var middle = Unproject(click.Ndc, 0.5f, inverse);
        var direction = Vector3.Normalize(middle - near);

        if (MathF.Abs(direction.Y) < 0.0001f)
            return false;

        float distance = (planeY - near.Y) / direction.Y;
        if (distance < 0f || !float.IsFinite(distance))
            return false;

        var hit = near + direction * distance;
        var clip = Vector4.Transform(new Vector4(hit, 1f), viewProjection);
        if (!(clip.Z <= clip.W))
            return false;

        hit.Y = planeY;
        target = hit;
        return true;
    }

    private static Vector3 Unproject(Vector2 ndc, float depth, Matrix4x4 inverse)
    {
        var point = Vector4.Transform(new Vector4(ndc.X, ndc.Y, depth, 1f), inverse);
        return new Vector3(point.X, point.Y, point.Z) / point.W;
    }
}

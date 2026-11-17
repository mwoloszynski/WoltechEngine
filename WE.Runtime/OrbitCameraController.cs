using System.Numerics;
using WE.Graphics;

namespace WE.Runtime;

public sealed class OrbitCameraController
{
    private const float RadiansPerPixel = 0.005f;
    private const float ZoomPerNotch = 0.85f;
    private const float MinZoom = 0.5f;
    private const float MaxZoom = 2.5f;

    private readonly Camera3D _camera;
    private readonly Vector3 _initialOffset;
    private readonly float _initialOrthographicHeight;
    private float _yaw;
    private float _zoom = 1f;

    public Vector3 Offset { get; private set; }

    public OrbitCameraController(Camera3D camera)
    {
        _camera = camera;
        _initialOffset = camera.Position - camera.Target;
        _initialOrthographicHeight = camera.OrthographicHeight;
        Offset = _initialOffset;
    }

    public void ApplyInput(MouseState mouse)
    {
        if (mouse.MiddleDragX == 0f && mouse.ScrollY == 0f)
            return;

        _yaw = MathF.IEEERemainder(_yaw - mouse.MiddleDragX * RadiansPerPixel, MathF.Tau);
        _zoom = Math.Clamp(_zoom * MathF.Pow(ZoomPerNotch, mouse.ScrollY), MinZoom, MaxZoom);
        var rotatedOffset = Vector3.Transform(_initialOffset, Matrix4x4.CreateRotationY(_yaw));

        if (_camera.ProjectionMode == CameraProjectionMode.Orthographic)
        {
            Offset = rotatedOffset;
            _camera.OrthographicHeight = _initialOrthographicHeight * _zoom;
        }
        else
        {
            Offset = rotatedOffset * _zoom;
        }
    }

    public void Follow(Vector3 target)
    {
        _camera.Position = target + Offset;
        _camera.Target = target;
    }

    public void Pan(KeyboardState keyboard, float distance)
    {
        Pan(keyboard, distance, useWasd: false);
    }

    public void Pan(KeyboardState keyboard, float distance, bool useWasd)
    {
        bool right = keyboard.Right || (useWasd && keyboard.D);
        bool left = keyboard.Left || (useWasd && keyboard.A);
        bool up = keyboard.Up || (useWasd && keyboard.W);
        bool down = keyboard.Down || (useWasd && keyboard.S);
        int horizontal = (right ? 1 : 0) - (left ? 1 : 0);
        int vertical = (up ? 1 : 0) - (down ? 1 : 0);

        var target = KeyboardMovement.MoveInView(
            _camera.Target, horizontal, vertical, Offset, distance);

        Follow(target);
    }
}

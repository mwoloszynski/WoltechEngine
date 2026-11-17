using System.Numerics;

namespace WE.Graphics;

public enum CameraProjectionMode
{
    Perspective,
    Orthographic
}

public sealed class Camera3D
{
    public Vector3 Position { get; set; } = new(2.5f, 1.8f, 3f);
    public Vector3 Target { get; set; } = Vector3.Zero;
    public Vector3 Up { get; set; } = Vector3.UnitY;

    public float FieldOfViewRadians { get; set; } = MathF.PI / 3f;
    public CameraProjectionMode ProjectionMode { get; set; } = CameraProjectionMode.Perspective;
    public float OrthographicHeight { get; set; } = 5f;
    public float NearPlane { get; set; } = 0.1f;
    public float FarPlane { get; set; } = 100f;

    public Matrix4x4 CreateViewMatrix()
    {
        return Matrix4x4.CreateLookAt(Position, Target, Up);
    }

    public Matrix4x4 CreateProjectionMatrix(float aspectRatio)
    {
        if (ProjectionMode == CameraProjectionMode.Orthographic)
        {
            return Matrix4x4.CreateOrthographic(
                OrthographicHeight * aspectRatio, OrthographicHeight, NearPlane, FarPlane);
        }

        return Matrix4x4.CreatePerspectiveFieldOfView(
            FieldOfViewRadians, aspectRatio, NearPlane, FarPlane);
    }
}

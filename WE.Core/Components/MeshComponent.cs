using System.Numerics;
using WE.Core.Geometry;

namespace WE.Core.Components;

public readonly record struct MeshComponent(Vector3 Position, MeshData Mesh)
{
    public Vector3 Color { get; init; } = new(0.35f, 0.75f, 1.0f);
    public Vector3 Scale { get; init; } = Vector3.One;
}

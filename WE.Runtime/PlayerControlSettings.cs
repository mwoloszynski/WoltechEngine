using System.Numerics;

namespace WE.Runtime;

public sealed record PlayerControlSettings(int Entity, float Speed)
{
    public float GroundY { get; init; }
    public Predicate<Vector3>? IsTargetAllowed { get; init; }
    public Func<Vector3, Vector3>? ConstrainPosition { get; init; }
}

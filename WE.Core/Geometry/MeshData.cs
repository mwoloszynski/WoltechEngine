namespace WE.Core.Geometry;

public sealed class MeshData
{
    public const int FloatsPerVertex = 4;

    private readonly float[] _vertices;
    private readonly uint[] _indices;

    public ReadOnlySpan<float> Vertices => _vertices;
    public ReadOnlySpan<uint> Indices => _indices;

    public MeshData(ReadOnlySpan<float> vertices, ReadOnlySpan<uint> indices)
    {
        if (vertices.IsEmpty || vertices.Length % FloatsPerVertex != 0)
            throw new ArgumentException("Expected X, Y, Z and shade per vertex.", nameof(vertices));

        if (indices.IsEmpty || indices.Length % 3 != 0)
            throw new ArgumentException("Expected three indices per triangle.", nameof(indices));

        uint vertexCount = (uint)(vertices.Length / FloatsPerVertex);
        foreach (uint index in indices)
        {
            if (index >= vertexCount)
                throw new ArgumentOutOfRangeException(nameof(indices), "Index exceeds vertex count.");
        }

        _vertices = vertices.ToArray();
        _indices = indices.ToArray();
    }
}

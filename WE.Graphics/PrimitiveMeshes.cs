using WE.Core.Geometry;

namespace WE.Graphics;

public static class PrimitiveMeshes
{
    private static readonly float[] CubePositions =
    {
        // +Z
        -.5f, -.5f,  .5f,   .5f, -.5f,  .5f,
         .5f,  .5f,  .5f,  -.5f,  .5f,  .5f,
        // +X
         .5f, -.5f,  .5f,   .5f, -.5f, -.5f,
         .5f,  .5f, -.5f,   .5f,  .5f,  .5f,
        // +Y
        -.5f,  .5f,  .5f,   .5f,  .5f,  .5f,
         .5f,  .5f, -.5f,  -.5f,  .5f, -.5f,
        // -Z
         .5f, -.5f, -.5f,  -.5f, -.5f, -.5f,
        -.5f,  .5f, -.5f,   .5f,  .5f, -.5f,
        // -X
        -.5f, -.5f, -.5f,  -.5f, -.5f,  .5f,
        -.5f,  .5f,  .5f,  -.5f,  .5f, -.5f,
        // -Y
        -.5f, -.5f, -.5f,   .5f, -.5f, -.5f,
         .5f, -.5f,  .5f,  -.5f, -.5f,  .5f
    };

    private static readonly uint[] CubeIndices =
    {
         0,  1,  2,  0,  2,  3,
         4,  5,  6,  4,  6,  7,
         8,  9, 10,  8, 10, 11,
        12, 13, 14, 12, 14, 15,
        16, 17, 18, 16, 18, 19,
        20, 21, 22, 20, 22, 23
    };

    public static MeshData Cube { get; } = CreateCube();

    public static MeshData Pyramid { get; } = new MeshData(
        new float[]
        {
            // +Z
            -.5f, -.5f,  .5f, .95f,
             .5f, -.5f,  .5f, .95f,
               0f,  .5f,    0f, .95f,
            // +X
             .5f, -.5f,  .5f, .80f,
             .5f, -.5f, -.5f, .80f,
               0f,  .5f,    0f, .80f,
            // -Z
             .5f, -.5f, -.5f, .65f,
            -.5f, -.5f, -.5f, .65f,
               0f,  .5f,    0f, .65f,
            // -X
            -.5f, -.5f, -.5f, .72f,
            -.5f, -.5f,  .5f, .72f,
               0f,  .5f,    0f, .72f,
            // Base
            -.5f, -.5f, -.5f, .50f,
             .5f, -.5f, -.5f, .50f,
             .5f, -.5f,  .5f, .50f,
            -.5f, -.5f,  .5f, .50f
        },
        new uint[]
        {
             0,  1,  2,
             3,  4,  5,
             6,  7,  8,
             9, 10, 11,
            12, 13, 14, 12, 14, 15
        });

    private static MeshData CreateCube()
    {
        int vertexCount = CubePositions.Length / 3;
        var vertices = new float[vertexCount * MeshData.FloatsPerVertex];

        for (int i = 0; i < vertexCount; i++)
        {
            int source = i * 3;
            int destination = i * MeshData.FloatsPerVertex;

            vertices[destination] = CubePositions[source];
            vertices[destination + 1] = CubePositions[source + 1];
            vertices[destination + 2] = CubePositions[source + 2];
            vertices[destination + 3] = 0.95f - 0.09f * (i / 4);
        }

        return new MeshData(vertices, CubeIndices);
    }
}

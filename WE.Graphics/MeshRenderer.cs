using Silk.NET.OpenGL;
using System.Numerics;
using WE.Core.ECS;
using WE.Core.Geometry;

namespace WE.Graphics;

internal sealed class MeshRenderer : IDisposable
{
    private const string VertexSource = """
        #version 330 core
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in float aShade;
        uniform mat4 uModelViewProjection;
        uniform vec3 uColor;
        flat out vec3 vColor;

        void main()
        {
            vec4 clip = uModelViewProjection * vec4(aPosition, 1.0);
            clip.z = 2.0 * clip.z - clip.w;
            gl_Position = clip;
            vColor = uColor * aShade;
        }
        """;

    private const string FragmentSource = """
        #version 330 core
        flat in vec3 vColor;
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(vColor, 1.0);
        }
        """;

    private readonly GL _gl;
    private readonly ShaderProgram _shader;
    private readonly Dictionary<MeshData, GpuMesh> _meshes = new();
    private readonly int _matrixLocation;
    private readonly int _colorLocation;
    private readonly Camera3D _defaultCamera = new();

    public MeshRenderer(GL gl)
    {
        _gl = gl;
        _shader = new ShaderProgram(gl, VertexSource, FragmentSource);

        try
        {
            _matrixLocation = gl.GetUniformLocation(_shader.Handle, "uModelViewProjection");
            _colorLocation = gl.GetUniformLocation(_shader.Handle, "uColor");
        }
        catch
        {
            _shader.Dispose();
            throw;
        }
    }

    public unsafe void Render(Registry registry, float aspectRatio, Camera3D? camera)
    {
        camera ??= _defaultCamera;
        var viewProjection = camera.CreateViewMatrix() * camera.CreateProjectionMatrix(aspectRatio);

        _shader.Use();
        _gl.Enable(EnableCap.DepthTest);

        try
        {
            foreach (int entity in registry.Entities)
            {
                if (!registry.Meshes.TryGetValue(entity, out var instance) || instance.Mesh is null)
                    continue;

                if (!_meshes.TryGetValue(instance.Mesh, out var gpuMesh))
                {
                    gpuMesh = new GpuMesh(_gl, instance.Mesh);
                    _meshes.Add(instance.Mesh, gpuMesh);
                }

                var model = Matrix4x4.CreateScale(instance.Scale)
                    * Matrix4x4.CreateTranslation(instance.Position);
                var mvp = model * viewProjection;

                _gl.UniformMatrix4(_matrixLocation, 1, false, &mvp.M11);
                _gl.Uniform3(_colorLocation, instance.Color.X, instance.Color.Y, instance.Color.Z);
                gpuMesh.Draw();
            }
        }
        finally
        {
            _gl.Disable(EnableCap.DepthTest);
        }
    }

    public void Dispose()
    {
        foreach (var mesh in _meshes.Values)
            mesh.Dispose();

        _meshes.Clear();
        _shader.Dispose();
    }
}

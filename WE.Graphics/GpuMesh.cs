using Silk.NET.OpenGL;
using WE.Core.Geometry;

namespace WE.Graphics;

internal sealed class GpuMesh : IDisposable
{
    private readonly GL _gl;
    private readonly VertexArrayObject _vao;
    private readonly BufferObject<float> _vbo;
    private readonly BufferObject<uint> _ebo;
    private readonly uint _indexCount;

    public GpuMesh(GL gl, MeshData mesh)
    {
        _gl = gl;
        _indexCount = (uint)mesh.Indices.Length;
        _vao = new VertexArrayObject(gl);

        BufferObject<float>? vbo = null;
        BufferObject<uint>? ebo = null;

        try
        {
            _vao.Bind();
            vbo = new BufferObject<float>(gl, mesh.Vertices, BufferTargetARB.ArrayBuffer);
            ebo = new BufferObject<uint>(gl, mesh.Indices, BufferTargetARB.ElementArrayBuffer);

            const uint stride = MeshData.FloatsPerVertex * sizeof(float);
            _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, stride, 0);
            _vao.VertexAttributePointer(1, 1, VertexAttribPointerType.Float, stride, 3 * sizeof(float));

            _vbo = vbo;
            _ebo = ebo;
        }
        catch
        {
            ebo?.Dispose();
            vbo?.Dispose();
            _vao.Dispose();
            throw;
        }
    }

    public unsafe void Draw()
    {
        _vao.Bind();
        _gl.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, null);
    }

    public void Dispose()
    {
        _ebo.Dispose();
        _vbo.Dispose();
        _vao.Dispose();
    }
}

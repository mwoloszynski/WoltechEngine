using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;
using WE.Core.ECS;

namespace WE.Graphics
{
    public class RenderSystem : IDisposable
    {
        private readonly GL _gl;
        private ShaderProgram _shader;
        private VertexArrayObject _vao;
        private BufferObject<float> _vbo;
        private BufferObject<uint> _ebo;
        private MeshRenderer? _meshRenderer;

        private readonly float[] _vertices =
        {
            -0.5f,  0.5f, // Top-left
            -0.5f, -0.5f, // Bottom-left
             0.5f, -0.5f, // Bottom-right
             0.5f,  0.5f  // Top-right
        };

        private readonly uint[] _indices = { 0, 1, 2, 0, 2, 3 };

        public RenderSystem(GL gl)
        {
            _gl = gl;
            InitializeGraphics();
        }

        private void InitializeGraphics()
        {
            // Simple GLSL Vertex Shader code
            string vertexSource = @"
#version 330 core
layout (location = 0) in vec2 aPosition;
uniform vec2 uOffset;
uniform float uSize;
uniform vec2 uProjectionScale;
void main() {
    vec2 worldPosition = aPosition * uSize + uOffset;
    gl_Position = vec4(worldPosition * uProjectionScale, 0.0, 1.0);
}";

            // Simple GLSL Fragment Shader code
            string fragmentSource = @"
#version 330 core
out vec4 FragColor;
uniform vec3 uColor;
void main() {
    FragColor = vec4(uColor, 1.0);
}";

            _shader = new ShaderProgram(_gl, vertexSource, fragmentSource);

            _vao = new VertexArrayObject(_gl);
            _vao.Bind();

            _vbo = new BufferObject<float>(_gl, _vertices, BufferTargetARB.ArrayBuffer);
            _ebo = new BufferObject<uint>(_gl, _indices, BufferTargetARB.ElementArrayBuffer);

            _vao.VertexAttributePointer(0, 2, VertexAttribPointerType.Float, 2 * sizeof(float), 0);
        }

        public void Render(Registry registry, int framebufferWidth, int framebufferHeight, Camera3D? camera = null)
        {
            if (framebufferWidth <= 0 || framebufferHeight <= 0)
                return;

            _gl.Viewport(0, 0, (uint)framebufferWidth, (uint)framebufferHeight);

            _shader.Use();
            _vao.Bind();

            const float pixelsPerWorldUnit = 512f;
            float scaleX = 2f * pixelsPerWorldUnit / framebufferWidth;
            float scaleY = 2f * pixelsPerWorldUnit / framebufferHeight;

            int projectionScaleLoc = _gl.GetUniformLocation(_shader.Handle, "uProjectionScale");
            _gl.Uniform2(projectionScaleLoc, scaleX, scaleY);

            int offsetLoc = _gl.GetUniformLocation(_shader.Handle, "uOffset");
            int sizeLoc = _gl.GetUniformLocation(_shader.Handle, "uSize");
            int colorLoc = _gl.GetUniformLocation(_shader.Handle, "uColor");

            foreach (var entity in registry.Entities)
            {
                if (registry.Transforms.ContainsKey(entity) && registry.Sprites.ContainsKey(entity))
                {
                    var transform = registry.Transforms[entity];
                    var sprite = registry.Sprites[entity];

                    _gl.Uniform2(offsetLoc, transform.X, transform.Y);
                    _gl.Uniform1(sizeLoc, sprite.Size);
                    _gl.Uniform3(colorLoc, sprite.R, sprite.G, sprite.B);

                    unsafe
                    {
                        _gl.DrawElements(PrimitiveType.Triangles, (uint)_indices.Length, DrawElementsType.UnsignedInt, null);
                    }
                }
            }

            if (registry.Meshes.Count > 0)
            {
                _meshRenderer ??= new MeshRenderer(_gl);
                float aspectRatio = (float)framebufferWidth / framebufferHeight;
                _meshRenderer.Render(registry, aspectRatio, camera);
            }
        }

        public void Dispose()
        {
            _meshRenderer?.Dispose();
            _vbo.Dispose();
            _ebo.Dispose();
            _vao.Dispose();
            _shader.Dispose();
        }
    }
}

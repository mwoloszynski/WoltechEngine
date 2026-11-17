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

        // Define a standard 2D quad geometry (X, Y layout)
        private readonly float[] _vertices =
        {
            -0.5f,  0.5f, // Top-left
            -0.5f, -0.5f, // Bottom-left
             0.5f, -0.5f, // Bottom-right
             0.5f,  0.5f  // Top-right
        };

        // Triangle indices to form the quad
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
void main() {
    gl_Position = vec4((aPosition * uSize) + uOffset, 0.0, 1.0);
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

            // Setup vertex arrays and buffers on GPU memory
            _vao = new VertexArrayObject(_gl);
            _vao.Bind();

            _vbo = new BufferObject<float>(_gl, _vertices, BufferTargetARB.ArrayBuffer);
            _ebo = new BufferObject<uint>(_gl, _indices, BufferTargetARB.ElementArrayBuffer);

            // Tell OpenGL how to read layout location 0 (2 floats per vertex position)
            _vao.VertexAttributePointer(0, 2, VertexAttribPointerType.Float, 2 * sizeof(float), 0);
        }

        public void Render(Registry registry)
        {
            _shader.Use();
            _vao.Bind();

            // Get GPU uniform uniform variable locations
            int offsetLoc = _gl.GetUniformLocation(_shader.Handle, "uOffset");
            int sizeLoc = _gl.GetUniformLocation(_shader.Handle, "uSize");
            int colorLoc = _gl.GetUniformLocation(_shader.Handle, "uColor");

            // Iterate through all active entities inside the ECS database
            foreach (var entity in registry.Entities)
            {
                // Draw only entities that possess both Transform and Sprite components
                if (registry.Transforms.ContainsKey(entity) && registry.Sprites.ContainsKey(entity))
                {
                    var transform = registry.Transforms[entity];
                    var sprite = registry.Sprites[entity];

                    // Send ECS data directly into the active GPU shader program
                    _gl.Uniform2(offsetLoc, transform.X, transform.Y);
                    _gl.Uniform1(sizeLoc, sprite.Size);
                    _gl.Uniform3(colorLoc, sprite.R, sprite.G, sprite.B);

                    // Execute the draw command on the GPU
                    unsafe
                    {
                        _gl.DrawElements(PrimitiveType.Triangles, (uint)_indices.Length, DrawElementsType.UnsignedInt, null);
                    }
                }
            }
        }

        public void Dispose()
        {
            _vbo.Dispose();
            _ebo.Dispose();
            _vao.Dispose();
            _shader.Dispose();
        }
    }
}

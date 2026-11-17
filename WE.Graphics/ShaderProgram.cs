using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace WE.Graphics
{
    public class ShaderProgram : IDisposable
    {
        private readonly GL _gl;
        public uint Handle { get; private set; }

        public ShaderProgram(GL gl, string vertexCode, string fragmentCode)
        {
            _gl = gl;

            uint vertexShader = CompileShader(ShaderType.VertexShader, vertexCode);
            uint fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentCode);

            Handle = _gl.CreateProgram();
            _gl.AttachShader(Handle, vertexShader);
            _gl.AttachShader(Handle, fragmentShader);
            _gl.LinkProgram(Handle);

            _gl.GetProgram(Handle, ProgramPropertyARB.LinkStatus, out int status);
            if (status == 0)
            {
                throw new Exception($"Shader linking failed: {_gl.GetProgramInfoLog(Handle)}");
            }

            _gl.DeleteShader(vertexShader);
            _gl.DeleteShader(fragmentShader);
        }

        public void Use() => _gl.UseProgram(Handle);

        private uint CompileShader(ShaderType type, string code)
        {
            uint shader = _gl.CreateShader(type);
            _gl.ShaderSource(shader, code);
            _gl.CompileShader(shader);

            _gl.GetShader(shader, ShaderParameterName.CompileStatus, out int status);
            if (status == 0)
            {
                throw new Exception($"Shader compilation failed ({type}): {_gl.GetShaderInfoLog(shader)}");
            }
            return shader;
        }

        public void Dispose() => _gl.DeleteProgram(Handle);
    }
}

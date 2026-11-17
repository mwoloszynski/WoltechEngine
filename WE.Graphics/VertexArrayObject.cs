using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace WE.Graphics
{
    public class VertexArrayObject : IDisposable
    {
        private readonly GL _gl;
        public uint Handle { get; private set; }

        public VertexArrayObject(GL gl)
        {
            _gl = gl;
            Handle = _gl.GenVertexArray();
        }

        public void Bind() => _gl.BindVertexArray(Handle);

        public void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint stride, int offset)
        {
            unsafe
            {
                _gl.VertexAttribPointer(index, count, type, false, stride, (void*)offset);
                _gl.EnableVertexAttribArray(index);
            }
        }

        public void Dispose() => _gl.DeleteVertexArray(Handle);
    }
}

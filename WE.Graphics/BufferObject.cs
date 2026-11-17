using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace WE.Graphics
{
    public class BufferObject<TDataType> : IDisposable where TDataType : unmanaged
    {
        private readonly GL _gl;
        public uint Handle { get; private set; }
        private readonly BufferTargetARB _bufferType;

        public BufferObject(GL gl, ReadOnlySpan<TDataType> data, BufferTargetARB bufferType)
        {
            _gl = gl;
            _bufferType = bufferType;

            Handle = _gl.GenBuffer();
            _gl.BindBuffer(_bufferType, Handle);

            unsafe
            {
                fixed (void* d = data)
                {
                    _gl.BufferData(_bufferType, (nuint)(data.Length * sizeof(TDataType)), d, BufferUsageARB.StaticDraw);
                }
            }
        }

        public void Bind() => _gl.BindBuffer(_bufferType, Handle);
        public void Dispose() => _gl.DeleteBuffer(Handle);
    }
}

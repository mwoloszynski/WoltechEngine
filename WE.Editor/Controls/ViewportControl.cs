using Avalonia;
using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;
using WE.Core.ECS;
using WE.Graphics;

namespace WE.Editor.Controls
{
    public class ViewportControl : OpenGlControlBase
    {
        private GL? _gl;
        private RenderSystem? _renderSystem;
        private DispatcherTimer? _renderTimer;

        public static readonly StyledProperty<Registry?> ActiveRegistryProperty =
        AvaloniaProperty.Register<ViewportControl, Registry?>(nameof(ActiveRegistry));

        public static readonly StyledProperty<Camera3D?> ActiveCameraProperty =
            AvaloniaProperty.Register<ViewportControl, Camera3D?>(nameof(ActiveCamera));

        public Registry? ActiveRegistry
        {
            get => GetValue(ActiveRegistryProperty);
            set => SetValue(ActiveRegistryProperty, value);
        }

        public Camera3D? ActiveCamera
        {
            get => GetValue(ActiveCameraProperty);
            set => SetValue(ActiveCameraProperty, value);
        }

        protected override void OnOpenGlInit(GlInterface gl)
        {
            base.OnOpenGlInit(gl);
            _gl = GL.GetApi(gl.GetProcAddress);
            _renderSystem = new RenderSystem(_gl);

            _renderTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(16), DispatcherPriority.Render, (_, _) =>
            {
                if (IsVisible && Bounds.Width > 0 && Bounds.Height > 0)
                {
                    RequestNextFrameRendering();
                }
            });
            _renderTimer.Start();
        }

        protected override void OnOpenGlRender(GlInterface gl, int fb)
        {
            if (_gl == null || _renderSystem == null) return;

            double scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
            int width = (int)(Bounds.Width * scaling);
            int height = (int)(Bounds.Height * scaling);

            if (width <= 0 || height <= 0) return;

            _gl.BindFramebuffer(FramebufferTarget.Framebuffer, (uint)fb);
            _gl.ClearColor(0.08f, 0.09f, 0.11f, 1.0f);
            _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var registry = ActiveRegistry;
            if (registry != null)
            {
                _renderSystem.Render(registry, width, height, ActiveCamera);
            }

            // Reset OpenGL state cleanly
            _gl.BindVertexArray(0);
            _gl.UseProgram(0);
            _gl.BindFramebuffer(FramebufferTarget.Framebuffer, (uint)fb);
        }

        protected override void OnOpenGlDeinit(GlInterface gl)
        {
            _renderTimer?.Stop();
            _renderSystem?.Dispose();
            _gl?.Dispose();
            base.OnOpenGlDeinit(gl);
        }
    }
}

using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using WE.Core.ECS;
using WE.Core.Systems;
using WE.Core.Time;
using WE.Graphics;

namespace WE.Runtime
{
    public static class GameHost
    {
        public static void Run(
            GameSettings settings,
            Func<Registry, PlayerControlSettings> loadScene,
            Camera3D camera,
            Action<Registry, float>? updateScene = null)
        {
            camera.ProjectionMode = settings.UseOrthographicProjection
                ? CameraProjectionMode.Orthographic
                : CameraProjectionMode.Perspective;

            if (settings.CameraInclination is { } inclination)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(inclination, nameof(settings.CameraInclination));
                ArgumentOutOfRangeException.ThrowIfGreaterThan(inclination, 100, nameof(settings.CameraInclination));

                var offset = camera.Position - camera.Target;
                float distance = offset.Length();
                float heading = MathF.Atan2(offset.X, offset.Z);
                float pitch = MathF.Min(inclination * 0.9f, 89.9f) * (MathF.PI / 180f);
                float horizontalDistance = distance * MathF.Cos(pitch);

                camera.Position = camera.Target + new Vector3(
                    horizontalDistance * MathF.Sin(heading),
                    distance * MathF.Sin(pitch),
                    horizontalDistance * MathF.Cos(heading));
            }

            var orbit = new OrbitCameraController(camera);
            var clickMovement = new ClickMovement();
            bool useMouse = settings.MovementMode == PlayerMovementMode.ClickToMove;
            bool freeCamera = settings.CameraMode == CameraControlMode.FreePan;

            KeyboardState keyboard = default;
            PlayerControlSettings? player = null;

            Run(
                title: settings.WindowTitle,
                loadScene: registry =>
                {
                    player = loadScene(registry);

                    if (!registry.Meshes.TryGetValue(player.Entity, out var mesh))
                        throw new InvalidOperationException(
                            "The controlled entity must have a MeshComponent.");

                    if (!freeCamera)
                        orbit.Follow(mesh.Position);
                },
                updateScene: (registry, deltaTime) =>
                {
                    var followTarget = camera.Target;

                    if (player is { } control
                        && registry.Meshes.TryGetValue(control.Entity, out var mesh))
                    {
                        float distance = control.Speed * deltaTime;
                        var position = useMouse
                            ? clickMovement.Step(mesh.Position, distance)
                            : KeyboardMovement.Step(mesh.Position, keyboard, orbit.Offset, distance);

                        position = control.ConstrainPosition?.Invoke(position) ?? position;
                        registry.Meshes[control.Entity] = mesh with { Position = position };
                        followTarget = position;
                    }

                    if (freeCamera)
                        orbit.Pan(keyboard, settings.CameraPanSpeed * deltaTime, useWasd: useMouse);
                    else
                        orbit.Follow(followTarget);

                    updateScene?.Invoke(registry, deltaTime);
                },
                handleInput: (_, input) => keyboard = input,
                camera: camera,
                handleMouse: mouse =>
                {
                    if (useMouse && player is { } control
                        && mouse.RightClick is { } click
                        && ClickMovement.TryGetTarget(click, camera, control.GroundY, out var target)
                        && (control.IsTargetAllowed?.Invoke(target) ?? true))
                    {
                        clickMovement.SetTarget(control.ConstrainPosition?.Invoke(target) ?? target);
                    }

                    orbit.ApplyInput(mouse);
                });
        }

        public static void Run(
            string title,
            Action<Registry> loadScene,
            Action<Registry, float> updateScene,
            Action<Registry, KeyboardState>? handleInput = null,
            Camera3D? camera = null,
            Action<MouseState>? handleMouse = null)
        {
            var options = WindowOptions.Default;
            options.Title = title;
            options.Size = new Vector2D<int>(1024, 768);
            options.VSync = true;
            options.PreferredDepthBufferBits = 24;

            using var window = Window.Create(options);
            window.Initialize();

            using var input = window.CreateInput();
            var keyboard = input.Keyboards.FirstOrDefault();

            float middleDragX = 0f;
            float scrollY = 0f;
            MouseClick? rightClick = null;
            var mouse = input.Mice.FirstOrDefault();
            if (handleMouse is not null && mouse is not null)
            {
                var previousPosition = mouse.Position;

                mouse.MouseMove += (device, position) =>
                {
                    if (device.IsButtonPressed(MouseButton.Middle))
                        middleDragX += position.X - previousPosition.X;

                    previousPosition = position;
                };

                mouse.Scroll += (_, wheel) => scrollY += wheel.Y;
                mouse.MouseDown += (device, button) =>
                {
                    if (button != MouseButton.Right)
                        return;

                    var size = window.Size;
                    var framebuffer = window.FramebufferSize;
                    if (size.X <= 0 || size.Y <= 0 || framebuffer.X <= 0 || framebuffer.Y <= 0)
                        return;

                    var cursor = device.Position;
                    var ndc = new Vector2(
                        2f * cursor.X / size.X - 1f,
                        1f - 2f * cursor.Y / size.Y);

                    rightClick = new MouseClick(ndc, (float)framebuffer.X / framebuffer.Y);
                };
            }

            using var gl = GL.GetApi(window);
            using var renderer = new RenderSystem(gl);

            var registry = new Registry();
            var time = new TimeManager();
            loadScene(registry);

            window.Update += _ =>
            {
                float deltaTime = (float)time.GetDeltaTime();
                handleInput?.Invoke(registry, new KeyboardState(
                    Left: keyboard?.IsKeyPressed(Key.Left) == true,
                    Right: keyboard?.IsKeyPressed(Key.Right) == true)
                {
                    Up = keyboard?.IsKeyPressed(Key.Up) == true,
                    Down = keyboard?.IsKeyPressed(Key.Down) == true,
                    W = keyboard?.IsKeyPressed(Key.W) == true,
                    A = keyboard?.IsKeyPressed(Key.A) == true,
                    S = keyboard?.IsKeyPressed(Key.S) == true,
                    D = keyboard?.IsKeyPressed(Key.D) == true
                });
                var mouseState = new MouseState(middleDragX, scrollY)
                {
                    RightClick = rightClick
                };
                middleDragX = 0f;
                scrollY = 0f;
                rightClick = null;
                handleMouse?.Invoke(mouseState);
                MovementSystem.Update(registry, deltaTime);
                updateScene(registry, deltaTime);
            };

            window.Render += _ =>
            {
                gl.ClearColor(0.08f, 0.09f, 0.11f, 1.0f);
                gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                var framebufferSize = window.FramebufferSize;
                renderer.Render(registry, framebufferSize.X, framebufferSize.Y, camera);
            };

            time.Start();
            window.Run(() =>
            {
                window.DoEvents();

                if (!window.IsClosing)
                    window.DoUpdate();

                if (!window.IsClosing)
                    window.DoRender();
            });
        }
    }
}

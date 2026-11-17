using System;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using WE.Core.ECS;
using WE.Core.Components;
using WE.Core.Time;
using WE.Graphics;

namespace WE.Sandbox
{
    public class Program
    {
        private static IWindow _window;
        private static GL _gl;

        // Core engine subsystems
        private static Registry _registry;
        private static TimeManager _timeManager;
        private static RenderSystem _renderSystem;

        // Test entities
        private static int _movingQuad;

        public static void Main(string[] args)
        {
            // Configure native window options
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(1024, 768);
            options.Title = "WoltechEngine - Sandbox Playground";
            options.VSync = true;

            // Create the window instance
            _window = Window.Create(options);

            // Bind engine loop events to window callbacks
            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.Closing += OnClosing;

            // Start the engine loop
            _window.Run();
        }

        private static void OnLoad()
        {
            // Initialize native OpenGL context via Silk.NET
            _gl = GL.GetApi(_window);

            // Initialize core manager systems
            _registry = new Registry();
            _timeManager = new TimeManager();
            _timeManager.Start();

            // Initialize graphics render pipeline
            _renderSystem = new RenderSystem(_gl);

            // --- GAMEPLAY SANDBOX INITIALIZATION ---

            // Entity 1: A fast moving red quad
            _movingQuad = _registry.CreateEntity();
            _registry.Transforms[_movingQuad] = new TransformComponent(x: -0.8f, y: 0.0f, vx: 0.5f, vy: 0.0f);
            _registry.Sprites[_movingQuad] = new SpriteComponent(r: 1.0f, g: 0.2f, b: 0.2f, size: 0.15f);

            // Entity 2: A stationary blue static obstacle
            int staticQuad = _registry.CreateEntity();
            _registry.Transforms[staticQuad] = new TransformComponent(x: 0.5f, y: 0.3f, vx: 0.0f, vy: 0.0f);
            _registry.Sprites[staticQuad] = new SpriteComponent(r: 0.2f, g: 0.4f, b: 1.0f, size: 0.25f);

            Console.WriteLine("Sandbox systems initialized and running.");
        }

        private static void OnUpdate(double windowDeltaTime)
        {
            // Fetch highly precise delta time from our core TimeManager
            double dt = _timeManager.GetDeltaTime();

            // --- SYSTEM SIMULATION LAYER ---

            // Simulate movement logic for all entities manually for now
            // (Later this will be abstracted out into a dedicated MovementSystem)
            foreach (var entity in _registry.Entities)
            {
                if (_registry.Transforms.ContainsKey(entity))
                {
                    var transform = _registry.Transforms[entity];

                    // Apply velocity vector multiplied by current delta time
                    transform.X += transform.VelocityX * (float)dt;
                    transform.Y += transform.VelocityY * (float)dt;

                    // Simple screen boundary collision check (bounce back)
                    if (transform.X > 1.0f || transform.X < -1.0f)
                    {
                        transform.VelocityX = -transform.VelocityX;
                    }

                    // Save modified transform back to the ECS registry
                    _registry.Transforms[entity] = transform;
                }
            }
        }

        private static void OnRender(double windowDeltaTime)
        {
            // Clear frame buffers with a nice deep dark background
            _gl.ClearColor(0.08f, 0.09f, 0.11f, 1.0f);
            _gl.Clear(ClearBufferMask.ColorBufferBit);

            // Pass our ECS data registry to the specialized render system
            _renderSystem.Render(_registry);
        }

        private static void OnClosing()
        {
            // Safe release of all native GPU heap-allocated resources
            _renderSystem.Dispose();
            _gl.Dispose();

            Console.WriteLine("Sandbox successfully terminated.");
        }
    }
}
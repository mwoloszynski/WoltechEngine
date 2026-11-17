using System.Numerics;
using WE.Core.Components;
using WE.Core.ECS;
using WE.Core.Systems;
using WE.Graphics;
using WE.Runtime;

namespace WE.Sandbox
{
    public class Program
    {
        private static int _movingQuad;
        private static int _playerQuad;

        public static void Main(string[] args)
        {
            if (args.Contains("--3d"))
            {
                Run3D();
            }
            else
            {
                GameHost.Run("WoltechEngine - Sandbox", LoadScene, UpdateScene, HandleInput);
            }
        }

        private static void Run3D()
        {
            const float groundSize = 20f;
            const float groundThickness = 0.2f;
            const float playerHalfSize = 0.5f;
            const float speed = 3f;
            const float movementLimit = groundSize / 2f - playerHalfSize;

            var initialCameraOffset = new Vector3(10f, 9.6f, 10f);
            var playerStart = new Vector3(0f, playerHalfSize, 0f);
            var camera = new Camera3D
            {
                Position = playerStart + initialCameraOffset,
                Target = playerStart,
                OrthographicHeight = 5f
            };

            var settings = new GameSettings
            {
                WindowTitle = "WoltechEngine - Sandbox 3D",
                MovementMode = PlayerMovementMode.ClickToMove,
                CameraMode = CameraControlMode.FreePan,
                CameraPanSpeed = 5f,
                UseOrthographicProjection = true,
                CameraInclination = 35
            };

            GameHost.Run(
                settings,
                loadScene: registry =>
                {
                    int playerEntity = registry.CreateEntity();
                    registry.Meshes[playerEntity] = new MeshComponent(playerStart, PrimitiveMeshes.Cube);

                    int markerEntity = registry.CreateEntity();
                    registry.Meshes[markerEntity] = new MeshComponent(
                        new Vector3(1.6f, playerHalfSize, -1.1f), PrimitiveMeshes.Pyramid)
                    {
                        Color = new Vector3(1.0f, 0.55f, 0.2f)
                    };

                    int groundEntity = registry.CreateEntity();
                    registry.Meshes[groundEntity] = new MeshComponent(
                        new Vector3(0f, -groundThickness / 2f, 0f), PrimitiveMeshes.Cube)
                    {
                        Scale = new Vector3(groundSize, groundThickness, groundSize),
                        Color = new Vector3(0.35f, 0.45f, 0.38f)
                    };
                    return new PlayerControlSettings(playerEntity, speed)
                    {
                        GroundY = 0f,
                        IsTargetAllowed = point =>
                            MathF.Abs(point.X) <= groundSize / 2f
                            && MathF.Abs(point.Z) <= groundSize / 2f,
                        ConstrainPosition = ClampToGround
                    };
                },
                camera: camera);

            static Vector3 ClampToGround(Vector3 position)
            {
                position.X = Math.Clamp(position.X, -movementLimit, movementLimit);
                position.Z = Math.Clamp(position.Z, -movementLimit, movementLimit);
                return position;
            }
        }

        private static void LoadScene(Registry registry)
        {
            _movingQuad = registry.CreateEntity();
            registry.Transforms[_movingQuad] = new TransformComponent(x: -0.8f, y: 0.0f, vx: 0.5f, vy: 0.0f);
            registry.Sprites[_movingQuad] = new SpriteComponent(r: 1.0f, g: 0.2f, b: 0.2f, size: 0.15f);
            registry.BoxColliders[_movingQuad] = new BoxColliderComponent(0.15f, 0.15f);

            _playerQuad = registry.CreateEntity();
            registry.Transforms[_playerQuad] = new TransformComponent(x: 0.5f, y: 0.0f, vx: 0.0f, vy: 0.0f);
            registry.Sprites[_playerQuad] = new SpriteComponent(r: 0.2f, g: 0.4f, b: 1.0f, size: 0.25f);
            registry.BoxColliders[_playerQuad] = new BoxColliderComponent(0.25f, 0.25f);
        }

        private static void UpdateScene(Registry registry, float deltaTime)
        {
            if (registry.Transforms.TryGetValue(_movingQuad, out var transform) && (transform.X < -1f || transform.X > 1f))
            {
                transform.VelocityX = -transform.VelocityX;
                registry.Transforms[_movingQuad] = transform;
            }

            if (registry.Sprites.TryGetValue(_playerQuad, out var sprite))
            {
                bool overlaps = CollisionSystem.Overlaps(registry, _movingQuad, _playerQuad);
                sprite.R = overlaps ? 1.0f : 0.2f;
                registry.Sprites[_playerQuad] = sprite;
            }
        }

        private static void HandleInput(Registry registry, KeyboardState keyboard)
        {
            if (!registry.Transforms.TryGetValue(_playerQuad, out var player))
                return;

            int direction = (keyboard.Right ? 1 : 0) - (keyboard.Left ? 1 : 0);
            player.VelocityX = direction * 0.8f;
            registry.Transforms[_playerQuad] = player;
        }
    }
}

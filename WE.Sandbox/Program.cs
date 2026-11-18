using System.Numerics;
using WE.AssetPipeline.Serializers;
using WE.Core.Components;
using WE.Core.ECS;
using WE.Core.Systems;
using WE.Graphics;
using WE.Runtime;

namespace WE.Sandbox
{
    public class Program
    {
        private const float GroundSize = 20f;
        private const float GroundThickness = 0.2f;
        private const float PlayerHalfSize = 0.5f;
        private const float Speed = 3f;
        private static readonly Vector3 PlayerStart = new(0f, PlayerHalfSize, 0f);

        public static void Main(string[] args)
        {
            var camera = new Camera3D
            {
                Position = PlayerStart + new Vector3(10f, 9.6f, 10f),
                Target = PlayerStart,
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

            GameHost.Run(settings, loadScene: LoadScene, camera: camera);
        }

        private static PlayerControlSettings LoadScene(Registry registry)
        {
            const float movementLimit = GroundSize / 2f - PlayerHalfSize;

            int playerEntity = registry.CreateEntity();
            registry.Meshes[playerEntity] = new MeshComponent(PlayerStart, PrimitiveMeshes.Cube);

            int markerEntity = registry.CreateEntity();
            registry.Meshes[markerEntity] = new MeshComponent(
                new Vector3(1.6f, PlayerHalfSize, -1.1f), PrimitiveMeshes.Pyramid)
            {
                Color = new Vector3(1.0f, 0.55f, 0.2f)
            };

            int groundEntity = registry.CreateEntity();
            registry.Meshes[groundEntity] = new MeshComponent(
                new Vector3(0f, -GroundThickness / 2f, 0f), PrimitiveMeshes.Cube)
            {
                Scale = new Vector3(GroundSize, GroundThickness, GroundSize),
                Color = new Vector3(0.35f, 0.45f, 0.38f)
            };

            SceneSerializer.SaveScene(registry, "level1.json", playerEntity);

            return new PlayerControlSettings(playerEntity, Speed)
            {
                GroundY = 0f,
                IsTargetAllowed = point =>
                    MathF.Abs(point.X) <= GroundSize / 2f
                    && MathF.Abs(point.Z) <= GroundSize / 2f,
                ConstrainPosition = ClampToGround
            };

            static Vector3 ClampToGround(Vector3 position)
            {
                position.X = Math.Clamp(position.X, -movementLimit, movementLimit);
                position.Z = Math.Clamp(position.Z, -movementLimit, movementLimit);
                return position;
            }
        }
    }
}

using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;
using WE.AssetPipeline.Serializers;
using WE.Core.ECS;
using WE.Core.Time;
using WE.Graphics;
using WE.Runtime;

namespace WE.Launcher;

class Program
{
    private const float GroundSize = 20f;
    private const float PlayerHalfSize = 0.5f;
    private const float Speed = 3f;
    private static readonly Vector3 CameraStartTarget = new(0f, PlayerHalfSize, 0f);

    private const string StartScenePath = "Scenes/level1.json";

    static void Main(string[] args)
    {
        var camera = new Camera3D
        {
            Position = CameraStartTarget + new Vector3(10f, 9.6f, 10f),
            Target = CameraStartTarget,
            OrthographicHeight = 5f
        };

        var settings = new GameSettings
        {
            WindowTitle = "WoltechEngine - Standalone Game",
            MovementMode = PlayerMovementMode.ClickToMove,
            CameraMode = CameraControlMode.FreePan,
            CameraPanSpeed = 5f,
            UseOrthographicProjection = true,
            CameraInclination = 35
        };

        GameHost.Run(settings, loadScene: LoadSceneFromDisk, camera: camera);
    }

    private static PlayerControlSettings LoadSceneFromDisk(Registry registry)
    {
        if (!File.Exists(StartScenePath))
        {
            throw new FileNotFoundException($"Missing game scene file at: {Path.GetFullPath(StartScenePath)}");
        }

        int? playerEntityId = SceneSerializer.LoadScene(registry, StartScenePath);

        if (playerEntityId == null)
        {
            throw new InvalidDataException("Loaded scene does not define a valid PlayerEntityId.");
        }

        const float movementLimit = GroundSize / 2f - PlayerHalfSize;

        return new PlayerControlSettings(playerEntityId.Value, Speed)
        {
            GroundY = 0f,
            IsTargetAllowed = point =>
                MathF.Abs(point.X) <= GroundSize / 2f
                && MathF.Abs(point.Z) <= GroundSize / 2f,
            ConstrainPosition = position =>
            {
                position.X = Math.Clamp(position.X, -movementLimit, movementLimit);
                position.Z = Math.Clamp(position.Z, -movementLimit, movementLimit);
                return position;
            }
        };
    }
}
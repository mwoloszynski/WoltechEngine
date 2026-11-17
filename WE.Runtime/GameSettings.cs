namespace WE.Runtime;

public enum PlayerMovementMode
{
    Wasd,
    ClickToMove
}

public enum CameraControlMode
{
    FollowPlayer,
    FreePan
}

public sealed class GameSettings
{
    public string WindowTitle { get; init; } = "WoltechEngine";
    public PlayerMovementMode MovementMode { get; init; } = PlayerMovementMode.Wasd;
    public CameraControlMode CameraMode { get; init; } = CameraControlMode.FollowPlayer;
    public float CameraPanSpeed { get; init; } = 5f;
    public bool UseOrthographicProjection { get; init; }
    public int? CameraInclination { get; init; }
}
